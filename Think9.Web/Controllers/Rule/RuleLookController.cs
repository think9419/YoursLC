using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class RuleLookController : BaseController
    {
        private ComService comService = new ComService();
        private ExtraDbService extraDb = new ExtraDbService();
        private RuleAutoService ruleAutoService = new RuleAutoService();
        private RuleListService ruleListService = new RuleListService();
        private string _dbtype = HelpCode.GetDBProvider("DBProvider");//数据库类型

        public ActionResult RuleDataListByTbIndexID(string indexid, string tbid)
        {
            string id = comService.GetSingleField("select RuleId from tbindex where TbId='" + tbid + "' and IndexId='" + indexid + "'");
            ViewBag.Id = id;

            string type = "";
            DataTable dt = comService.GetDataTable("select * from rulelist where RuleId='" + id + "'");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["RuleType"].ToString() == "6")
                {
                    type = "RuleSingleDataList";
                }
                if (dt.Rows[0]["RuleType"].ToString() == "7")
                {
                    type = "RuleMultipleDataList";
                }
                if (dt.Rows[0]["RuleType"].ToString() == "8")
                {
                    type = "RuleTreeDataList";
                }
            }

            if (string.IsNullOrEmpty(type))
            {
                return Json("只能预览单列选择、多列选择、树形选择类型的数据");
            }
            else
            {
                return View(type);
            }
        }

        public ActionResult RuleDataList(string id)
        {
            ViewBag.Id = id;

            string type = "";
            DataTable dt = comService.GetDataTable("select * from rulelist where RuleId='" + id + "'");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["RuleType"].ToString() == "6")
                {
                    type = "RuleSingleDataList";
                }
                if (dt.Rows[0]["RuleType"].ToString() == "7")
                {
                    type = "RuleMultipleDataList";
                }
                if (dt.Rows[0]["RuleType"].ToString() == "8")
                {
                    type = "RuleTreeDataList";
                }
            }
            else
            {
                return Json("数据不存在");
            }

            if (string.IsNullOrEmpty(type))
            {
                return Json("只能预览单列选择、多列选择、树形选择类型的数据");
            }
            else
            {
                return View(type);
            }
        }

        //备注20240912：不要试图将此函数的逻辑转移到Service层，如转移，需要Service层也得引用Think9.CodeBuild
        public ActionResult GetRuleSingleDataList(string id)
        {
            string sql = "";
            DataTable dtIndex = comService.GetDataTable("select * from tbindex order by TbId,IndexOrderNo");
            DataTable allTB = comService.GetDataTable("select * from tbbasic ");
            DataTable dtRuleSingle = comService.GetDataTable("select * from rulesingle");
            DataTable dtRuleMultiple = comService.GetDataTable("select * from rulemultiple");
            DataTable dtRuleMultipleFiled = comService.GetDataTable("select * from rulemultiplefiled order by RuleId,FiledOrder");
            DataTable dtRuleTree = comService.GetDataTable("select * from ruletree");
            DataTable dtExternalDb = comService.GetDataTable(@"select * from externaldb");

            TableRuleModel model = new TableRuleModel();
            model.DbID = "0";
            model.DataType = "2";
            model.RuleId = id;
            model.RuleType = "6";

            object param = BasicHelp.GetParamObject(CurrentUser);
            CreatTableCom.GetRuleModel(ref model, allTB, dtIndex, dtRuleSingle, dtRuleMultiple, dtRuleMultipleFiled, dtRuleTree, dtExternalDb, _dbtype);

            try
            {
                DataTable dt = new DataTable();

                if (model.DbID == "0")
                {
                    if (_dbtype == "mysql")
                    {
                        sql = model.SqlListTop.Trim() + " limit 100";
                    }
                    else
                    {
                        sql = "select top 100 " + model.SqlListTop.Trim().Substring(6, model.SqlListTop.Trim().Length - 6);
                    }
                    dt = comService.GetDataTable(sql, param);
                }
                else
                {
                    string dbType = "";
                    string dbCon = "";
                    string dbName = "";
                    DataTable dtDb = comService.GetDataTable("select * from externaldb ");
                    foreach (DataRow dr in dtDb.Rows)
                    {
                        if (dr["DbID"].ToString() == model.DbID)
                        {
                            dbType = dr["DbType"].ToString();
                            dbCon = dr["DbCon"].ToString();
                            dbName = dr["DbName"].ToString();
                            break;
                        }
                    }

                    if (dbType != "")
                    {
                        if (dbType == "sqlserver")
                        {
                            //sql = Regex.Replace(model.SqlListTop.Trim(), "select top 100 ", "select ", RegexOptions.IgnoreCase);
                            sql = "select top 100 " + model.SqlListTop.Trim().Substring(6, model.SqlListTop.Trim().Length - 6);
                        }
                        if (dbType == "mysql")
                        {
                            sql = model.SqlListTop.Trim() + " LIMIT 100";
                        }
                        if (dbType == "postgresql")
                        {
                            sql = model.SqlListTop_PG + " LIMIT 100";
                        }
                        if (dbType == "oracle")
                        {
                            sql = "SELECT * from (" + model.SqlListTop_ORACLE + ") where rownum <= 100";
                        }
                        dt = extraDb.GetDataTable(dbCon, dbType, sql, param);
                    }
                }
                IEnumerable<InfoListEntity> _list = DataTableHelp.ToEnumerable<InfoListEntity>(dt);
                var result = new { code = 0, msg = "", count = 99999, data = _list };
                return Json(result);
            }
            catch (Exception ex)
            {
                List<InfoListEntity> _list = new List<InfoListEntity>();
                _list.Add(new InfoListEntity { info1 = sql, info2 = ex.Message });
                var result = new { msg = sql + ex.Message, count = -1 };
                return Json(result);
            }
        }

        //备注20240912：不要试图将此函数的逻辑转移到Service层，如转移，需要Service层也得引用Think9.CodeBuild
        public ActionResult GetRuleTreeDataList(string id)
        {
            DataTable dt = DataTableHelp.NewValueTextDt();

            DataTable dtIndex = comService.GetDataTable("select * from tbindex order by TbId,IndexOrderNo");
            DataTable allTB = comService.GetDataTable("select * from tbbasic ");
            DataTable dtrulesingle = comService.GetDataTable("select * from rulesingle");
            DataTable dtrulemultiple = comService.GetDataTable("select * from rulemultiple");
            DataTable dtrulemultiplefiled = comService.GetDataTable("select * from rulemultiplefiled order by RuleId,FiledOrder");
            DataTable dtruletree = comService.GetDataTable("select * from ruletree");
            DataTable dtExternalDb = comService.GetDataTable(@"select * from externaldb");

            TableRuleModel model = new TableRuleModel();
            model.DbID = "0";
            model.DataType = "2";
            model.RuleId = id;
            model.RuleType = "8";
            string sql;

            object param = BasicHelp.GetParamObject(CurrentUser);
            CreatTableCom.GetRuleModel(ref model, allTB, dtIndex, dtrulesingle, dtrulemultiple, dtrulemultiplefiled, dtruletree, dtExternalDb, _dbtype);

            try
            {
                if (_dbtype == "mysql")
                {
                    sql = model.SqlListTop.Trim() + " LIMIT 100";
                }
                else
                {
                    sql = "select top 100 " + model.SqlListTop.Trim().Substring(6, model.SqlListTop.Trim().Length - 6);
                }
                BaseUtil.GetTreeDt(dt, comService.GetDataTable(sql, param), "");
                IEnumerable<valueTextEntity> _list = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
                var result = new { code = 0, msg = "", count = 99999, data = _list };
                return Json(result);
            }
            catch (Exception ex)
            {
                List<valueTextEntity> _list = new List<valueTextEntity>();
                _list.Add(new valueTextEntity { Value = model.SqlListTop, Text = ex.Message });
                var result = new { msg = model.SqlListTop + ex.Message, count = -1 };
                return Json(result);
            }
        }

        //备注20240912：不要试图将此函数的逻辑转移到Service层，如转移，需要Service层也得引用Think9.CodeBuild
        public ActionResult GetRuleMultipleDataList(string id)
        {
            string sql = "";
            DataTable dtIndex = comService.GetDataTable("select * from tbindex order by TbId,IndexOrderNo");
            DataTable allTB = comService.GetDataTable("select * from tbbasic ");
            DataTable dtrulesingle = comService.GetDataTable("select * from rulesingle");
            DataTable dtrulemultiple = comService.GetDataTable("select * from rulemultiple");
            DataTable dtrulemultiplefiled = comService.GetDataTable("select * from rulemultiplefiled order by RuleId,FiledOrder");
            DataTable dtruletree = comService.GetDataTable("select * from ruletree");
            DataTable dtExternalDb = comService.GetDataTable(@"select * from externaldb");

            TableRuleModel model = new TableRuleModel();
            model.DbID = "0";
            model.DataType = "2";
            model.RuleId = id;
            model.RuleType = "7";

            object param = BasicHelp.GetParamObject(CurrentUser);
            CreatTableCom.GetRuleModel(ref model, allTB, dtIndex, dtrulesingle, dtrulemultiple, dtrulemultiplefiled, dtruletree, dtExternalDb, _dbtype);

            try
            {
                DataTable dt = new DataTable();

                if (model.DbID == "0")
                {
                    if (_dbtype == "mysql")
                    {
                        sql = model.SqlListTop.Trim() + " LIMIT 100";
                    }
                    else
                    {
                        sql = "select top 100 " + model.SqlListTop.Trim().Substring(6, model.SqlListTop.Trim().Length - 6);
                    }
                    dt = comService.GetDataTable(sql, param);
                }
                else
                {
                    string dbType = "";
                    string dbCon = "";
                    string dbName = "";
                    DataTable dtDb = comService.GetDataTable("select * from externaldb ");
                    foreach (DataRow dr in dtDb.Rows)
                    {
                        if (dr["DbID"].ToString() == model.DbID)
                        {
                            dbType = dr["DbType"].ToString();
                            dbCon = dr["DbCon"].ToString();
                            dbName = dr["DbName"].ToString();
                            break;
                        }
                    }

                    if (dbType != "")
                    {
                        if (dbType == "sqlserver")
                        {
                            sql = "select top 100 " + model.SqlListTop.Trim().Substring(6, model.SqlListTop.Trim().Length - 6);
                        }
                        if (dbType == "mysql")
                        {
                            sql = model.SqlListTop.Trim() + " LIMIT 100";
                        }
                        if (dbType == "postgresql")
                        {
                            sql = model.SqlListTop_PG + " LIMIT 100";
                        }
                        if (dbType == "oracle")
                        {
                            sql = "SELECT * from (" + model.SqlListTop_ORACLE + ") where rownum <= 100";
                        }
                        dt = extraDb.GetDataTable(dbCon, dbType, sql, param);
                    }
                }
                IEnumerable<InfoListEntity> _list = DataTableHelp.ToEnumerable<InfoListEntity>(dt);
                var result = new { code = 0, msg = "", count = 99999, data = _list };
                return Json(result);
            }
            catch (Exception ex)
            {
                List<InfoListEntity> _list = new List<InfoListEntity>();
                _list.Add(new InfoListEntity { Value = sql, info1 = ex.Message });
                var result = new { msg = sql + ex.Message, count = -1 };
                return Json(result);
            }
        }
    }
}