#if mysql

using DapperExtensions.MySQLExt;

#endif
#if sqlserver

using DapperExtensions.SqlServerExt;

#endif

using Think9.IRepository;
using Think9.Models;
using System.Data;
using System.Collections.Generic;

namespace Think9.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
    {
        public DbContext dbContext = new DbContext();

        ///// <summary>
        ///// dapper通用分页方法
        ///// </summary>
        public IEnumerable<T> GetPageList(out long total, string files, string tableName, string where, string orderby, int pageIndex, int pageSize, object param)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetPageList<T>(out total, files, tableName, where, orderby, pageIndex, pageSize, param);
            }
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        public int UpdateByWhere(string where, string updateFields, object entity)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.UpdateByWhere<T>(where, updateFields, entity);
            }
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public T GetByWhereFirst(string where = null, object param = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetByWhereFirst<T>(where, param);
            }
        }

        /// <summary>
        /// 根据主键返回实体--这个不能使用要删除
        /// </summary>
        public IEnumerable<T> GetByIdsBase(object ids)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetByIds<T>(ids);
            }
        }

        #region CRUD

        /// <summary>
        /// 根据主键返回实体
        /// </summary>
        public T GetById(int Id)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetById<T>(Id);
            }
        }

        /// <summary>
        /// 根据主键返回实体
        /// </summary>
        public T GetById(long Id)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetById<T>(Id);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        public dynamic Insert(T model)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.Insert<T>(model);
            }
        }

        /// <summary>
        /// 根据主键修改数据
        /// </summary>
        public int UpdateById(T model)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.UpdateById<T>(model);
            }
        }

        /// <summary>
        /// 根据主键修改数据 修改指定字段
        /// </summary>
        public int UpdateById(T model, string updateFields)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.UpdateById<T>(model, updateFields);
            }
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        public int DeleteById(int Id)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.DeleteById<T>(Id);
            }
        }

        /// <summary>
        /// 根据主键批量删除数据
        /// </summary>
        public int DeleteByIds(object Ids)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.DeleteByIds<T>(Ids);
            }
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        public int DeleteByWhere(string where)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.DeleteByWhere<T>(where);
            }
        }

        #endregion CRUD

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public IEnumerable<T> GetByPage(SearchFilterEntity filter, out long total)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetByPage<T>(filter.pageIndex, filter.pageSize, out total, filter.returnFields, filter.where, filter.param, filter.orderBy, filter.transaction, filter.commandTimeout);
            }
        }

        /// <summary>
        /// 获取分页数据 联合查询
        /// </summary>
        public IEnumerable<T> GetByPageUnite(SearchFilterEntity filter, out long total)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetByPageUnite<T>(filter.pageIndex, filter.pageSize, out total, filter.returnFields, filter.where, filter.param, filter.orderBy, filter.transaction, filter.commandTimeout);
            }
        }

        /// <summary>
        /// 返回整张表数据
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public IEnumerable<T> GetAll(string returnFields = null, string orderby = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetAll<T>(returnFields, orderby);
            }
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public IEnumerable<T> GetByWhere(string where = null, object param = null, string returnFields = null, string orderby = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetByWhere<T>(where, param, returnFields, orderby);
            }
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public IEnumerable<T> GetBySql(string sql, object param = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetBySql<T>(sql, param);
            }
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        public long GetTotal(string where = null, object param = null)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.GetTotal<T>(where, param);
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
    }
}