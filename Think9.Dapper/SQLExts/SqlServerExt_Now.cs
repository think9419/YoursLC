using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DapperExtensions.SqlServerExt
{
    public static partial class SqlServerExt
    {
        /// <summary>
        /// dapper通用分页方法
        /// </summary>
        /// <typeparam name="T">泛型集合实体类</typeparam>
        /// <param name="conn">数据库连接池连接对象</param>
        /// <param name="returnFields">列</param>
        /// <param name="tableName">表</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">当前页显示条数</param>
        /// <param name="total">结果集总数</param>
        /// <returns></returns>
        public static IEnumerable<T> GetPageList<T>(this IDbConnection conn, out long total, string returnFields, string tableName, string where, string orderby, int pageIndex, int pageSize, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @total INT;SELECT @total = COUNT(1) FROM [{0}] WITH(NOLOCK) {1};SELECT @total;", tableName, where);
            sb.Append("IF(@total>0) BEGIN ");
            if (pageIndex == 1)
            {
                sb.AppendFormat("SELECT TOP ({0}) {1} FROM [{2}] WITH(NOLOCK) {3} {4}", pageSize, returnFields, tableName, where, orderby);
            }
            else
            {
                sb.AppendFormat("WITH cte AS (SELECT ROW_NUMBER() OVER({0}) AS rownum,{1} FROM [{2}] WITH(NOLOCK) {3})", orderby, returnFields, tableName, where);
                if (returnFields.Contains(" AS") || returnFields.Contains(" as"))
                {
                    sb.AppendFormat("SELECT * FROM cte WHERE cte.rownum BETWEEN {0} AND {1}", skip + 1, skip + pageSize);
                }
                else
                {
                    sb.AppendFormat("SELECT {0} FROM cte WHERE cte.rownum BETWEEN {1} AND {2}", returnFields, skip + 1, skip + pageSize);
                }
            }
            sb.Append(" END");

            string sql = sb.ToString();
            using (var reader = conn.QueryMultiple(sql, param, transaction, commandTimeout))
            {
                total = reader.ReadFirst<int>();
                if (total > 0)
                {
                    return reader.Read<T>();
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        public static DataTable GetPageList(this IDbConnection conn, string files, string tableName, string where, string orderby, int pageIndex, int pageSize, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT {0} FROM {1} {2} {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY", files, tableName, where, orderby, skip, pageSize);
            string sql = sb.ToString();

            return DapperExtAllSQL.GetDataTableBase(conn, sql, param, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取分页数据 联合查询
        /// </summary>
        public static IEnumerable<T> GetByPageUnite<T>(this IDbConnection conn, int pageIndex, int pageSize, out long total, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM {0};", where);
            sb.AppendFormat("SELECT {0} FROM {1} {2} OFFSET {3} ROWS FETCH NEXT {4} ROWS ONLY", returnFields, where, orderBy, (pageIndex - 1) * pageSize, pageSize);

            string sql = sb.ToString();
            using (var reader = conn.QueryMultiple(sb.ToString(), param, transaction, commandTimeout))
            {
                total = reader.ReadFirst<long>();
                return reader.Read<T>();
            }
        }

        /// <summary>
        /// dapper通用分页方法
        /// </summary>
        /// <param name="conn">数据库连接池连接对象</param>
        /// <param name="files">列</param>
        /// <param name="tableName">表</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">当前页显示条数</param>
        /// <param name="total">结果集总数</param>
        /// <returns></returns>
        public static DataTable GetDataTablePageList(this IDbConnection conn, string files, string tableName, string where, string orderby, int pageIndex, int pageSize, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT {0} FROM {1} {2} {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY", files, tableName, where, orderby, skip, pageSize);

            string sql = sb.ToString();

            return DapperExtAllSQL.GetDataTableBase(conn, sql, param, transaction, commandTimeout);
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
                    orderBy = string.Format("ORDER BY [{0}] DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY [{0}]", sqls.AllFieldList.First());
                }
            }

            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @total INT;SELECT @total = COUNT(1) FROM [{0}] WITH(NOLOCK) {1};SELECT @total;", sqls.TableName, where);
            sb.Append("IF(@total>0) BEGIN ");
            if (pageIndex == 1)
            {
                sb.AppendFormat("SELECT TOP ({0}) {1} FROM [{2}] WITH(NOLOCK) {3} {4}", pageSize, returnFields, sqls.TableName, where, orderBy);
            }
            else
            {
                sb.AppendFormat("WITH cte AS (SELECT ROW_NUMBER() OVER({0}) AS rownum,{1} FROM [{2}] WITH(NOLOCK) {3})", orderBy, returnFields, sqls.TableName, where);
                if (returnFields.Contains(" AS") || returnFields.Contains(" as"))
                {
                    sb.AppendFormat("SELECT * FROM cte WHERE cte.rownum BETWEEN {0} AND {1}", skip + 1, skip + pageSize);
                }
                else
                {
                    sb.AppendFormat("SELECT {0} FROM cte WHERE cte.rownum BETWEEN {1} AND {2}", returnFields, skip + 1, skip + pageSize);
                }
            }
            sb.Append(" END");

            string sql = sb.ToString();
            using (var reader = conn.QueryMultiple(sql, param, transaction, commandTimeout))
            {
                total = reader.ReadFirst<int>();
                if (total > 0)
                {
                    return reader.Read<T>();
                }
                else
                {
                    return new List<T>();
                }
            }
        }
    }
}