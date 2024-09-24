using DapperExtensions.MySQLExt;
using DapperExtensions.OracleExt;
using DapperExtensions.PostgreSQLExt;
using DapperExtensions.SqlServerExt;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Think9.Models;
using Think9.Repository;

namespace Think9.Services.Base
{
    public class BaseService<T> where T : class, new()
    {
        public BaseRepository<T> BaseRepository = new BaseRepository<T>();

        /// <summary>
        /// 获取分页数据 联合查询
        /// </summary>
        public IEnumerable<dynamic> GetPageUnite(ref long total, T filter, PageInfoEntity pageInfo, string where = null)
        {
            string _orderBy = string.Empty;
            if (!string.IsNullOrEmpty(pageInfo.field))
            {
                if (pageInfo.field.ToUpper().Trim().StartsWith("ORDER BY "))
                {
                    _orderBy = string.Format(" {0} ", pageInfo.field);
                }
                else
                {
                    _orderBy = string.Format(" ORDER BY {0} {1}", pageInfo.field, pageInfo.order);
                }
            }
            else
            {
                //_orderBy = " ORDER BY createTime desc";
            }

            var list = BaseRepository.GetByPageUnite(new SearchFilterEntity { pageIndex = pageInfo.page, pageSize = pageInfo.limit, returnFields = pageInfo.returnFields, param = filter, where = where, orderBy = _orderBy }, out total);

            return list;
        }

        /// <summary>
        /// 获取分页数据 联合查询
        /// </summary>
        public IEnumerable<dynamic> GetPageUnite(ref long total, T filter, PageInfoEntity pageInfo, string where = null, string orderBy = null)
        {
            orderBy = string.IsNullOrEmpty(orderBy) ? "" : orderBy;

            var list = BaseRepository.GetByPageUnite(new SearchFilterEntity { pageIndex = pageInfo.page, pageSize = pageInfo.limit, returnFields = pageInfo.returnFields, param = filter, where = where, orderBy = orderBy }, out total);

            return list;
        }

        /// <summary>
        /// 获取分页数据--
        /// </summary>
        public IEnumerable<dynamic> GetPageList(ref long total, PageInfoEntity pageInfo, string some, string tableName, string where, string orderby, object param)
        {
            var list = BaseRepository.GetPageList(out total, some, tableName, where, orderby, pageInfo.page, pageInfo.limit, param);

            return list;
        }

        /// <summary>
        /// 获取分页数据--从其他数据库读取
        /// </summary>
        public IEnumerable<dynamic> GetPageList(ref long total, string connString, string dbType, PageInfoEntity pageInfo, string some, string tableName, string where, string orderby, object param)
        {
            if (dbType == "sqlserver")
            {
                var connection = new SqlConnection(connString);
                return SqlServerExt.GetPageList<T>(connection, out total, some, tableName, where, orderby, pageInfo.page, pageInfo.limit, param);
            }
            if (dbType == "mysql")
            {
                var connection = new MySqlConnection(connString);
                return MySQLExt.GetPageList<T>(connection, out total, some, tableName, where, orderby, pageInfo.page, pageInfo.limit, param);
            }
            if (dbType == "postgresql")
            {
                var connection = new NpgsqlConnection(connString);
                return PostgreSQLExt.GetPageList<T>(connection, out total, some, tableName, where, orderby, pageInfo.page, pageInfo.limit, param);
            }
            if (dbType == "oracle")
            {
                var connection = new OracleConnection(connString);
                return OracleExt.GetPageList<T>(connection, out total, some, tableName, where, orderby, pageInfo.page, pageInfo.limit, param);
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 获取分页数据--
        /// </summary>
        public IEnumerable<dynamic> GetPageByFilter(ref long total, T filter, PageInfoEntity pageInfo, string where = null)
        {
            string _orderBy = string.Empty;
            if (!string.IsNullOrEmpty(pageInfo.field))
            {
                if (pageInfo.field.ToUpper().Trim().StartsWith("ORDER BY "))
                {
                    _orderBy = string.Format(" {0} ", pageInfo.field);
                }
                else
                {
                    if (pageInfo.field.ToUpper().Trim().EndsWith(" DESC") || pageInfo.field.ToUpper().Trim().EndsWith(" ASC"))
                    {
                        _orderBy = string.Format(" ORDER BY {0} ", pageInfo.field);
                    }
                    else
                    {
                        _orderBy = string.Format(" ORDER BY {0} {1}", pageInfo.field, pageInfo.order);
                    }
                }
            }
            else
            {
                //_orderBy = " ORDER BY createTime desc";
            }

            var list = BaseRepository.GetByPage(new SearchFilterEntity { pageIndex = pageInfo.page, pageSize = pageInfo.limit, returnFields = pageInfo.returnFields, param = filter, where = where, orderBy = _orderBy }, out total);

            return list;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public dynamic GetPageByFilter(T filter, PageInfoEntity pageInfo, string where = null)
        {
            string _orderBy = string.Empty;
            if (!string.IsNullOrEmpty(pageInfo.field))
            {
                if (pageInfo.field.ToUpper().Trim().StartsWith("ORDER BY "))
                {
                    _orderBy = string.Format(" {0} ", pageInfo.field);
                }
                else
                {
                    _orderBy = string.Format(" ORDER BY {0} {1}", pageInfo.field, pageInfo.order);
                }
            }
            else
            {
                //_orderBy = " ORDER BY createTime desc";
            }

            long total = 0;

            var list = BaseRepository.GetByPage(new SearchFilterEntity { pageIndex = pageInfo.page, pageSize = pageInfo.limit, returnFields = pageInfo.returnFields, param = filter, where = where, orderBy = _orderBy }, out total);

            return PagerEntity.Paging(list, total);
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        public int UpdateByWhere(string where, string updateFields, object entity)
        {
            return BaseRepository.UpdateByWhere(where, updateFields, entity);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public T GetByWhereFirst(string where = null, object param = null)
        {
            return BaseRepository.GetByWhereFirst(where, param);
        }

        /// <summary>
        /// 根据主键返回实体
        /// </summary>
        public IEnumerable<T> GetByIdsBase(object ids)
        {
            return BaseRepository.GetByIdsBase(ids);
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        public long GetTotal(string where = null, object param = null)
        {
            return BaseRepository.GetTotal(where, param);
        }

        #region CRUD

        /// <summary>
        /// 根据主键返回实体
        /// </summary>
        public T GetById(int Id)
        {
            return BaseRepository.GetById(Id);
        }

        /// <summary>
        /// 根据主键返回实体
        /// </summary>
        public T GetById(long Id)
        {
            return BaseRepository.GetById(Id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        public bool Insert(T model)
        {
            return BaseRepository.Insert(model) > 0 ? true : false;
        }

        /// <summary>
        /// 新增
        /// </summary>
        public dynamic InsertReturnID(T model)
        {
            return BaseRepository.Insert(model);
        }

        /// <summary>
        /// 根据主键修改数据
        /// </summary>
        public bool UpdateById(T model)
        {
            return BaseRepository.UpdateById(model) > 0 ? true : false;
        }

        /// <summary>
        /// 根据主键修改数据 修改指定字段
        /// </summary>
        public bool UpdateById(T model, string updateFields)
        {
            return BaseRepository.UpdateById(model, updateFields) > 0 ? true : false;
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        public bool DeleteById(int Id)
        {
            return BaseRepository.DeleteById(Id) > 0 ? true : false;
        }

        /// <summary>
        /// 根据主键批量删除数据
        /// </summary>
        public bool DeleteByIds(object Ids)
        {
            return BaseRepository.DeleteByIds(Ids) > 0 ? true : false;
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public bool DeleteByWhere(string where)
        {
            return BaseRepository.DeleteByWhere(where) > 0 ? true : false;
        }

        #endregion CRUD

        /// <summary>
        /// 返回整张表数据
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public IEnumerable<T> GetAll(string returnFields = null, string orderby = null)
        {
            return BaseRepository.GetAll(returnFields, orderby);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public IEnumerable<T> GetByWhere(string where = null, object param = null, string returnFields = null, string orderby = null)
        {
            return BaseRepository.GetByWhere(where, param, returnFields, orderby);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public IEnumerable<dynamic> GetListByWhere(string where = null, object param = null, string orderby = null)
        {
            return BaseRepository.GetByWhere(where, param, null, orderby);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public IEnumerable<T> GetBySql(string sql, object param = null)
        {
            return BaseRepository.GetBySql(sql, param);
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public IEnumerable<T> GetBySql(string connString, string dbType, string sql, object param = null)
        {
            if (dbType == "sqlserver")
            {
                var connection = new SqlConnection(connString);
                return SqlServerExt.GetBySql<T>(connection, sql, param);
            }
            if (dbType == "mysql")
            {
                var connection = new MySqlConnection(connString);
                return MySQLExt.GetBySql<T>(connection, sql, param);
            }
            if (dbType == "postgresql")
            {
                var connection = new NpgsqlConnection(connString);
                return PostgreSQLExt.GetBySql<T>(connection, sql, param);
            }
            if (dbType == "oracle")
            {
                var connection = new OracleConnection(connString);
                return OracleExt.GetBySql<T>(connection, sql, param);
            }
            throw new Exception("不支持的数据库类型");
        }

        ///// <summary>
        ///// 根据查询条件获取数据
        ///// </summary>
        //public DataTable GetDataTable(string connString, string dbType, string tbName, string some, string where, string order, object param = null)
        //{
        //    string sql = "";
        //    if (string.IsNullOrEmpty(where))
        //    {
        //        sql = "SELECT  " + some + " FROM " + tbName + "  " + order;
        //    }
        //    else
        //    {
        //        string _where = where.ToLower().StartsWith("where ") ? where : "WHERE " + where;
        //        sql = "SELECT  " + some + " FROM " + tbName + "  " + _where + "  " + order;
        //    }

        //    if (dbType == "sqlserver")
        //    {
        //        var connection = new SqlConnection(connString);
        //        return SqlServerExt.GetDataTable(connection, sql, param);
        //    }
        //    if (dbType == "mysql")
        //    {
        //        var connection = new MySqlConnection(connString);
        //        return MySQLExt.GetDataTable(connection, sql, param);
        //    }
        //    if (dbType == "postgresql")
        //    {
        //        var connection = new NpgsqlConnection(connString);
        //        return PostgreSQLExt.GetDataTable(connection, sql, param);
        //    }
        //    if (dbType == "oracle")
        //    {
        //        var connection = new OracleConnection(connString);
        //        return OracleExt.GetDataTable(connection, sql, param);
        //    }
        //    throw new Exception("不支持的数据库类型");
        //}
    }
}