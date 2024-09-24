using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DapperExtensions.PostgreSQLExt
{
    public static partial class PostgreSQLExt
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, DapperExtSqls> dapperExtsqlsDict = new ConcurrentDictionary<RuntimeTypeHandle, DapperExtSqls>();

        /// <summary>
        /// 求值
        /// </summary>
        public static string GetSingleField(this IDbConnection conn, string sql, object param)
        {
            object obj = conn.ExecuteScalar(sql, param);

            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return "";
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public static DataTable GetTbFieldsList(this IDbConnection conn, string tbID)
        {
            return GetDataTable(conn, "SELECT col_description(a.attrelid,a.attnum) as comment,pg_type.typname as DATA_type,a.attname as COLUMN_NAME, a.attnotnull as notnull FROM pg_class as c,pg_attribute as a inner join pg_type on pg_type.oid = a.atttypid where c.relname = '" + tbID + "' and a.attrelid = c.oid and a.attnum>0");
        }

        /// <summary>
        /// 获取已定义的所有录入表和视图
        /// </summary>
        public static DataTable GetTableAndViewList(this IDbConnection conn)
        {
            DataTable dt = new DataTable("table_name");
            dt.Columns.Add("table_name", typeof(String));

            foreach (DataRow dr in GetDataTable(conn, "SELECT table_name as table_name FROM information_schema.tables WHERE table_type='BASE TABLE' and table_schema='public'").Rows)
            {
                DataRow row = dt.NewRow();
                row["table_name"] = dr["table_name"].ToString();
                dt.Rows.Add(row);
            }

            foreach (DataRow dr in GetDataTable(conn, "SELECT table_name as table_name FROM information_schema.views where table_schema='public'").Rows)
            {
                DataRow row = dt.NewRow();
                row["table_name"] = dr["table_name"].ToString();
                dt.Rows.Add(row);
            }

            return dt;
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public static IEnumerable<T> GetBySql<T>(this IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetBySqlBase<T>(conn, typeof(T), sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        private static IEnumerable<T> GetBySqlBase<T>(this IDbConnection conn, Type t, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);

            return conn.Query<T>(sql, param, transaction, true, commandTimeout);
        }

        /// <summary>
        /// dapper通用分页方法
        /// </summary>
        /// <typeparam name="T">泛型集合实体类</typeparam>
        /// <param name="conn">数据库连接池连接对象</param>
        /// <param name="files">列</param>
        /// <param name="tableName">表</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">当前页显示条数</param>
        /// <param name="total">结果集总数</param>
        /// <returns></returns>
        public static IEnumerable<T> GetPageList<T>(this IDbConnection conn, out long total, string files, string tableName, string where, string orderby, int pageIndex, int pageSize, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM \"{0}\" {1};", tableName, where);

            sb.AppendFormat("SELECT {0} FROM \"{1}\" {2} {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY", files, tableName, where, orderby, skip, pageSize);
            string sql = sb.ToString();
            using (var reader = conn.QueryMultiple(sb.ToString(), param))
            {
                total = reader.ReadFirst<long>();
                return reader.Read<T>();
            }
        }

        public static DapperExtSqls GetDapperExtSqls(Type t)
        {
            if (dapperExtsqlsDict.Keys.Contains(t.TypeHandle))
            {
                return dapperExtsqlsDict[t.TypeHandle];
            }
            else
            {
                DapperExtSqls sqls = DapperExtCommon.GetDapperExtSqls(t);

                string Fields = DapperExtCommon.GetFieldsStr(sqls.AllFieldList, "\"", "\"");
                string FieldsAt = DapperExtCommon.GetFieldsAtStr(sqls.AllFieldList);
                string FieldsEq = DapperExtCommon.GetFieldsEqStr(sqls.AllFieldList, "\"", "\"");

                string FieldsExtKey = DapperExtCommon.GetFieldsStr(sqls.ExceptKeyFieldList, "\"", "\"");
                string FieldsAtExtKey = DapperExtCommon.GetFieldsAtStr(sqls.ExceptKeyFieldList);
                string FieldsEqExtKey = DapperExtCommon.GetFieldsEqStr(sqls.ExceptKeyFieldList, "\"", "\"");

                sqls.AllFields = Fields;

                if (sqls.HasKey && sqls.IsIdentity) //有主键并且是自增
                {
                    sqls.InsertSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", sqls.TableName, FieldsExtKey, FieldsAtExtKey);
                    sqls.InsertIdentitySql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", sqls.TableName, Fields, FieldsAt);
                }
                else
                {
                    sqls.InsertSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", sqls.TableName, Fields, FieldsAt);
                }

                if (sqls.HasKey) //含有主键
                {
                    sqls.DeleteByIdSql = string.Format("DELETE FROM \"{0}\" WHERE \"{1}\"=@id", sqls.TableName, sqls.KeyName);
                    sqls.DeleteByIdsSql = string.Format("DELETE FROM \"{0}\" WHERE \"{1}\" IN (@idreplace)", sqls.TableName, sqls.KeyName);
                    sqls.GetByIdSql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=@id", Fields, sqls.TableName, sqls.KeyName);
                    sqls.GetByIdsSql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN (@idreplace)", Fields, sqls.TableName, sqls.KeyName);
                    sqls.UpdateByIdSql = string.Format("UPDATE \"{0}\" SET {1} WHERE \"{2}\"=@{2}", sqls.TableName, FieldsEqExtKey, sqls.KeyName);
                }
                sqls.DeleteAllSql = string.Format("DELETE FROM \"{0}\"", sqls.TableName);
                sqls.GetAllSql = string.Format("SELECT {0} FROM \"{1}\"", Fields, sqls.TableName);

                dapperExtsqlsDict[t.TypeHandle] = sqls;
                return sqls;
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        public static dynamic Insert<T>(this IDbConnection conn, T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey && sqls.IsIdentity)
            {
                switch (sqls.KeyType)
                {
                    case "Int32": return conn.ExecuteScalar<int>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //int
                    case "Int64": return conn.ExecuteScalar<long>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //long
                    case "Decimal": return conn.ExecuteScalar<decimal>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //decimal
                    case "UInt32": return conn.ExecuteScalar<uint>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //uint
                    case "UInt64": return conn.ExecuteScalar<ulong>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //ulong
                    case "Double": return conn.ExecuteScalar<double>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //double
                    case "Single": return conn.ExecuteScalar<float>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //float
                    case "Byte": return conn.ExecuteScalar<byte>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout);  //byte
                    case "SByte": return conn.ExecuteScalar<sbyte>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //sbyte
                    case "Int16": return conn.ExecuteScalar<short>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //short
                    case "UInt16": return conn.ExecuteScalar<ushort>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //ushort
                    default: return conn.ExecuteScalar<dynamic>(sqls.InsertSql + "RETURNING " + sqls.KeyName, entity, transaction, commandTimeout); //dynamic
                }
            }
            else
            {
                return conn.Execute(sqls.InsertSql, entity, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 新增多条数据
        /// </summary>
        public static int InsertBatch<T>(this IDbConnection conn, IEnumerable<T> entitys, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            return conn.Execute(sqls.InsertSql, entitys, transaction, commandTimeout);
        }

        /// <summary>
        /// 新增(插入自增键)
        /// </summary>
        public static int InsertIdentity<T>(this IDbConnection conn, T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey && sqls.IsIdentity)
            {
                return conn.Execute(sqls.InsertIdentitySql, entity, transaction, commandTimeout);
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有自增键，无法进行InsertIdentity。");
            }
        }

        /// <summary>
        /// 新增多条记录(插入自增键)
        /// </summary>
        public static int InsertIdentityBatch<T>(this IDbConnection conn, IEnumerable<T> entitys, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey && sqls.IsIdentity)
            {
                return conn.Execute(sqls.InsertIdentitySql, entitys, transaction, commandTimeout);
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有自增键，无法进行InsertIdentity。");
            }
        }

        /// <summary>
        /// 根据Id，若存在则更新，不存在就插入
        /// </summary>
        public static dynamic InsertOrUpdate<T>(this IDbConnection conn, T entity, string updateFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                int result = UpdateById<T>(conn, entity, updateFields, transaction, commandTimeout);
                if (result == 0)
                    return Insert<T>(conn, entity, transaction, commandTimeout);
                return result;
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法进行InsertOrUpdate。");
            }
        }

        /// <summary>
        /// 根据Id，若存在则更新，不存在就插入，连id都一起插入
        /// </summary>
        public static dynamic InsertOrUpdateIdentity<T>(this IDbConnection conn, T entity, string updateFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey && sqls.IsIdentity)
            {
                int result = UpdateById<T>(conn, entity, updateFields, transaction, commandTimeout);
                if (result == 0)
                    return InsertIdentity<T>(conn, entity, transaction, commandTimeout);
                return result;
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有自增键，无法进行InsertOrUpdateIdentity。");
            }
        }

        /// <summary>
        /// 根据主键返回实体Base
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        private static T GetByIdBase<T>(this IDbConnection conn, Type t, dynamic id, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (sqls.HasKey)
            {
                DynamicParameters dpar = new DynamicParameters();
                dpar.Add("@id", id);
                if (returnFields == null)
                {
                    return conn.QueryFirstOrDefault<T>(sqls.GetByIdSql, dpar, transaction, commandTimeout);
                }
                else
                {
                    string sql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=@id", returnFields, sqls.TableName, sqls.KeyName);
                    return conn.QueryFirstOrDefault<T>(sql, dpar, transaction, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法GetById。");
            }
        }

        /// <summary>
        /// 根据主键返dynamic
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static dynamic GetByIdDynamic<T>(this IDbConnection conn, dynamic id, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                DynamicParameters dpar = new DynamicParameters();
                dpar.Add("@id", id);
                if (returnFields == null)
                {
                    return conn.QueryFirstOrDefault(sqls.GetByIdSql, dpar, transaction, commandTimeout);
                }
                else
                {
                    string sql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=@id", returnFields, sqls.TableName, sqls.KeyName);
                    return conn.QueryFirstOrDefault(sql, dpar, transaction, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法GetByIdDynamic。");
            }
        }

        /// <summary>
        /// 根据主键返回实体
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static T GetById<T>(this IDbConnection conn, dynamic id, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByIdBase<T>(conn, typeof(T), id, returnFields, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据主键返回任意类型实体
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static T GetById<Table, T>(this IDbConnection conn, dynamic id, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByIdBase<T>(conn, typeof(Table), id, returnFields, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据主键ids返回实体列表
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        private static IEnumerable<T> GetByIdsBase<T>(this IDbConnection conn, Type t, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (ids == null || ids.Count() == 0)
                return new List<T>();

            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (sqls.HasKey)
            {
                string idsStr = string.Join(",", ids);
                if (returnFields == null)
                {
                    string idssql = sqls.GetByIdsSql;
                    return conn.Query<T>(idssql.Replace("@idreplace", idsStr), null, transaction, true, commandTimeout);
                }
                else
                {
                    string sql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN ({3})", returnFields, sqls.TableName, sqls.KeyName, idsStr);
                    return conn.Query<T>(sql, null, transaction, true, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法GetByIds。");
            }
        }

        /// <summary>
        /// 根据主键ids返回dynamic列表
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<dynamic> GetByIdsDynamic<T>(this IDbConnection conn, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (ids == null || ids.Count() == 0)
                return new List<dynamic>();

            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                string idsStr = string.Join(",", ids);
                if (returnFields == null)
                {
                    string idssql = sqls.GetByIdsSql;
                    return conn.Query(idssql.Replace("@idreplace", idsStr), null, transaction, true, commandTimeout);
                }
                else
                {
                    string sql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN ({3})", returnFields, sqls.TableName, sqls.KeyName, idsStr);
                    return conn.Query(sql, null, transaction, true, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法GetByIds。");
            }
        }

        /// <summary>
        /// 根据主键ids返回任意类型实体列表
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<T> GetByIds<T>(this IDbConnection conn, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByIdsBase<T>(conn, typeof(T), ids, returnFields, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据主键ids返回任意类型实体列表
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<T> GetByIds<Table, T>(this IDbConnection conn, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByIdsBase<T>(conn, typeof(Table), ids, returnFields, transaction, commandTimeout);
        }

        /// <summary>
        /// 返回整张表数据
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        private static IEnumerable<T> GetAllBase<T>(this IDbConnection conn, Type t, string returnFields = null, string orderby = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (returnFields == null)
            {
                return conn.Query<T>(sqls.GetAllSql + " " + orderby, null, transaction, true, commandTimeout);
            }
            else
            {
                string sql = string.Format("SELECT {0} FROM \"{1}\" " + orderby, returnFields, sqls.TableName);
                return conn.Query<T>(sql, null, transaction, true, commandTimeout);
            }
        }

        /// <summary>
        /// 返回整张表数据
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<dynamic> GetAllDynamic<T>(this IDbConnection conn, string returnFields = null, string orderby = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (returnFields == null)
            {
                return conn.Query(sqls.GetAllSql + " " + orderby, null, transaction, true, commandTimeout);
            }
            else
            {
                string sql = string.Format("SELECT {0} FROM \"{1}\" " + orderby, returnFields, sqls.TableName);
                return conn.Query(sql, null, transaction, true, commandTimeout);
            }
        }

        /// <summary>
        /// 返回整张表数据
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<T> GetAll<T>(this IDbConnection conn, string returnFields = null, string orderby = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetAllBase<T>(conn, typeof(T), returnFields, orderby, transaction, commandTimeout);
        }

        /// <summary>
        /// 返回整张表任意类型数据
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<T> GetAll<Table, T>(this IDbConnection conn, string returnFields = null, string orderby = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetAllBase<T>(conn, typeof(Table), returnFields, orderby, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        public static int DeleteById<T>(this IDbConnection conn, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                DynamicParameters dpar = new DynamicParameters();
                dpar.Add("@id", id);
                return conn.Execute(sqls.DeleteByIdSql, dpar, transaction, commandTimeout);
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法DeleteById。");
            }
        }

        /// <summary>
        /// 根据主键批量删除数据
        /// </summary>
        public static int DeleteByIds<T>(this IDbConnection conn, IEnumerable<dynamic> ids, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (ids == null || ids.Count() == 0)
                return 0;

            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                string idsStr = string.Join(",", ids);
                return conn.Execute(sqls.DeleteByIdsSql.Replace("@idreplace", idsStr), null, transaction, commandTimeout);
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法DeleteById。");
            }
        }

        /// <summary>
        /// 删除整张表数据
        /// </summary>
        public static int DeleteAll<T>(this IDbConnection conn, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            return conn.Execute(sqls.DeleteAllSql, null, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public static int DeleteByWhere<T>(this IDbConnection conn, string where, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            string sql = string.Format("DELETE FROM \"{0}\" {1}", sqls.TableName, where);
            return conn.Execute(sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据主键修改数据
        /// </summary>
        public static int UpdateById<T>(this IDbConnection conn, T entity, string updateFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                if (updateFields == null)
                {
                    return conn.Execute(sqls.UpdateByIdSql, entity, transaction, commandTimeout);
                }
                else
                {
                    string updateList = DapperExtCommon.GetFieldsEqStr(updateFields.Split(',').ToList(), "\"", "\"");
                    string sql = string.Format("UPDATE \"{0}\" SET {1} WHERE \"{2}\"=@{2}", sqls.TableName, updateList, sqls.KeyName);
                    return conn.Execute(sql, entity, transaction, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法UpdateById。");
            }
        }

        /// <summary>
        /// 根据主键修改数据
        /// </summary>
        public static int UpdateByIdBatch<T>(this IDbConnection conn, IEnumerable<T> entitys, string updateFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                if (updateFields == null)
                {
                    return conn.Execute(sqls.UpdateByIdSql, entitys, transaction, commandTimeout);
                }
                else
                {
                    string updateList = DapperExtCommon.GetFieldsEqStr(updateFields.Split(',').ToList(), "\"", "\"");
                    string sql = string.Format("UPDATE \"{0}\" SET {1} WHERE \"{2}\"=@{2}", sqls.TableName, updateList, sqls.KeyName);
                    return conn.Execute(sql, entitys, transaction, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法UpdateById。");
            }
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        public static int UpdateByWhere<T>(this IDbConnection conn, string where, string updateFields, object entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            updateFields = DapperExtCommon.GetFieldsEqStr(updateFields.Split(',').ToList(), "\"", "\"");
            string sql = string.Format("UPDATE \"{0}\" SET {1} {2}", sqls.TableName, updateFields, where);
            return conn.Execute(sql, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <returns></returns>
        public static long GetTotal<T>(this IDbConnection conn, string where = null, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            string sql = string.Format("SELECT COUNT(1) FROM \"{0}\" {1}", sqls.TableName, where);
            return conn.ExecuteScalar<long>(sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// Base获取数据,使用skip 和take
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<T> GetBySkipBase<T>(this IDbConnection conn, Type t, int skip, int take, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (returnFields == null)
                returnFields = sqls.AllFields;

            if (orderBy == null)
            {
                if (sqls.HasKey)
                {
                    orderBy = string.Format("ORDER BY \"{0}\" DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY \"{0}\"", sqls.AllFieldList.First());
                }
            }

            string sql = string.Format("SELECT {0} FROM \"{1}\" {2} {3} LIMIT {4} offset {5}", returnFields, sqls.TableName, where, orderBy, take, skip);

            return conn.Query<T>(sql, param, transaction, true, commandTimeout);
        }

        /// <summary>
        /// 获取dynamic数据,使用skip 和take
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<dynamic> GetBySkipDynamic<T>(this IDbConnection conn, int skip, int take, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (returnFields == null)
                returnFields = sqls.AllFields;

            if (orderBy == null)
            {
                if (sqls.HasKey)
                {
                    orderBy = string.Format("ORDER BY \"{0}\" DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY \"{0}\"", sqls.AllFieldList.First());
                }
            }

            string sql = string.Format("SELECT {0} FROM \"{1}\" {2} {3} LIMIT {4} offset {5}", returnFields, sqls.TableName, where, orderBy, take, skip);
            return conn.Query(sql, param, transaction, true, commandTimeout);
        }

        /// <summary>
        /// 获取dynamic分页数据
        /// </summary>
        public static IEnumerable<dynamic> GetByPageIndexDynamic<T>(this IDbConnection conn, int pageIndex, int pageSize, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }
            return GetBySkipDynamic<T>(conn, skip, pageSize, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取数据,使用skip 和take
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GetBySkip<T>(this IDbConnection conn, int skip, int take, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetBySkipBase<T>(conn, typeof(T), skip, take, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取数据，使用skip 和take,返回任意类型数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GetBySkip<Table, T>(this IDbConnection conn, int skip, int take, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetBySkipBase<T>(conn, typeof(Table), skip, take, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public static IEnumerable<T> GetByPageIndex<T>(this IDbConnection conn, int pageIndex, int pageSize, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }
            return GetBySkip<T>(conn, skip, pageSize, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取分页数据,返回任意类型数据
        /// </summary>
        public static IEnumerable<T> GetByPageIndex<Table, T>(this IDbConnection conn, int pageIndex, int pageSize, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }
            return GetBySkip<Table, T>(conn, skip, pageSize, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        private static IEnumerable<T> GetByWhereBase<T>(this IDbConnection conn, Type t, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM \"{1}\" {2} {3}", returnFields, sqls.TableName, where, orderBy);

            return conn.Query<T>(sql, param, transaction, true, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取Dynamic数据
        /// </summary>
        public static IEnumerable<dynamic> GetByWhereDynamic<T>(this IDbConnection conn, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM \"{1}\" {2} {3}", returnFields, sqls.TableName, where, orderBy);

            return conn.Query(sql, param, transaction, true, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public static IEnumerable<T> GetByWhere<T>(this IDbConnection conn, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByWhereBase<T>(conn, typeof(T), where, param, returnFields, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public static IEnumerable<T> GetByWhere<Table, T>(this IDbConnection conn, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByWhereBase<T>(conn, typeof(Table), where, param, returnFields, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        public static DataTable GetDataTable(this IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return DapperExtAllSQL.GetDataTableBase(conn, sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        public static DataSet GetDataSet(this IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return DapperExtAllSQL.GetDataSetBase(conn, sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取表结构，返回DataTable
        /// </summary>
        public static DataTable GetSchemaTable<T>(this IDbConnection conn, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (returnFields == null)
            {
                returnFields = sqls.AllFields;
            }

            string sql = string.Format("SELECT {0} FROM \"{1}\" LIMIT 0", returnFields, sqls.TableName);
            return GetDataTable(conn, sql, null, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        private static IEnumerable<T> GetByPageBase<T>(this IDbConnection conn, Type t, int pageIndex, int pageSize, out long total, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (returnFields == null)
                returnFields = sqls.AllFields;

            if (orderBy == null)
            {
                if (sqls.HasKey)
                {
                    orderBy = string.Format("ORDER BY \"{0}\" DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY \"{0}\"", sqls.AllFieldList.First());
                }
            }

            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM \"{0}\" {1};", sqls.TableName, where);
            sb.AppendFormat("SELECT {0} FROM \"{1}\" {2} {3} LIMIT {4} offset {5}", returnFields, sqls.TableName, where, orderBy, pageSize, skip);
            using (var reader = conn.QueryMultiple(sb.ToString(), param, transaction, commandTimeout))
            {
                total = reader.ReadFirst<long>();
                return reader.Read<T>();
            }
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public static IEnumerable<dynamic> GetByPageDynamic<T>(this IDbConnection conn, int pageIndex, int pageSize, out long total, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (returnFields == null)
                returnFields = sqls.AllFields;

            if (orderBy == null)
            {
                if (sqls.HasKey)
                {
                    orderBy = string.Format("ORDER BY \"{0}\" DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY \"{0}\"", sqls.AllFieldList.First());
                }
            }

            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM \"{0}\" {1};", sqls.TableName, where);
            sb.AppendFormat("SELECT {0} FROM \"{1}\" {2} {3} LIMIT {4} offset {5}", returnFields, sqls.TableName, where, orderBy, pageSize, skip);
            using (var reader = conn.QueryMultiple(sb.ToString(), param, transaction, commandTimeout))
            {
                total = reader.ReadFirst<long>();
                return reader.Read();
            }
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public static IEnumerable<T> GetByPage<T>(this IDbConnection conn, int pageIndex, int pageSize, out long total, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByPageBase<T>(conn, typeof(T), pageIndex, pageSize, out total, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public static IEnumerable<T> GetByPage<Table, T>(this IDbConnection conn, int pageIndex, int pageSize, out long total, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByPageBase<T>(conn, typeof(Table), pageIndex, pageSize, out total, returnFields, where, param, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        private static T GetByWhereFirstBase<T>(this IDbConnection conn, Type t, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM \"{1}\" {2} {3}", returnFields, sqls.TableName, where, orderBy);

            return conn.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取Dynamic数据
        /// </summary>
        public static dynamic GetByWhereFirstDynamic<T>(this IDbConnection conn, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM \"{1}\" {2} {3}", returnFields, sqls.TableName, where, orderBy);

            return conn.QueryFirstOrDefault(sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public static T GetByWhereFirst<T>(this IDbConnection conn, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByWhereFirstBase<T>(conn, typeof(T), where, param, returnFields, orderBy, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public static T GetByWhereFirst<Table, T>(this IDbConnection conn, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByWhereFirstBase<T>(conn, typeof(Table), where, param, returnFields, orderBy, transaction, commandTimeout);
        }

        private static IEnumerable<T> GetByInBase<T>(this IDbConnection conn, Type t, string field, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (ids == null || ids.Count() == 0)
                return new List<T>();

            DapperExtSqls sqls = GetDapperExtSqls(t);
            string idsStr = string.Join(",", ids);
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN ({3})", returnFields, sqls.TableName, field, idsStr);
            return conn.Query<T>(sql, null, transaction, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetByInDynamic<T>(this IDbConnection conn, string field, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (ids == null || ids.Count() == 0)
                return new List<dynamic>();

            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            string idsStr = string.Join(",", ids);
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN ({3})", returnFields, sqls.TableName, field, idsStr);
            return conn.Query(sql, null, transaction, true, commandTimeout);
        }

        public static IEnumerable<T> GetByIn<T>(this IDbConnection conn, string field, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByInBase<T>(conn, typeof(T), field, ids, returnFields, transaction, commandTimeout);
        }

        public static IEnumerable<T> GetByIn<Table, T>(this IDbConnection conn, string field, IEnumerable<dynamic> ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByInBase<T>(conn, typeof(Table), field, ids, returnFields, transaction, commandTimeout);
        }
    }
}