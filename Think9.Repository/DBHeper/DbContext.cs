using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Think9.Repository
{
    public class DbContext
    {
        public IConfiguration configuration { set; get; }

        /// <summary>
        /// 根据Key取Value值
        /// </summary>
        /// <param name="key"></param>
        public string GetDBProvider(string key)
        {
            string systemConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configs/appSet.config");
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(systemConfigPath);
            System.Xml.XmlNode xNode = xDoc.SelectSingleNode("//appSettings");
            System.Xml.XmlElement xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
            return xElem1.GetAttribute("value");
        }

        public System.Data.IDbConnection GetConnection()
        {
            string connectionString = GetDBProvider("ConnectionString");
            if (connectionString.Contains("%CONTENTROOTPATH%"))
            {
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", Path.Combine(Directory.GetCurrentDirectory(), ""));
            }

            //string _dbtype = GetDBProvider("DBProvider");//数据库类型
            //if(_dbtype == "mysql")
            //{
            //    var connection = new MySqlConnection(connectionString);
            //    connection.Open();
            //    return connection;
            //}

            //if (_dbtype == "sqlserver")
            //{
            //    var connection = new SqlConnection(connectionString);
            //    connection.Open();
            //    return connection;
            //}

#if mysql
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
#endif
#if sqlserver
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
#endif

            throw new Exception("不支持的数据库类型");
        }
    }
}