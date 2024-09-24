using System;
using System.Data;
using Think9.CreatCode;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public partial class AppSet
    {
        public static ReportDtModel GetReportDtModel(string _dbtype)
        {
            string order = "";
            string sql = "select * from tbindex order by TbId,IndexOrderNo";
            ComService comService = new ComService();

            ReportDtModel model = new ReportDtModel();

            sql = "select * from tbindex order by TbId,IndexOrderNo";
            DataTable _dtindex = comService.GetDataTable(sql);
            model.dtIndex = _dtindex;

            sql = "select * from tbbasic ";
            DataTable allTB = comService.GetDataTable(sql);
            model.allTB = allTB;

            sql = "select * from rulelist";
            model.dtRuleList = comService.GetDataTable(sql);

            sql = "select * from rulesingle";
            DataTable dtrulesingle = comService.GetDataTable(sql);
            model.dtRuleSingle = dtrulesingle;

            sql = "select * from rulemultiple";
            DataTable dtrulemultiple = comService.GetDataTable(sql);
            model.dtRuleMultiple = dtrulemultiple;

            sql = "select * from rulemultiplefiled order by RuleId,FiledOrder";
            DataTable dtrulemultiplefiled = comService.GetDataTable(sql);
            model.dtRuleMultipleFiled = dtrulemultiplefiled;

            sql = "select * from ruletree";
            DataTable dtruletree = comService.GetDataTable(sql);
            model.dtRuleTree = dtruletree;

            order = "ORDER BY LENGTH(ParmId) DESC";
            if (_dbtype != "mysql")
            {
                order = "ORDER BY LEN(ParmId) DESC";
            }
            sql = "select * from IndexParm ";
            model.IndexParm = comService.GetDataTable(sql + order);

            sql = "select * from reportparm  ";
            model.ReportParm = comService.GetDataTable(sql);

            order = "ORDER BY LENGTH(IndexId) DESC";
            if (_dbtype != "mysql")
            {
                order = "ORDER BY LEN(IndexId) DESC";
            }
            sql = "select * from indexstats ";
            model.IndexStats = comService.GetDataTable(sql + order);

            sql = "select * from reportbasic ";
            model.ReportBasic = comService.GetDataTable(sql);

            sql = "select * from reportcols  order by ReportId,ColNum ";
            model.ReportCols = comService.GetDataTable(sql);

            sql = "select * from reportrows order by ReportId,OrderNo ";
            model.ReportRows = comService.GetDataTable(sql);

            sql = "select * from reportdynparm";
            model.ReportDYNParm = comService.GetDataTable(sql);

            sql = "select * from reportparmquery order by ReportId,OrderNo ";
            model.ReportParmQuery = comService.GetDataTable(sql);

            sql = "select * from reportparmqueryselect";
            model.ReportParmQuerySelect = comService.GetDataTable(sql);

            //sql = @"SELECT ReportParm.ReportId, ReportParm.ColNum,ReportParm.RowId,ReportParm.ParmId, ReportParm.ParmValue,ReportRows.Type,ReportRows.OrderNo, ReportRows.DynamicId,ReportRows.I1, ReportRows.I2, ReportRows.I3, ReportRows.I4, ReportRows.I5, ReportRows.I6,ReportRows.I7, ReportRows.I8, ReportRows.I9, ReportRows.I10, ReportRows.I11,ReportRows.I12, ReportRows.I13, ReportRows.I14, ReportRows.I15, ReportRows.I16,ReportRows.I17, ReportRows.I18, ReportRows.I19, ReportRows.I20, ReportRows.I21,ReportRows.I22, ReportRows.I23, ReportRows.I24, ReportRows.I25, ReportRows.I26,ReportRows.I27, ReportRows.I28, ReportRows.I29, ReportRows.I30, ReportRows.I31,ReportRows.I32, ReportRows.I33, ReportRows.I34, ReportRows.I35, ReportRows.I36,ReportRows.I37, ReportRows.I38, ReportRows.I39, ReportRows.I40, ReportRows.I41,ReportRows.I42, ReportRows.I43, ReportRows.I44, ReportRows.I45, ReportRows.I46,ReportRows.I47, ReportRows.I48, ReportRows.I49, ReportRows.I50  FROM ReportRows INNER JOIN ReportParm ON ReportRows.id = ReportParm.RowId order by ReportRows.ReportId,ReportRows.OrderNo ";
            sql = @"SELECT reportparm.ReportId, reportparm.ColNum,reportparm.RowId,reportparm.ParmId, reportparm.ParmValue,reportrows.Type,reportrows.OrderNo, reportrows.DynamicId,reportrows.I1, reportrows.I2, reportrows.I3, reportrows.I4, reportrows.I5, reportrows.I6,reportrows.I7, reportrows.I8, reportrows.I9, reportrows.I10, reportrows.I11,reportrows.I12, reportrows.I13, reportrows.I14, reportrows.I15, reportrows.I16,reportrows.I17, reportrows.I18, reportrows.I19, reportrows.I20, reportrows.I21,reportrows.I22, reportrows.I23, reportrows.I24, reportrows.I25, reportrows.I26,reportrows.I27, reportrows.I28, reportrows.I29, reportrows.I30, reportrows.I31,reportrows.I32, reportrows.I33, reportrows.I34, reportrows.I35, reportrows.I36,reportrows.I37, reportrows.I38, reportrows.I39, reportrows.I40, reportrows.I41,reportrows.I42, reportrows.I43, reportrows.I44, reportrows.I45, reportrows.I46,reportrows.I47, reportrows.I48, reportrows.I49, reportrows.I50  FROM reportrows INNER JOIN reportparm ON reportrows.id = reportparm.RowId order by reportrows.ReportId,reportrows.OrderNo ";
            model.ReportParm_View = comService.GetDataTable(sql);

            sql = "select * from reportindexcol  order by ReportId,OrderNo";
            model.ReportIndexCol = comService.GetDataTable(sql);

            sql = "select * from reportindexcolparm";
            model.ReportIndexColParm = comService.GetDataTable(sql);

            sql = "select * from reportindexsearch order by ReportId,OrderNo";
            model.ReportIndexSearch = comService.GetDataTable(sql);

            sql = "select * from reportindexorder order by ReportId,OrderNo";
            model.ReportIndexOrder = comService.GetDataTable(sql);

            model.allView = comService.GetViewsList();

            DataTable dt = DataTableHelp.NewViewFieldDt();
            foreach (DataRow drView in model.allView.Rows)
            {
                foreach (DataRow dr in comService.GetTbFieldsList(drView["Name"].ToString()).Rows)
                {
                    DataRow row = dt.NewRow();
                    row["View_Name"] = drView["Name"].ToString();
                    row["COLUMN_NAME"] = dr["COLUMN_NAME"].ToString();
                    row["DATA_type"] = dr["DATA_type"].ToString();
                    dt.Rows.Add(row);
                }
            }
            model.ViewField = dt;

            sql = @"select * from externaldb";
            DataTable dtExternalDb = comService.GetDataTable(sql);
            model.dtExternalDb = dtExternalDb;

            string dbId = "";
            string dbType = "";
            string dbName = "";
            string dbCon = "";
            string tbid = "";
            ExtraDbService extraDb = new ExtraDbService();
            DataTable dtExtraDbTbFields = DataTableHelp.NewTableFieldDt();
            foreach (DataRow drDb in dtExternalDb.Rows)
            {
                dbId = drDb["DbID"].ToString();
                dbType = drDb["DbType"].ToString();
                dbName = drDb["DbName"].ToString();
                dbCon = drDb["DbCon"].ToString();

                if (comService.GetTotal("reportbasic", "where DbID=" + dbId + "") > 0)
                {
                    //取视图和数据表
                    DataTable dbTableAndView = extraDb.GetTableAndViewList(dbCon, dbType, dbName);//
                    foreach (DataRow drTable in dbTableAndView.Rows)
                    {
                        tbid = drTable["table_name"].ToString();
                        if (comService.GetTotal("reportbasic", "where DbID=" + dbId + " and TbId='" + tbid + "' ") > 0)
                        {
                            DataTable tbTbFieldList = extraDb.GetTbFieldsList(dbCon, dbType, dbName, tbid);//
                            foreach (DataRow drField in tbTbFieldList.Rows)
                            {
                                DataRow row = dtExtraDbTbFields.NewRow();
                                row["DbId"] = dbId;
                                row["DbType"] = dbType;
                                row["Table_Name"] = tbid;
                                row["COLUMN_NAME"] = drField["COLUMN_NAME"].ToString();
                                row["DATA_type"] = drField["DATA_type"].ToString();
                                dtExtraDbTbFields.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            model.dtExtraDbTbFields = dtExtraDbTbFields;

            return model;
        }

        public static string CheckConnection()
        {
            string dbId = "";
            string dbType = "";
            string dbName = "";
            string dbCon = "";
            string err = "";
            ComService comService = new ComService();
            Think9.Services.Basic.ExternalDbService dbService = new Think9.Services.Basic.ExternalDbService();

            foreach (DataRow drDb in comService.GetDataTable(@"select * from externaldb").Rows)
            {
                dbId = drDb["DbID"].ToString();
                dbType = drDb["DbType"].ToString();
                dbName = drDb["DbName"].ToString();
                dbCon = drDb["DbCon"].ToString();

                if (comService.GetTotal("reportbasic", "where DbID=" + dbId + "") > 0)
                {
                    try
                    {
                        var connection = dbService.GetConnection(dbCon, dbType);
                        connection.Open();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        //err += "外部数据源 - " + dbName + "无法连接";
                    }
                }
            }

            return err;
        }
    }
}