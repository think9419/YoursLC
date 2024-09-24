#if mysql

using DapperExtensions.MySQLExt;

#endif
#if sqlserver

using DapperExtensions.SqlServerExt;

#endif

using System.Data;
using System.Collections.Generic;
using System;
using Dapper;

namespace Think9.Repository
{
    public class ComRepository
    {
        public DbContext dbContext = new DbContext();

        /// <summary>
        /// 执行存储过程返回list
        /// </summary>
        public IEnumerable<dynamic> GetListByStoreProcedure(string proName, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.Query(proName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        public int ExecuteStoreProcedure(string proName, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.Execute(proName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 获取已定义的所有视图
        /// </summary>
        //public DataTable GetViewsList(string connStr, string type)
        //{
        //    using (var conn = dbContext.GetConnection(connStr, type))
        //    {
        //        return conn.GetViewsList();
        //    }
        //}

        ///// <summary>
        ///// dapper通用分页方法
        ///// </summary>
        public DataTable GetPageList(string tableName, string files, string where, string orderby, int pageIndex, int pageSize, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetPageList(files, tableName, where, orderby, pageIndex, pageSize, param);
            }
        }

        /// <summary>
        /// 求值
        /// </summary>
        public string GetSingleField(string sql)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetSingleField(sql);
            }
        }

        /// <summary>
        /// 求值
        /// </summary>
        public string GetSingleField(string sql, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetSingleField(sql, param);
            }
        }

        /// <summary>
        /// 得到最大值
        /// </summary>
        public int GetMaxID(string FieldName, string TableName, string strWhere)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetMaxID(FieldName, TableName, strWhere);
            }
        }

        /// <summary>
        /// 返回自动增长列id
        /// </summary>
        public long InsertAndReturnID(string tableName, List<string> columns, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.InsertAndReturnID(tableName, columns, param);
            }
        }

        /// <summary>
        /// 返回自动增长列id
        /// </summary>
        public void Insert(string tableName, List<string> columns, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                conn.Insert(tableName, columns, param);
            }
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public DataTable GetTbFieldsList(string tbid)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetTbFieldsList(tbid);
            }
        }

        /// <summary>
        /// 获取已定义的所有视图
        /// </summary>
        public DataTable GetViewsList()
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetViewsList();
            }
        }

        /// <summary>
        /// 根据录入表编码、指标编码、及类型生成数据库插入字段sql
        /// </summary>
        public string GetAddTbFieldsSql(string tbid, string indexid, string indexname, string datatype)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetAddTbFieldsSql(tbid, indexid, indexname, datatype);
            }
        }

        /// <summary>
        /// 根据录入表编码、指标编码、及类型生成数据库插入字段sql
        /// </summary>
        public string GetAlterTbFieldsSql(string tbid, string indexid, string datatype)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetAlterTbFieldsSql(tbid, indexid, datatype);
            }
        }

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        public bool IsColumnExists(string TableName, string columnName)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.IsColumnExists(TableName, columnName);
            }
        }

        /// <summary>
        /// 获取数据表包含字段的字符串以#分割
        /// </summary>
        public string GetTableFieldsString(string tbid)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetTableFieldsString(tbid);
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        public bool IsDataTableExists(string TableName)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.IsDataTableExists(TableName);
            }
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public string GetCreatDBSql(string tbid, string type, string flid)
        {
            using (var conn = dbContext.GetConnection())
            {
                if (type == "1")
                {
                    return conn.GenerateMainTableCreationSQL(tbid, flid);
                }
                else
                {
                    return conn.GenerateGridTableCreationSQL(tbid);
                }
            }
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public string GetCreatDBSqlForAux(string tbid)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GenerateAuxTableCreationSQL(tbid);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public int ExecuteSql(string Sql, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.ExecuteSql(Sql, param);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public DataTable ExecuteSql(List<string> sqlList, object param)
        {
            DataTable dtResult = NewAffectedResultDt();
            using (var conn = dbContext.GetConnection())
            {
                conn.ExecuteSql(sqlList, param, dtResult);
                return dtResult;
            }
        }

        /// <summary>
        /// 生成自动编号数据建表语句
        /// </summary>
        public string GenerateRuleAutoTableCreationSQL(string rid)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GenerateRuleAutoTableCreationSQL(rid);
            }
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        public DataTable GetDataTable(string sql, object param = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetDataTable(sql, param);
            }
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        public long GetTotal(string tablename, string where = null, object param = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetTotal(tablename, where, param);
            }
        }

        public static DataTable NewAffectedResultDt()
        {
            DataTable dt = new DataTable("Result");
            dt.Columns.Add("Num", typeof(int));
            dt.Columns.Add("Sql", typeof(String));
            dt.Columns.Add("Err", typeof(String));
            dt.Columns.Add("Exa", typeof(String));
            return dt;
        }
    }
}