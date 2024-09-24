using DapperExtensions.MySQLExt;
using DapperExtensions.OracleExt;
using DapperExtensions.PostgreSQLExt;
using DapperExtensions.SqlServerExt;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using Think9.Models;
using Think9.Repository;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class ExtraDbService : BaseService<ExtraDbEntity>
    {
        public ComRepository comRepository = new ComRepository();

        public ExtraDbEntity GetModel(int dbId)
        {
            string where = "where DbID=@DbID";
            object param = new { DbID = dbId };

            return base.GetByWhereFirst(where, param);
        }

        /// <summary>
        /// 得到外部数据库对象 可改写为从配置文件中获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ExtraDbEntity GetModel(string name)
        {
            string where = "where DbName=@DbName";
            object param = new { DbName = name };

            return base.GetByWhereFirst(where, param);
        }

        public System.Data.IDbConnection GetConnection(string connStr, string type)
        {
            if (type == "sqlserver")
            {
                var connection = new SqlConnection(connStr);
                return connection;
            }
            if (type == "mysql")
            {
                var connection = new MySqlConnection(connStr);
                return connection;
            }
            if (type == "postgresql")
            {
                var connection = new NpgsqlConnection(connStr);
                return connection;
            }
            if (type == "oracle")
            {
                var connection = new OracleConnection(connStr);
                return connection;
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public DataTable GetTbFieldsList(string connectionString, string type, string dbName, string tbId)
        {
            if (type == "sqlserver")
            {
                var connection = new SqlConnection(connectionString);
                return SqlServerExt.GetTbFieldsList(connection, tbId);
            }
            if (type == "mysql")
            {
                var connection = new MySqlConnection(connectionString);
                return MySQLExt.GetTbFieldsList(connection, dbName, tbId);
            }
            if (type == "postgresql")
            {
                var connection = new NpgsqlConnection(connectionString);
                return PostgreSQLExt.GetTbFieldsList(connection, tbId);
            }
            if (type == "oracle")
            {
                var connection = new OracleConnection(connectionString);
                return OracleExt.GetTbFieldsList(connection, tbId);
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 获取已定义的所有视图和数据表
        /// </summary>
        public DataTable GetTableAndViewList(string connectionString, string type, string dbName)
        {
            if (type == "sqlserver")
            {
                var connection = new SqlConnection(connectionString);
                return SqlServerExt.GetTableAndViewList(connection);
            }
            if (type == "mysql")
            {
                var connection = new MySqlConnection(connectionString);
                return MySQLExt.GetTableAndViewList(connection, dbName);
            }
            if (type == "postgresql")
            {
                var connection = new NpgsqlConnection(connectionString);
                return PostgreSQLExt.GetTableAndViewList(connection);
            }
            if (type == "oracle")
            {
                var connection = new OracleConnection(connectionString);
                return OracleExt.GetTableAndViewList(connection, dbName);
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        public DataTable GetDataTable(string conString, string type, string sql, object param)
        {
            if (type == "sqlserver")
            {
                var connection = new SqlConnection(conString);
                return SqlServerExt.GetDataTable(connection, sql, param);
            }
            if (type == "mysql")
            {
                var connection = new MySqlConnection(conString);
                return MySQLExt.GetDataTable(connection, sql, param);
            }
            if (type == "postgresql")
            {
                var connection = new NpgsqlConnection(conString);
                return PostgreSQLExt.GetDataTable(connection, sql, param);
            }
            if (type == "oracle")
            {
                var connection = new OracleConnection(conString);
                return OracleExt.GetDataTable(connection, sql, param);
            }
            throw new Exception("不支持的数据库类型");
        }
    }
}