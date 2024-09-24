using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DapperExtensions.MySQLExt
{
    public static partial class MySQLExt
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, DapperExtSqls> dapperExtsqlsDict = new ConcurrentDictionary<RuntimeTypeHandle, DapperExtSqls>();

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
        /// 根据Key取Value值
        /// </summary>
        /// <param name="key"></param>
        public static string GetDBProvider(string key)
        {
            string filePath = Assembly.GetExecutingAssembly().Location;
            string fileDir = Path.GetDirectoryName(filePath);
            string systemConfigPath = Path.Combine(fileDir, "Configs/appSet.config");
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(systemConfigPath);
            System.Xml.XmlNode xNode = xDoc.SelectSingleNode("//appSettings");
            System.Xml.XmlElement xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
            return xElem1.GetAttribute("value");
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
        public static DataTable GetPageList(this IDbConnection conn, string files, string tableName, string where, string orderby, int pageIndex, int pageSize, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", files, tableName, where, orderby, skip, pageSize);
            string sql = sb.ToString();

            return DapperExtAllSQL.GetDataTableBase(conn, sql, param, transaction, commandTimeout);
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
            sb.AppendFormat("SELECT COUNT(1) FROM `{0}` {1};", tableName, where);
            sb.AppendFormat("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", files, tableName, where, orderby, skip, pageSize);
            string sql = sb.ToString();
            using (var reader = conn.QueryMultiple(sb.ToString(), param))
            {
                total = reader.ReadFirst<long>();
                return reader.Read<T>();
            }
        }

        /// <summary>
        /// 求值
        /// </summary>
        public static string GetSingleField(this IDbConnection conn, string sql)
        {
            object obj = conn.ExecuteScalar(sql);

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
        /// 得到最大值
        /// </summary>
        public static int GetMaxID(this IDbConnection conn, string FieldName, string TableName, string strWhere)
        {
            string sql = "select max(" + FieldName + ")+1 from " + TableName + "   " + strWhere;
            object obj = conn.ExecuteScalar(sql);
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 执行SQL语句，返回自动增长列id
        /// </summary>
        public static int InsertAndReturnID(this IDbConnection conn, string tableName, List<string> columns, object param)
        {
            string columnName = "";
            string value = "";

            foreach (string item in columns)
            {
                if (item != columns.Last())
                {
                    columnName += "`" + item + "`,";
                    value += "@" + item + ",";
                }
                else
                {
                    columnName += "`" + item + "`";
                    value += "@" + item;
                }
            }
            string sql = " insert into `" + tableName + "`  (" + columnName + ")values(" + value + ");select @@IDENTITY";
            object obj = conn.ExecuteScalar(sql, param);
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        public static void Insert(this IDbConnection conn, string tableName, List<string> columns, object param)
        {
            string columnName = "";
            string value = "";

            foreach (string item in columns)
            {
                if (item != columns.Last())
                {
                    columnName += "`" + item + "`,";
                    value += "@" + item + ",";
                }
                else
                {
                    columnName += "`" + item + "`";
                    value += "@" + item;
                }
            }
            string sql = " insert into `" + tableName + "`  (" + columnName + ")values(" + value + ")";
            object obj = conn.ExecuteScalar(sql, param);
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public static DataTable GetTbFieldsList(this IDbConnection conn, string tbID)
        {
            return GetDataTable(conn, "select COLUMN_NAME,DATA_type from information_schema.COLUMNS where table_name = '" + tbID + "' and TABLE_SCHEMA='" + GetDBProvider("DBase") + "' ORDER BY COLUMN_NAME DESC");
        }

        /// <summary>
        /// 获取数据表包含字段列表
        /// </summary>
        public static DataTable GetTbFieldsList(this IDbConnection conn, string dbName, string tbID)
        {
            return GetDataTable(conn, "select COLUMN_NAME,DATA_type from information_schema.COLUMNS where table_name = '" + tbID + "' and TABLE_SCHEMA='" + dbName + "' ORDER BY COLUMN_NAME DESC");
        }

        /// <summary>
        /// 获取已定义的所有视图
        /// </summary>
        public static DataTable GetViewsList(this IDbConnection conn)
        {
            return GetDataTable(conn, "show table status where comment='view'");
        }

        /// <summary>
        /// 获取已定义的所有录入表和视图
        /// </summary>
        public static DataTable GetTableAndViewList(this IDbConnection conn, string dbName)
        {
            return GetDataTable(conn, "SELECT table_name, table_rows FROM information_schema.tables WHERE TABLE_SCHEMA='" + dbName + "' ");
        }

        /// <summary>
        /// 根据录入表编码、指标编码、及类型生成数据库插入字段sql
        /// </summary>
        public static string GetAddTbFieldsSql(this IDbConnection conn, string TbID, string IndexId, string indexname, string DataType)
        {
            int iMlen = DapperExtCommon.GetDataMlenByDataType(DataType);
            int iDigit = DapperExtCommon.GetDataDigitByDataType(DataType);
            string comment = " comment '" + indexname + "'";

            StringBuilder strSql = new StringBuilder();
            strSql.Append("ALTER TABLE  `" + TbID + "` ADD   `" + IndexId + "` ");

            if (DataType.Substring(0, 1) == "1")
            {
                strSql.Append(" datetime NULL" + comment);
            }
            if (DataType.Substring(0, 1) == "2")
            {
                if (iMlen == 0)
                {
                    strSql.Append(" LongText  NULL" + comment);
                }
                else
                {
                    strSql.Append(" nvarchar (" + iMlen + ") NULL" + comment);
                }
            }
            if (DataType.Substring(0, 1) == "3")
            {
                if (iDigit == 0)
                {
                    strSql.Append("  bigint NULL" + comment);
                }
                else
                {
                    strSql.Append(" Decimal(22, " + iDigit + ") NULL" + comment);
                }
            }
            if (DataType.Substring(0, 1) == "4")
            {
                strSql.Append(" nvarchar (" + iMlen + ") NULL" + comment);
            }
            if (DataType.Substring(0, 1) == "5")
            {
                strSql.Append(" nvarchar (" + iMlen + ") NULL" + comment);
            }

            return strSql.ToString();
        }

        /// <summary>
        /// 根据录入表编码、指标编码、及类型生成数据库修改字段sql
        /// </summary>
        public static string GetAlterTbFieldsSql(this IDbConnection conn, string TbID, string IndexId, string DataType)
        {
            int iMlen = DapperExtCommon.GetDataMlenByDataType(DataType);
            int iDigit = DapperExtCommon.GetDataDigitByDataType(DataType);

            StringBuilder strSql = new StringBuilder();
            strSql.Append("ALTER TABLE  `" + TbID + "`  modify column   `" + IndexId + "` ");

            if (DataType.Substring(0, 1) == "1")
            {
                strSql.Append(" datetime NULL ");
            }
            if (DataType.Substring(0, 1) == "2")
            {
                if (iMlen == 0)
                {
                    strSql.Append(" LongText  NULL");
                }
                else
                {
                    strSql.Append(" nvarchar (" + iMlen + ") NULL");
                }
            }
            if (DataType.Substring(0, 1) == "3")
            {
                if (iDigit == 0)
                {
                    strSql.Append("  bigint NULL ");
                }
                else
                {
                    strSql.Append(" Decimal(22, " + iDigit + ") NULL ");
                }
            }
            if (DataType.Substring(0, 1) == "4")
            {
                strSql.Append(" nvarchar (" + iMlen + ") NULL");
            }
            if (DataType.Substring(0, 1) == "5")
            {
                strSql.Append(" nvarchar (" + iMlen + ") NULL");
            }

            return strSql.ToString();
        }

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public static bool IsColumnExists(this IDbConnection conn, string tableName, string columnName)
        {
            string sql = "select count(1) from information_schema.columns where table_name = '" + tableName + "' and column_name = '" + columnName + "'  and TABLE_SCHEMA='" + GetDBProvider("DBase") + "'";
            //select column_name from Information_schema.columns  where table_Name = 'test' and TABLE_SCHEMA='dbname'
            object res = conn.ExecuteScalar(sql);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }

        /// <summary>
        /// 获取数据表包含字段的字符串以#分割
        /// </summary>
        public static string GetTableFieldsString(this IDbConnection conn, string TbID)
        {
            string sReturn = "#";
            DataRowCollection drc = GetDataTable(conn, "select COLUMN_NAME,DATA_type  from information_schema.COLUMNS where table_name = '" + TbID + "'   and TABLE_SCHEMA='" + GetDBProvider("DBase") + "'").Rows;
            foreach (DataRow dr in drc)
            {
                sReturn += dr["COLUMN_NAME"].ToString() + "#";
            }

            return sReturn;
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static bool IsDataTableExists(this IDbConnection conn, string TableName)
        {
            string sql = "SELECT count(*) FROM information_schema.TABLES WHERE table_name ='" + TableName + "' and TABLE_SCHEMA='" + GetDBProvider("DBase") + "'";

            object obj = conn.ExecuteScalar(sql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public static string GenerateMainTableCreationSQL(this IDbConnection conn, string tbid, string flid)
        {
            string sql = "";
            string indexId = "";
            string comment = "";
            int iMlen = 0;
            int iDigit = 0;

            if (GetTotal(conn, "tbbasic", "where TbId=@TbId", new { TbId = tbid }) > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("CREATE TABLE  `" + tbid + "` ( ");
                if (flid.StartsWith("bi_"))//独立模式 自增长
                {
                    strSql.Append("`listid`  bigint(20) NOT NULL auto_increment primary key comment '主键 自增长'");
                }
                else
                {
                    strSql.Append("`listid`  bigint(20) NOT NULL primary key  comment '主键'");
                }

                DataRowCollection drc = GetDataTable(conn, "SELECT  * FROM `tbindex` WHERE `TbId`='" + tbid + "'  ORDER BY IndexOrderNo").Rows;
                foreach (DataRow dr in drc)
                {
                    indexId = dr["IndexId"].ToString();
                    iMlen = DapperExtCommon.GetDataMlenByDataType(dr["DataType"].ToString());
                    iDigit = DapperExtCommon.GetDataDigitByDataType(dr["DataType"].ToString());
                    comment = " comment '" + dr["IndexName"].ToString() + "'";

                    if (dr["DataType"].ToString().Substring(0, 1) == "1")//日期
                    {
                        strSql.Append(", `" + indexId + "` datetime NULL" + comment);
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "2")//字符型
                    {
                        //这个要注意--为0时当做text处理
                        if (iMlen == 0)
                        {
                            strSql.Append(", `" + indexId + "` LongText  NULL" + comment);
                        }
                        else
                        {
                            if (iMlen > 250)
                            {
                                strSql.Append(", `" + indexId + "` LongText  NULL" + comment);
                            }
                            else
                            {
                                strSql.Append(", `" + indexId + "` nvarchar (" + iMlen + ") NULL" + comment);
                            }
                        }
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "3")//数字
                    {
                        if (iDigit == 0)
                        {
                            strSql.Append(", `" + indexId + "`   bigint NULL" + comment);
                        }
                        else
                        {
                            strSql.Append(", `" + indexId + "`  Decimal(22, " + iDigit + ") NULL" + comment);
                        }
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "4")//附件
                    {
                        strSql.Append(", `" + indexId + "` nvarchar (" + iMlen + ") NULL" + comment);
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "5")// 图片
                    {
                        strSql.Append(", `" + indexId + "` nvarchar (" + iMlen + ") NULL" + comment);
                    }
                }

                if (flid.StartsWith("bi_"))//独立模式 自增长
                {
                    strSql.Append(", `isLock` nvarchar (1) NULL comment '锁定状态<0未锁定1已锁定9临时锁定>'");
                    strSql.Append(", `createTime` nvarchar (30) NULL comment '创建时间'");
                    strSql.Append(", `createUser` nvarchar (30) NULL comment '创建用户'");
                    strSql.Append(", `createDept` nvarchar (30) NULL comment '创建用户所在单位(部门)编码'");
                    strSql.Append(", `createDeptStr` LongText NULL comment '创建用户所有上级单位(部门)编码通过;相连的字符,用于筛选创建用户的下级单位(部门)'");
                    strSql.Append(", `runName` nvarchar (200) NULL comment '工作名称'");
                    strSql.Append(", `attachmentId` nvarchar (50) NULL comment '公共附件id'");
                }

                strSql.Append(", `state` int NULL comment '数据状态<1有效0无效-1已删除>'");

                strSql.Append(")");

                sql = strSql.ToString();
            }

            return sql;
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public static string GenerateGridTableCreationSQL(this IDbConnection conn, string tbid)
        {
            string sql = "";
            string indexId = "";
            string comment = "";
            int iMlen = 0;
            int iDigit = 0;

            if (GetTotal(conn, "tbbasic", "where TbId=@TbId", new { TbId = tbid }) > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("CREATE TABLE  `" + tbid + "` ( ");
                strSql.Append("`id`  bigint(20) NOT NULL auto_increment primary key  comment '主键 自增长',");
                strSql.Append("`listid`  bigint(20) NOT NULL  comment '与主表关联'");

                DataRowCollection drc = GetDataTable(conn, "SELECT  * FROM `tbindex` WHERE `TbId`='" + tbid + "'  ORDER BY IndexOrderNo").Rows;
                foreach (DataRow dr in drc)
                {
                    indexId = dr["IndexId"].ToString();
                    iMlen = DapperExtCommon.GetDataMlenByDataType(dr["DataType"].ToString());
                    iDigit = DapperExtCommon.GetDataDigitByDataType(dr["DataType"].ToString());
                    comment = " comment '" + dr["IndexName"].ToString() + "'";

                    if (dr["DataType"].ToString().Substring(0, 1) == "1")//日期
                    {
                        strSql.Append(", `" + indexId + "` datetime NULL" + comment);
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "2")//字符型
                    {
                        //这个要注意--为0时当做text处理
                        if (iMlen == 0)
                        {
                            strSql.Append(", `" + indexId + "` LongText  NULL" + comment);
                        }
                        else
                        {
                            if (iMlen > 250)
                            {
                                strSql.Append(", `" + indexId + "` LongText  NULL" + comment);
                            }
                            else
                            {
                                strSql.Append(", `" + indexId + "` nvarchar (" + iMlen + ") NULL" + comment);
                            }
                        }
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "3")//数字
                    {
                        if (iDigit == 0)
                        {
                            strSql.Append(", `" + indexId + "`   bigint NULL" + comment);
                        }
                        else
                        {
                            strSql.Append(", `" + indexId + "`  Decimal(22, " + iDigit + ") NULL" + comment);
                        }
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "4")// 附件
                    {
                        strSql.Append(", `" + indexId + "` nvarchar (" + iMlen + ") NULL" + comment);
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "5")// 图片
                    {
                        strSql.Append(", `" + indexId + "` nvarchar (" + iMlen + ") NULL" + comment);
                    }
                }

                strSql.Append(")");

                sql = strSql.ToString();
            }
            return sql;
        }

        /// <summary>
        /// 根据设置的指标，生成数据表语句
        /// </summary>
        public static string GenerateAuxTableCreationSQL(this IDbConnection conn, string sTbID)
        {
            string stReturn = "";
            string IndexId = "";
            int iMlen = 0;
            int iDigit = 0;

            if (GetTotal(conn, "tbbasic", "where TbId=@TbId", new { TbId = sTbID }) > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("CREATE TABLE  `" + sTbID + "` ( ");
                strSql.Append("`listid`  bigint(20) NOT NULL auto_increment primary key ");

                DataRowCollection drc = GetDataTable(conn, "SELECT  * FROM `tbindex` WHERE `TbId`='" + sTbID + "'  ORDER BY IndexOrderNo").Rows;
                foreach (DataRow dr in drc)
                {
                    IndexId = dr["IndexId"].ToString();
                    iMlen = DapperExtCommon.GetDataMlenByDataType(dr["DataType"].ToString());
                    iDigit = DapperExtCommon.GetDataDigitByDataType(dr["DataType"].ToString());

                    if (dr["DataType"].ToString().Substring(0, 1) == "1")//日期
                    {
                        strSql.Append(", `" + IndexId + "` datetime NULL ");
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "2")//字符型
                    {
                        //这个要注意--为0时当做text处理
                        if (iMlen == 0)
                        {
                            strSql.Append(", `" + IndexId + "` LongText  NULL");
                        }
                        else
                        {
                            if (iMlen > 250)
                            {
                                strSql.Append(", `" + IndexId + "` LongText  NULL");
                            }
                            else
                            {
                                strSql.Append(", `" + IndexId + "` nvarchar (" + iMlen + ") NULL");
                            }
                        }
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "3")//数字
                    {
                        if (iDigit == 0)
                        {
                            strSql.Append(", `" + IndexId + "`   bigint NULL ");
                        }
                        else
                        {
                            strSql.Append(", `" + IndexId + "`  Decimal(22, " + iDigit + ") NULL ");
                        }
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "4")// 附件
                    {
                        strSql.Append(", `" + IndexId + "` nvarchar (" + iMlen + ") NULL");
                    }
                    if (dr["DataType"].ToString().Substring(0, 1) == "5")// 图片
                    {
                        strSql.Append(", `" + IndexId + "` nvarchar (" + iMlen + ") NULL");
                    }
                }

                strSql.Append(")");

                stReturn = strSql.ToString();
            }

            return stReturn;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public static int ExecuteSql(this IDbConnection conn, string sql, object param)
        {
            return conn.Execute(sql, param);
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public static void ExecuteSql(this IDbConnection conn, List<string> sqlList, object param, DataTable dtResult)
        {
            string sql = "";
            foreach (string str in sqlList)
            {
                sql = str;
                DataRow newRow = dtResult.NewRow();
                newRow["Num"] = conn.Execute(sql, param);
                newRow["Sql"] = sql;
                newRow["Err"] = "";
                dtResult.Rows.Add(newRow);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public static List<string> ExecuteSql(this IDbConnection conn, List<string> sqlList, object param)
        {
            List<string> resultList = new List<string>();
            foreach (string sql in sqlList)
            {
                resultList.Add("#N{" + conn.Execute(sql, param) + "}#" + "#Sql{" + sql + "} Parm" + JsonConvert.SerializeObject(param) + "#");
            }

            return resultList;
        }

        /// <summary>
        /// 生成自动编号数据建表语句
        /// </summary>
        public static string GenerateRuleAutoTableCreationSQL(this IDbConnection conn, string RuleId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE `ruleauto_" + RuleId + "`( ");
            sb.Append("`id`  bigint(20) NOT NULL auto_increment primary key,");
            sb.Append("`listid` bigint(20) NOT NULL,");
            sb.Append("`itemid` bigint(20)  NOT NULL,");
            sb.Append("`tbid`  varchar (50)  NOT NULL,");
            sb.Append("`indexid` varchar (50)  NOT NULL,");
            sb.Append("`deptno` varchar (50)  NULL,");
            sb.Append("`userid` varchar (50)  NULL,");
            sb.Append("`timeno` varchar (50)  NULL,");
            sb.Append("`autono` bigint(20) NULL,");
            sb.Append("`autoid` LongText NULL) ");
            return sb.ToString();
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <returns></returns>
        public static long GetTotal(this IDbConnection conn, string tablename, string where = null, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            string sql = string.Format("SELECT COUNT(1) FROM `{0}` {1}", tablename, where);
            return conn.ExecuteScalar<long>(sql, param, transaction, commandTimeout);
        }

        //界限
        public static DapperExtSqls GetDapperExtSqls(Type t)
        {
            if (dapperExtsqlsDict.Keys.Contains(t.TypeHandle))
            {
                return dapperExtsqlsDict[t.TypeHandle];
            }
            else
            {
                DapperExtSqls sqls = DapperExtCommon.GetDapperExtSqls(t);

                string Fields = DapperExtCommon.GetFieldsStr(sqls.AllFieldList, "`", "`");
                string FieldsAt = DapperExtCommon.GetFieldsAtStr(sqls.AllFieldList);
                string FieldsEq = DapperExtCommon.GetFieldsEqStr(sqls.AllFieldList, "`", "`");

                string FieldsExtKey = DapperExtCommon.GetFieldsStr(sqls.ExceptKeyFieldList, "`", "`");
                string FieldsAtExtKey = DapperExtCommon.GetFieldsAtStr(sqls.ExceptKeyFieldList);
                string FieldsEqExtKey = DapperExtCommon.GetFieldsEqStr(sqls.ExceptKeyFieldList, "`", "`");

                sqls.AllFields = Fields;

                if (sqls.HasKey && sqls.IsIdentity) //有主键并且是自增
                {
                    sqls.InsertSql = string.Format("INSERT INTO `{0}`({1})VALUES({2})", sqls.TableName, FieldsExtKey, FieldsAtExtKey);
                    sqls.InsertIdentitySql = string.Format("INSERT INTO `{0}`({1})VALUES({2})", sqls.TableName, Fields, FieldsAt);
                }
                else
                {
                    sqls.InsertSql = string.Format("INSERT INTO `{0}`({1})VALUES({2})", sqls.TableName, Fields, FieldsAt);
                }

                if (sqls.HasKey) //含有主键
                {
                    sqls.DeleteByIdSql = string.Format("DELETE FROM `{0}` WHERE `{1}`=@id", sqls.TableName, sqls.KeyName);
                    sqls.DeleteByIdsSql = string.Format("DELETE FROM `{0}` WHERE `{1}` IN @ids", sqls.TableName, sqls.KeyName);
                    sqls.GetByIdSql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}`=@id", Fields, sqls.TableName, sqls.KeyName);
                    sqls.GetByIdsSql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", Fields, sqls.TableName, sqls.KeyName);
                    sqls.UpdateByIdSql = string.Format("UPDATE `{0}` SET {1} WHERE `{2}`=@{2}", sqls.TableName, FieldsEqExtKey, sqls.KeyName);
                }
                sqls.DeleteAllSql = string.Format("DELETE FROM `{0}`", sqls.TableName);
                sqls.GetAllSql = string.Format("SELECT {0} FROM `{1}`", Fields, sqls.TableName);

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
            try
            {
                if (sqls.HasKey && sqls.IsIdentity)
                {
                    switch (sqls.KeyType)
                    {
                        case "Int32":
                            return conn.ExecuteScalar<int>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //int
                        case "Int64":
                            return conn.ExecuteScalar<long>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //long
                        case "Decimal":
                            return conn.ExecuteScalar<decimal>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //decimal
                        case "UInt32":
                            return conn.ExecuteScalar<uint>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //uint
                        case "UInt64":
                            return conn.ExecuteScalar<ulong>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //ulong
                        case "Double":
                            return conn.ExecuteScalar<double>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //double
                        case "Single":
                            return conn.ExecuteScalar<float>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //float
                        case "Byte":
                            return conn.ExecuteScalar<byte>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout);  //byte
                        case "SByte":
                            return conn.ExecuteScalar<sbyte>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //sbyte
                        case "Int16":
                            return conn.ExecuteScalar<short>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //short
                        case "UInt16":
                            return conn.ExecuteScalar<ushort>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //ushort
                        default:
                            return conn.ExecuteScalar<dynamic>(sqls.InsertSql + ";SELECT @@IDENTITY", entity, transaction, commandTimeout); //dynamic
                    }
                }
                else
                {
                    return conn.Execute(sqls.InsertSql, entity, transaction, commandTimeout);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("{" + sqls.InsertSql + "}错误", ex);
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
                //";select @@IDENTITY" SELECT CAST(SCOPE_IDENTITY() as int)
                //string str = " SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER) ";
                int i = conn.Execute(sqls.InsertIdentitySql, entity, transaction, commandTimeout);
                return i;
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有自增键，无法进行InsertIdentity。");
            }
        }

        /// <summary>
        /// 新增多条数据(插入自增键)
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
                    string sql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}`=@id", returnFields, sqls.TableName, sqls.KeyName);
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
                    string sql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}`=@id", returnFields, sqls.TableName, sqls.KeyName);
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
        private static IEnumerable<T> GetByIdsBase<T>(this IDbConnection conn, Type t, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (DapperExtCommon.ObjectIsEmpty(ids))
                return new List<T>();

            DapperExtSqls sqls = GetDapperExtSqls(t);
            if (sqls.HasKey)
            {
                DynamicParameters dpar = new DynamicParameters();
                dpar.Add("@ids", ids);
                if (returnFields == null)
                {
                    return conn.Query<T>(sqls.GetByIdsSql, dpar, transaction, true, commandTimeout);
                }
                else
                {
                    string sql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", returnFields, sqls.TableName, sqls.KeyName);
                    return conn.Query<T>(sql, dpar, transaction, true, commandTimeout);
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
        public static IEnumerable<dynamic> GetByIdsDynamic<T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (DapperExtCommon.ObjectIsEmpty(ids))
                return new List<dynamic>();

            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                DynamicParameters dpar = new DynamicParameters();
                dpar.Add("@ids", ids);
                if (returnFields == null)
                {
                    return conn.Query(sqls.GetByIdsSql, dpar, transaction, true, commandTimeout);
                }
                else
                {
                    string sql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", returnFields, sqls.TableName, sqls.KeyName);
                    return conn.Query(sql, dpar, transaction, true, commandTimeout);
                }
            }
            else
            {
                throw new ArgumentException("表" + sqls.TableName + "没有主键，无法GetByIdsDynamic。");
            }
        }

        /// <summary>
        /// 根据主键ids返回任意类型实体列表
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<T> GetByIds<T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByIdsBase<T>(conn, typeof(T), ids, returnFields, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据主键ids返回任意类型实体列表
        /// returnFields需要返回的列，用逗号隔开。默认null，返回所有列
        /// </summary>
        public static IEnumerable<T> GetByIds<Table, T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
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
                string sql = string.Format("SELECT {0} FROM `{1}` " + orderby, returnFields, sqls.TableName);
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
                string sql = string.Format("SELECT {0} FROM `{1}` " + orderby, returnFields, sqls.TableName);
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
        public static int DeleteByIds<T>(this IDbConnection conn, object ids, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (DapperExtCommon.ObjectIsEmpty(ids))
                return 0;

            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            if (sqls.HasKey)
            {
                DynamicParameters dpar = new DynamicParameters();
                dpar.Add("@ids", ids);
                return conn.Execute(sqls.DeleteByIdsSql, dpar, transaction, commandTimeout);
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
            string sql = string.Format("DELETE FROM `{0}` {1}", sqls.TableName, where);
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
                    string updateList = DapperExtCommon.GetFieldsEqStr(updateFields.Split(',').ToList(), "`", "`");
                    string sql = string.Format("UPDATE `{0}` SET {1} WHERE `{2}`=@{2}", sqls.TableName, updateList, sqls.KeyName);
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
                    string updateList = DapperExtCommon.GetFieldsEqStr(updateFields.Split(',').ToList(), "`", "`");
                    string sql = string.Format("UPDATE `{0}` SET {1} WHERE `{2}`=@{2}", sqls.TableName, updateList, sqls.KeyName);
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
            updateFields = DapperExtCommon.GetFieldsEqStr(updateFields.Split(',').ToList(), "`", "`");
            string sql = string.Format("UPDATE `{0}` SET {1} {2}", sqls.TableName, updateFields, where);
            return conn.Execute(sql, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <returns></returns>
        public static long GetTotal<T>(this IDbConnection conn, string where = null, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            string sql = string.Format("SELECT COUNT(1) FROM `{0}` {1}", sqls.TableName, where);
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
                    orderBy = string.Format("ORDER BY `{0}` DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY `{0}`", sqls.AllFieldList.First());
                }
            }

            string sql = string.Format("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", returnFields, sqls.TableName, where, orderBy, skip, take);

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
                    orderBy = string.Format("ORDER BY `{0}` DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY `{0}`", sqls.AllFieldList.First());
                }
            }

            string sql = string.Format("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", returnFields, sqls.TableName, where, orderBy, skip, take);
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
            string sql = string.Format("SELECT {0} FROM `{1}` {2} {3}", returnFields, sqls.TableName, where, orderBy);

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
            string sql = string.Format("SELECT {0} FROM `{1}` {2} {3}", returnFields, sqls.TableName, where, orderBy);

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

        ///// <summary>
        ///// 根据查询条件获取数据
        ///// </summary>
        //public static IEnumerable<T> GetBySql<Table, T>(this IDbConnection conn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        //{
        //    return GetBySqlBase<T>(conn, typeof(Table), sql, param, transaction, commandTimeout);
        //}

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

            string sql = string.Format("SELECT {0} FROM `{1}` LIMIT 0", returnFields, sqls.TableName);
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
                    orderBy = string.Format("ORDER BY `{0}` DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY `{0}`", sqls.AllFieldList.First());
                }
            }

            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM `{0}` {1};", sqls.TableName, where);
            sb.AppendFormat("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", returnFields, sqls.TableName, where, orderBy, skip, pageSize);
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
                    orderBy = string.Format("ORDER BY `{0}` DESC", sqls.KeyName);
                }
                else
                {
                    orderBy = string.Format("ORDER BY `{0}`", sqls.AllFieldList.First());
                }
            }

            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("SELECT COUNT(1) FROM `{0}` {1};", sqls.TableName, where);
            sb.AppendFormat("SELECT {0} FROM `{1}` {2} {3} LIMIT {4},{5}", returnFields, sqls.TableName, where, orderBy, skip, pageSize);
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
            string sql = string.Format("SELECT {0} FROM `{1}` {2} {3}", returnFields, sqls.TableName, where, orderBy);

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
            string sql = string.Format("SELECT {0} FROM `{1}` {2} {3}", returnFields, sqls.TableName, where, orderBy);

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

        private static IEnumerable<T> GetByInBase<T>(this IDbConnection conn, Type t, string field, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (DapperExtCommon.ObjectIsEmpty(ids))
                return new List<T>();

            DapperExtSqls sqls = GetDapperExtSqls(t);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", returnFields, sqls.TableName, field);
            return conn.Query<T>(sql, dpar, transaction, true, commandTimeout);
        }

        public static IEnumerable<dynamic> GetByInDynamic<T>(this IDbConnection conn, string field, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (DapperExtCommon.ObjectIsEmpty(ids))
                return new List<dynamic>();

            DapperExtSqls sqls = GetDapperExtSqls(typeof(T));
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            if (returnFields == null)
                returnFields = sqls.AllFields;
            string sql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", returnFields, sqls.TableName, field);
            return conn.Query(sql, dpar, transaction, true, commandTimeout);
        }

        public static IEnumerable<T> GetByIn<T>(this IDbConnection conn, string field, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByInBase<T>(conn, typeof(T), field, ids, returnFields, transaction, commandTimeout);
        }

        public static IEnumerable<T> GetByIn<Table, T>(this IDbConnection conn, string field, object ids, string returnFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return GetByInBase<T>(conn, typeof(Table), field, ids, returnFields, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取分页数据 联合查询
        /// </summary>
        public static IEnumerable<T> GetByPageUnite<T>(this IDbConnection conn, int pageIndex, int pageSize, out long total, string returnFields = null, string where = null, object param = null, string orderBy = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int skip = 0;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM {0};", where);
            sb.AppendFormat("SELECT {0} FROM {1} {2}  LIMIT {3},{4}", returnFields, where, orderBy, skip, pageSize);
            using (var reader = conn.QueryMultiple(sb.ToString(), param, transaction, commandTimeout))
            {
                total = reader.ReadFirst<long>();
                return reader.Read<T>();
            }
        }
    }
}