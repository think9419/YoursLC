using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class ExternalDbService : BaseService<ExtraDbEntity>
    {
        private ComService comService = new ComService();

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

        public DataTable GetList()
        {
            DataTable dtReturn = DataTableHelp.NewValueTextDt();

            string sql = "select *  from externaldb";
            DataTable dtTb = comService.GetDataTable(sql);
            foreach (DataRow dr in dtTb.Rows)
            {
                DataRow row = dtReturn.NewRow();
                row["Value"] = dr["DbID"].ToString();
                row["Text"] = dr["DbName"].ToString() + " - {" + dr["DbType"].ToString() + "}";
                dtReturn.Rows.Add(row);
            }

            return dtReturn;
        }

        public ExtraDbEntity GetModel(int dbId)
        {
            string where = "where DbID=@DbID";
            object param = new { DbID = dbId };
            return base.GetByWhereFirst(where, param);
        }

        public ExtraDbEntity GetModel(string name)
        {
            string where = "where DbName=@DbName";
            object param = new { DbName = name };
            return base.GetByWhereFirst(where, param);
        }

        public static string GetName(int dbId)
        {
            ComService comService = new ComService();
            string name = "";
            if (dbId == 0)
            {
                name = "当前数据库";
            }
            else
            {
                var dt = comService.GetDataTable("select * from externaldb where DbID=" + dbId + "");
                if (dt.Rows.Count > 0)
                {
                    name = "外部数据库 - " + dt.Rows[0]["DbName"].ToString() + " - {" + dt.Rows[0]["DbType"].ToString() + "}";
                }
                else
                {
                    name = "ERR：外部数据库不存在";
                }
            }
            return name;
        }
    }
}