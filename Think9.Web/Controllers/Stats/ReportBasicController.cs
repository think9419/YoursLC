using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Stats;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class ReportBasicController : BaseController
    {
        private ComService comService = new ComService();
        private SortService sortService = new SortService();
        private ReportService reportService = new ReportService();
        private ReportIndexColService reportIndex = new ReportIndexColService();
        private RuleServiceBasic ruleService = new RuleServiceBasic();

        private ReportIndexSearchService reportIndexSearch = new ReportIndexSearchService();//ReportIndexOrderService
        private ReportIndexOrderService reportIndexOrder = new ReportIndexOrderService();
        private TbFieldService tbFieldService = new TbFieldService();

        /// <summary>
        /// 数据源选择
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSelectValueFieldByDB(string id)
        {
            try
            {
                return Json(ruleService.GetSelectValueFieldListByDbId(id == null ? "0" : id));
            }
            catch (Exception ex)
            {
                List<valueTextEntity> list = new List<valueTextEntity>();
                list.Add(new valueTextEntity { ClassID = "err", Value = "", Text = ex.Message });
                return Json(list);
            }
        }

        /// <summary>
        /// 分类
        /// </summary>
        private SelectList SortList
        { get { return new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_report"), "SortID", "SortName"); } }

        [HttpGet]
        public ActionResult AddReportBasic(string frm)
        {
            TbBasicService tbServer = new TbBasicService();
            ExternalDbService externalDbService = new ExternalDbService();

            ViewBag.Guid = "temp_" + Think9.Services.Basic.CreatCode.NewGuid();
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.SortList = SortList;
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(tbServer.GetMainAndGridTb(""));
            ViewBag.ViewList = tbFieldService.GetViewsList();
            ViewBag.FontStyle = FontStyleList.GetList();
            ViewBag.DbList = DataTableHelp.ToEnumerable<valueTextEntity>(externalDbService.GetList());

            return View();
        }

        [HttpPost]
        public ActionResult AddReportBasic(ReportBasicEntity entity, string tbid, string guid)
        {
            string err = "";
            string str = "#login#home#com#";

            if (entity.ReportId.ToLower().StartsWith("sys"))
            {
                err = "编码不能sys开头";
                return Json(ErrorTip(err));
            }

            if (str.ToLower().Contains("#" + entity.ReportId.ToLower() + "#"))
            {
                err = "编码不能为以下关键字：" + str.Replace("#", " ");
                return Json(ErrorTip(err));
            }

            try
            {
                err = reportService.AddReportBasic(entity, tbid, guid);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult EditReportBasic(string rpId)
        {
            TbBasicService tbServer = new TbBasicService();

            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.SortList = SortList;
            ViewBag.FontStyle = FontStyleList.GetList();
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(tbServer.GetMainAndGridTb(""));
            ViewBag.ViewList = tbFieldService.GetViewsList();
            var model = reportService.GetReportModel(rpId);
            if (model != null)
            {
                model.DbID_Exa = ExternalDbService.GetName(model.DbID);
                return View(model);
            }
            else
            {
                var result = ErrorTip("数据不存在");
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult EditReportBasic(ReportBasicEntity entity, IEnumerable<ReportColsEntity> list)
        {
            try
            {
                string err = reportService.EditReport(entity, list);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        #region 参数赋值

        [HttpGet]
        public ActionResult IndexColParmList(string rpId, string colId, string tbid, string dbid)
        {
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.colId = string.IsNullOrEmpty(colId) ? "0" : colId;
            ViewBag.dbId = string.IsNullOrEmpty(dbid) ? "0" : dbid;
            return View();
        }

        [HttpGet]
        public ActionResult AddIndexColParm(string rpId, string colId, string tbid, string dbid)
        {
            IndexStatsService indexStats = new IndexStatsService();
            IndexParmService indexParmService = new IndexParmService();

            string _tbid = GetTbID.GetTbIdByIdStr(tbid);
            ViewBag.tbid = _tbid;
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.colId = string.IsNullOrEmpty(colId) ? "0" : colId;
            ViewBag.dbId = string.IsNullOrEmpty(dbid) ? "0" : dbid;

            ViewBag.FieldList = tbFieldService.GetTableColumList(_tbid, int.Parse(ViewBag.dbId));
            ViewBag.IndexList = indexStats.GetList();
            ViewBag.ParmSort = indexParmService.GetParmList();

            return View();
        }

        [HttpPost]
        public ActionResult AddReportIndexColParm(string rpId, int colId, string type, string parmId, string parmValue)
        {
            ReportIndexColParmService reportIndexColParm = new ReportIndexColParmService();

            if (comService.GetTotal("reportindexcolparm", "where ColId=" + colId + " and ParmId='" + parmId + "'") > 0)
            {
                return Json(ErrorTip("重复添加"));
            }

            ReportIndexColParmEntity model = new ReportIndexColParmEntity();
            model.ReportId = rpId;
            model.ColId = colId;
            model.ParmId = parmId;
            model.ParmValue = parmValue;
            model.Type = type;

            try
            {
                if (reportIndexColParm.Insert(model))
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip("添加失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetReportIndexColParmList(string rpId, string colId)
        {
            ReportIndexColParmService reportIndexColParm = new ReportIndexColParmService();

            var list = reportIndexColParm.GetByWhere("where colId=" + colId + "", null, null, "");
            var result = new { code = 0, msg = "", count = 99999, data = list };
            return Json(result);
        }

        [HttpPost]
        public JsonResult BatchDelIndexColParm(string idsStr)
        {
            ReportIndexColParmService reportIndexColParm = new ReportIndexColParmService();
            try
            {
                string err = reportIndexColParm.BatchDel(idsStr);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("删除成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        #endregion 参数赋值

        #region 显示列

        [HttpGet]
        public ActionResult AddReportIndexCol(string rpId, string tbid, string dbid)
        {
            string _tbid = GetTbID.GetTbIdByIdStr(tbid);
            ViewBag.rpId = rpId;
            ViewBag.tbid = _tbid;
            ViewBag.dbId = string.IsNullOrEmpty(dbid) ? "0" : dbid;

            IndexStatsService indexStats = new IndexStatsService();
            ViewBag.FieldList = tbFieldService.GetTableColumList(_tbid, int.Parse(ViewBag.dbId));
            ViewBag.IndexList = indexStats.GetList();

            return View();
        }

        [HttpGet]
        public ActionResult EditReportIndexCol(int id, string rpId, string tbid, string dbid)
        {
            ReportIndexColEntity model = reportIndex.GetById(id);
            if (model == null)
            {
                return Json(ErrorTip("数据不存在"));
            }
            ViewBag.Id = id;
            string _tbid = GetTbID.GetTbIdByIdStr(tbid);
            ViewBag.rpId = rpId;
            ViewBag.tbid = _tbid;
            ViewBag.dbId = string.IsNullOrEmpty(dbid) ? "0" : dbid;

            IndexStatsService indexStats = new IndexStatsService();
            ViewBag.FieldList = tbFieldService.GetTableColumList(_tbid, int.Parse(ViewBag.dbId));
            ViewBag.IndexList = indexStats.GetList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditReportIndexCol(int id, ReportIndexColEntity model)
        {
            model.Id = id;
            string updateFields = "ColName,IsSum,ColWidth,OrderNo";
            var result = reportIndex.UpdateById(model, updateFields) ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult AddReportIndexCol(string rpId, string tbid, string index, ReportIndexColEntity model)
        {
            try
            {
                string err = reportIndex.AddIndexCol(rpId, tbid, index, model);
                if (err == "")
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult EditReportIndexCol2(List<ReportIndexColEntity> gridlist)
        {
            try
            {
                foreach (ReportIndexColEntity obj in gridlist)
                {
                    comService.ExecuteSql("update reportindexcol set OrderNo = " + obj.OrderNo + ",ColWidth = " + obj.ColWidth + "   WHERE id = " + obj.Id);
                }
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult BatchDelReportIndexCol(string idsStr)
        {
            try
            {
                string err = reportIndex.BatchDel(idsStr);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("删除成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult IndexColList(string rpId)
        {
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            var model = reportService.GetReportModel(rpId);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                var result = ErrorTip("数据不存在");
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult GetReportIndexColList(string rpId, string tbid)
        {
            List<ReportIndexColEntity> list = new List<ReportIndexColEntity>();
            var _list = reportIndex.GetByWhere("where ReportId='" + rpId + "'", null, null, "ORDER BY OrderNo").ToList();
            foreach (ReportIndexColEntity obj in _list)
            {
                obj.Parm = "";
                if (obj.TbId == "#_indexstats#")
                {
                    obj.Parm = "【" + comService.GetSingleField("SELECT COUNT(1) FROM reportindexcolparm where ColId=" + obj.Id + " ") + "】";
                }

                if (obj.IsSum == "1")
                {
                    obj.IsSum = "求和";
                }
                else
                {
                    obj.IsSum = "";
                }
                list.Add(obj);
            }

            var result = new { code = 0, msg = "", count = 99999, data = list };
            return Json(result);
        }

        #endregion 显示列

        #region 查询列

        [HttpGet]
        public ActionResult IndexSearchList(string rpId)
        {
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            var model = reportService.GetReportModel(rpId);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                var result = ErrorTip("数据不存在");
                return Json(result);
            }
        }

        //这个好像不调用
        public ActionResult AddReportIndexSearch(ReportIndexSearchEntity model)
        {
            try
            {
                if (reportIndexSearch.Insert(model))
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip("添加失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult AddReportIndexSearch(string rpId, string tbid, string index, ReportIndexSearchEntity model)
        {
            try
            {
                string err = reportIndexSearch.AddIndexSearch(rpId, tbid, index, model);
                if (err == "")
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult AddReportIndexSearch(string rpId, string tbid, string dbid)
        {
            string _tbid = GetTbID.GetTbIdByIdStr(tbid);
            ViewBag.rpId = rpId;
            ViewBag.tbid = _tbid;
            ViewBag.dbId = string.IsNullOrEmpty(dbid) ? "0" : dbid;

            ViewBag.FieldList = tbFieldService.GetTableColumList(_tbid, int.Parse(ViewBag.dbId));

            return View();
        }

        [HttpPost]
        public JsonResult EditReportIndexSearch(List<ReportIndexSearchEntity> gridlist)
        {
            try
            {
                foreach (ReportIndexSearchEntity obj in gridlist)
                {
                    comService.ExecuteSql("update reportindexsearch set OrderNo = " + obj.OrderNo + "  WHERE id = " + obj.Id);
                }
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult BatchDelReportIndexSearch(string idsStr)
        {
            try
            {
                string err = reportIndexSearch.BatchDel(idsStr);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("删除成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetReportIndexSearchList(string rpId, string tbid)
        {
            var list = reportIndexSearch.GetByWhere("where ReportId='" + rpId + "'", null, null, "ORDER BY OrderNo");

            var result = new { code = 0, msg = "", count = 99999, data = list };
            return Json(result);
        }

        #endregion 查询列

        #region 排序列

        [HttpGet]
        public ActionResult AddReportIndexOrder(string rpId, string tbid, string dbid)
        {
            string _tbid = GetTbID.GetTbIdByIdStr(tbid);
            ViewBag.rpId = rpId;
            ViewBag.tbid = _tbid;
            ViewBag.dbId = string.IsNullOrEmpty(dbid) ? "0" : dbid;

            ViewBag.FieldList = tbFieldService.GetTableColumList(_tbid, int.Parse(ViewBag.dbId));

            return View();
        }

        [HttpGet]
        public ActionResult IndexOrderList(string rpId)
        {
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            var model = reportService.GetReportModel(rpId);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                var result = ErrorTip("数据不存在");
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult AddReportIndexOrder(string rpId, string tbid, string index, ReportIndexOrderEntity model)
        {
            try
            {
                string err = reportIndexOrder.AddIndexOrder(rpId, tbid, index, model);
                if (err == "")
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult BatchDelReportIndexOrder(string idsStr)
        {
            try
            {
                string err = reportIndexOrder.BatchDel(idsStr);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("删除成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult EditReportIndexOrder(List<ReportIndexOrderEntity> gridlist)
        {
            try
            {
                foreach (ReportIndexOrderEntity obj in gridlist)
                {
                    comService.ExecuteSql("update reportindexorder set OrderNo = " + obj.OrderNo + "  WHERE id = " + obj.Id);
                }
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetReportIndexOrderList(string rpId, string tbid)
        {
            var list = reportIndexOrder.GetByWhere("where ReportId='" + rpId + "'", null, null, "ORDER BY OrderNo");
            var result = new { code = 0, msg = "", count = 99999, data = list };
            return Json(result);
        }

        #endregion 排序列
    }
}