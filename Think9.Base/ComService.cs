using DapperExtensions.MySQLExt;
using DapperExtensions.OracleExt;
using DapperExtensions.PostgreSQLExt;
using DapperExtensions.SqlServerExt;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Think9.Models;
using Think9.Repository;

namespace Think9.Services.Base
{
    public class ComService
    {
        public ComRepository comRepository = new ComRepository();
        private WriteBackService writeBackService = new WriteBackService();
        private SqlAndParamService sqlAndParamService = new SqlAndParamService();

        public IEnumerable<dynamic> GetListByStoreProcedure(string proName, object param)
        {
            try
            {
                return comRepository.GetListByStoreProcedure(proName, param);
            }
            catch (Exception ex)
            {
                throw new Exception("存储过程{" + proName + "}出现错误", ex);
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="proName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteStoreProcedure(string proName, object param)
        {
            try
            {
                return comRepository.ExecuteStoreProcedure(proName, param);
            }
            catch (Exception ex)
            {
                throw new Exception("存储过程{" + proName + "}出现错误", ex);
            }
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public DataTable GetPageList(string tbName, string files, string where, string orderby, int pageIndex, int pageSize, object param)
        {
            try
            {
                return comRepository.GetPageList(tbName, files, where, orderby, pageIndex, pageSize, param);
            }
            catch (Exception ex)
            {
                throw new Exception("函数comRepository.GetPageList{" + "(" + tbName + ", " + files + ", " + where + ", " + orderby + ", " + files + ", ....)" + "}出现错误", ex);
            }
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public DataTable GetTbFieldsList(string tbId)
        {
            return comRepository.GetTbFieldsList(tbId);
        }

        /// <summary>
        /// 获取已定义的所有视图
        /// </summary>
        public DataTable GetViewsList()
        {
            return comRepository.GetViewsList();
        }

        /// <summary>
        ///   求值  , object param
        /// </summary>
        public string GetSingleField(string sql)
        {
            try
            {
                return comRepository.GetSingleField(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("{" + sql + "}错误", ex);
            }
        }

        /// <summary>
        ///   求值
        /// </summary>
        public string GetSingleField(string sql, object param)
        {
            try
            {
                return comRepository.GetSingleField(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("{" + sql + "}错误", ex);
            }
        }

        /// <summary>
        /// 求值
        /// </summary>
        public string GetSingleField(string conString, string type, string sql, object param)
        {
            if (type == "sqlserver")
            {
                var connection = new SqlConnection(conString);
                return SqlServerExt.GetSingleField(connection, sql, param);
            }
            if (type == "mysql")
            {
                var connection = new MySqlConnection(conString);
                return MySQLExt.GetSingleField(connection, sql, param);
            }
            if (type == "postgresql")
            {
                var connection = new NpgsqlConnection(conString);
                return PostgreSQLExt.GetSingleField(connection, sql, param);
            }
            if (type == "oracle")
            {
                //TODO
                sql = sql.Replace("@", ":");

                //UNDONE:（没有做完）……

                //HACK：（修改）……

                //hack：（修改）……

                var connection = new OracleConnection(conString);
                return OracleExt.GetSingleField(connection, sql, param);
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        ///  得到最大值
        /// </summary>
        public int GetMaxID(string fieldName, string tbName, string where)
        {
            return comRepository.GetMaxID(fieldName, tbName, where);
        }

        /// <summary>
        ///  返回自动增长列id
        /// </summary>
        public long InsertAndReturnID(string tbName, List<string> columns, object param)
        {
            return comRepository.InsertAndReturnID(tbName, columns, param);
        }

        /// <summary>
        ///  返回自动增长列id
        /// </summary>
        public void Insert(string tbName, List<string> columns, object param)
        {
            comRepository.Insert(tbName, columns, param);
        }

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        public bool IsColumnExists(string tbName, string columnName)
        {
            return comRepository.IsColumnExists(tbName, columnName);
        }

        /// <summary>
        /// 获取数据表包含字段的字符串以#分割
        /// </summary>
        public string GetTableFieldsString(string tbid)
        {
            if (!string.IsNullOrEmpty(tbid))
            {
                return comRepository.GetTableFieldsString(tbid);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        public bool IsDataTableExists(string tbName)
        {
            return comRepository.IsDataTableExists(tbName);
        }

        /// <summary>
        /// 根据录入表编码、指标编码、及类型生成数据库插入字段sql
        /// </summary>
        public string GetAddTbFieldsSql(string tbid, string indexid, string indexname, string dataType)
        {
            return comRepository.GetAddTbFieldsSql(tbid, indexid, indexname, dataType);
        }

        /// <summary>
        /// 根据录入表编码、指标编码、及类型生成数据库修改字段sql
        /// </summary>
        public string GetAlterTbFieldsSql(string tbid, string indexid, string dataType)
        {
            return comRepository.GetAlterTbFieldsSql(tbid, indexid, dataType);
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public string GetCreatDBStr(string tbid, string type, string fwId)
        {
            return comRepository.GetCreatDBSql(tbid, type, fwId);
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public string GetCreatDBStr(string tbid)
        {
            return comRepository.GetCreatDBSqlForAux(tbid);
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public int ExecuteSql(string sql, object param = null, string name = "")
        {
            try
            {
                return comRepository.ExecuteSql(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("" + name + "{" + sql + "}错误", ex);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public DataTable ExecuteSql(List<string> sqlList, object param = null)
        {
            return comRepository.ExecuteSql(sqlList, param);
        }

        /// <summary>
        /// 执行sql语句，专用于执行自定义事件
        /// </summary>
        public List<string> ExecuteAfertEvent(ref string err, List<SqlAndParamEntity> list, string listid, string _flowid, string name)
        {
            int i = 0;
            string sql = "";
            SqlAndParamEntity objCurrent = new SqlAndParamEntity();
            List<string> resultList = new List<string>();
            try
            {
                foreach (SqlAndParamEntity obj in list)
                {
                    objCurrent = obj;
                    i = 0;
                    sql = "#Sql{" + obj.Sql + "} Parm" + JsonConvert.SerializeObject(BaseUtil.RemoveObjAttributesBySql(obj.Param, obj.Sql)) + "#";
                    resultList.Add(sql);
                    //执行sql
                    if (obj.Type == "1")
                    {
                        i = comRepository.ExecuteSql(obj.Sql, obj.Param);
                    }
                    //执行存储过程
                    if (obj.Type == "2")
                    {
                        i = comRepository.ExecuteStoreProcedure(obj.Sql, obj.Param);
                    }
                    resultList.Add("#N{" + i + "}#" + sql);
                    //写入记录
                    obj.WriteTime = DateTime.Now.ToString();
                    obj.Num = i;
                    obj.strSql = "#N{" + i + "}#" + sql;
                    sqlAndParamService.Insert(obj);
                }
                return resultList;
            }
            catch (Exception ex)
            {
                err += name + "出现异常：" + ex.Message;

                objCurrent.WriteTime = DateTime.Now.ToString();
                objCurrent.strSql = sql;
                objCurrent.Err = ex.Message;
                sqlAndParamService.Insert(objCurrent);

                Record.AddErr(listid.ToString(), _flowid, name, ex);
                throw new Exception("" + name + "", ex);
            }
        }

        /// <summary>
        /// 执行sql语句，专用于执行数据回写
        /// </summary>
        public List<string> ExecuteWriteBackData(ref string err, List<WriteBackEntity> list, string listid, string _flowid, string name)
        {
            List<string> resultList = new List<string>();
            WriteBackEntity objCurrent = new WriteBackEntity();
            string sql = "";
            try
            {
                bool execute = false;
                int i = 0;
                foreach (WriteBackEntity obj in list)
                {
                    execute = true;
                    objCurrent = obj;
                    if (obj.WriteNum <= 1)
                    {
                        //设置了执行一次
                        if (writeBackService.GetTotal("WHERE relationId=@RelationId AND FromId=@FromId", obj) > 0)
                        {
                            execute = false;
                        }
                    }
                    if (execute)
                    {
                        sql = "#Sql{" + obj.Sql + "} Parm" + JsonConvert.SerializeObject(BaseUtil.RemoveObjAttributesBySql(obj.Param, obj.Sql)) + "#";
                        //执行sql
                        if (obj.Type == "1")
                        {
                            i = comRepository.ExecuteSql(obj.Sql, obj.Param);
                        }
                        //执行存储过程
                        if (obj.Type == "2")
                        {
                            i = comRepository.ExecuteStoreProcedure(obj.Sql, obj.Param);
                        }
                        resultList.Add("#N{" + i + "}#" + sql);

                        //写入记录
                        obj.WriteTime = DateTime.Now.ToString();
                        obj.Num = i;
                        obj.strSql = "#N{" + i + "}#" + sql;
                        writeBackService.Insert(obj);
                    }
                }
                return resultList;
            }
            catch (Exception ex)
            {
                err += name + "数据回写出现异常：" + ex.Message;

                objCurrent.WriteTime = DateTime.Now.ToString();
                objCurrent.strSql = sql;
                objCurrent.Err = ex.Message;
                writeBackService.Insert(objCurrent);

                Record.AddErr(listid.ToString(), _flowid, name, ex);
                throw new Exception("" + name + "", ex);
            }
        }

        /// <summary>
        /// 执行sql语句，专用于执行数据回写
        /// </summary>
        public void ExecuteWriteBackData(ref string err, string sql, object param)
        {
            try
            {
                comRepository.ExecuteSql(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("{" + sql + "}错误", ex);
            }
        }

        /// <summary>
        /// 生成自动编号数据建表语句
        /// </summary>
        public string GetCreatRuleAutoTbStr(string ruleId)
        {
            return comRepository.GenerateRuleAutoTableCreationSQL(ruleId);
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        public DataTable GetDataTable(string sql, object param = null, string name = "")
        {
            try
            {
                return comRepository.GetDataTable(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("" + name + "{" + sql + "}错误", ex);
            }
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        public DataTable GetDataTable(string conString, string type, string sql, object param = null)
        {
            try
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
            catch (Exception ex)
            {
                //throw new Exception("数据库类别{" + type + "}" + "连接字符{" + conString + "}" + "sql语句{" + sql + "} 出现错误", ex);
                throw new Exception("外部数据库 - 数据库类别{" + type + "}" + "sql语句{" + sql + "} 出现错误", ex);
            }
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        public DataTable GetDataTable(string connString, string dbType, string tbName, string some, string where, string order, object param = null)
        {
            string sql = "";
            if (string.IsNullOrEmpty(where))
            {
                sql = "SELECT  " + some + " FROM " + tbName + "  " + order;
            }
            else
            {
                string _where = where.ToLower().StartsWith("where ") ? where : "WHERE " + where;
                sql = "SELECT  " + some + " FROM " + tbName + "  " + _where + "  " + order;
            }

            if (dbType == "sqlserver")
            {
                var connection = new SqlConnection(connString);
                return SqlServerExt.GetDataTable(connection, sql, param);
            }
            if (dbType == "mysql")
            {
                var connection = new MySqlConnection(connString);
                return MySQLExt.GetDataTable(connection, sql, param);
            }
            if (dbType == "postgresql")
            {
                if (string.IsNullOrEmpty(where))
                {
                    sql = "SELECT  " + some + " FROM \"" + tbName + "\"  " + order;
                }
                else
                {
                    string _where = where.ToLower().StartsWith("where ") ? where : "WHERE " + where;
                    sql = "SELECT  " + some + " FROM \"" + tbName + "\"  " + _where + "  " + order;
                }
                var connection = new NpgsqlConnection(connString);
                return PostgreSQLExt.GetDataTable(connection, sql, param);
            }
            if (dbType == "oracle")
            {
                if (string.IsNullOrEmpty(where))
                {
                    sql = "SELECT  " + some + " FROM \"" + tbName + "\"  " + order;
                }
                else
                {
                    string _where = where.ToLower().StartsWith("where ") ? where : "WHERE " + where;
                    sql = "SELECT  " + some + " FROM \"" + tbName + "\"  " + _where + "  " + order;
                }
                var connection = new OracleConnection(connString);
                return OracleExt.GetDataTable(connection, sql, param);
            }
            throw new Exception("不支持的数据库类型");
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        public DataTable GetDataTable(string tbName, string some, string where, string order, object param = null, string name = "")
        {
            string sql = "";
            try
            {
                if (string.IsNullOrEmpty(where))
                {
                    sql = "SELECT  " + some + " FROM " + tbName + "  " + order;
                }
                else
                {
                    string _where = where.ToLower().StartsWith("where ") ? where : "WHERE " + where;
                    sql = "SELECT  " + some + " FROM " + tbName + "  " + _where + "  " + order;
                }
                return comRepository.GetDataTable(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("" + name + "{" + sql + "}错误", ex);
            }
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        public long GetTotal(string tbName, string where = null, object param = null)
        {
            try
            {
                return comRepository.GetTotal(tbName, where, param);
            }
            catch (Exception ex)
            {
                throw new Exception("{comRepository.GetTotal(" + tbName + ", " + where + ", param)}错误", ex);
            }
        }
    }
}