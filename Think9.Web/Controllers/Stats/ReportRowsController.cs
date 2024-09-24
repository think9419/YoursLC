using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Stats;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class ReportRowsController : BaseController
    {
        private ComService comService = new ComService();
        private ReportService reportService = new ReportService();
        private IndexStatsService indexStatsService = new IndexStatsService();
        private IndexParmService indexParmService = new IndexParmService();
        private ReportParmService reportParmService = new ReportParmService();
        private ReportRowsService reportRowsService = new ReportRowsService();
        private ReportColsService reportColsService = new ReportColsService();

        [HttpGet]
        public ActionResult RowList(string frm, string rpId)
        {
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.MaxNum = reportService.GetReportColNum(rpId);
            ViewBag.colsNum = comService.GetTotal("reportcols", "WHERE ReportId='" + rpId + "'");
            ViewBag.rpId = rpId;
            ViewBag.rpName = comService.GetSingleField("SELECT ReportName  FROM reportbasic WHERE ReportId='" + rpId + "' ");

            ColsEntity model = reportColsService.GetColEntity(rpId);
            ViewBag.WidthN1 = model.WidthColN1;
            ViewBag.WidthN2 = model.WidthColN2;
            ViewBag.WidthN3 = model.WidthColN3;
            ViewBag.WidthN4 = model.WidthColN4;
            ViewBag.WidthN5 = model.WidthColN5;
            ViewBag.WidthN6 = model.WidthColN6;
            ViewBag.WidthN7 = model.WidthColN7;
            ViewBag.WidthN8 = model.WidthColN8;
            ViewBag.WidthN9 = model.WidthColN9;
            ViewBag.WidthN10 = model.WidthColN10;

            ViewBag.WidthN11 = model.WidthColN11;
            ViewBag.WidthN12 = model.WidthColN12;
            ViewBag.WidthN13 = model.WidthColN13;
            ViewBag.WidthN14 = model.WidthColN14;
            ViewBag.WidthN15 = model.WidthColN15;
            ViewBag.WidthN16 = model.WidthColN16;
            ViewBag.WidthN17 = model.WidthColN17;
            ViewBag.WidthN18 = model.WidthColN18;
            ViewBag.WidthN19 = model.WidthColN19;
            ViewBag.WidthN20 = model.WidthColN20;

            ViewBag.WidthN21 = model.WidthColN21;
            ViewBag.WidthN22 = model.WidthColN22;
            ViewBag.WidthN23 = model.WidthColN23;
            ViewBag.WidthN24 = model.WidthColN24;
            ViewBag.WidthN25 = model.WidthColN25;
            ViewBag.WidthN26 = model.WidthColN26;
            ViewBag.WidthN27 = model.WidthColN27;
            ViewBag.WidthN28 = model.WidthColN28;
            ViewBag.WidthN29 = model.WidthColN29;
            ViewBag.WidthN30 = model.WidthColN30;

            ViewBag.WidthN31 = model.WidthColN31;
            ViewBag.WidthN32 = model.WidthColN32;
            ViewBag.WidthN33 = model.WidthColN33;
            ViewBag.WidthN34 = model.WidthColN34;
            ViewBag.WidthN35 = model.WidthColN35;
            ViewBag.WidthN36 = model.WidthColN36;
            ViewBag.WidthN37 = model.WidthColN37;
            ViewBag.WidthN38 = model.WidthColN38;
            ViewBag.WidthN39 = model.WidthColN39;
            ViewBag.WidthN40 = model.WidthColN40;

            ViewBag.WidthN41 = model.WidthColN41;
            ViewBag.WidthN42 = model.WidthColN42;
            ViewBag.WidthN43 = model.WidthColN43;
            ViewBag.WidthN44 = model.WidthColN44;
            ViewBag.WidthN45 = model.WidthColN45;
            ViewBag.WidthN46 = model.WidthColN46;
            ViewBag.WidthN47 = model.WidthColN47;
            ViewBag.WidthN48 = model.WidthColN48;
            ViewBag.WidthN49 = model.WidthColN49;
            ViewBag.WidthN50 = model.WidthColN50;

            return View();
        }

        [HttpGet]
        public ActionResult RowCellsList(string frm, string rpId, string rowId)
        {
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.rowId = string.IsNullOrEmpty(rowId) ? "0" : rowId;
            ViewBag.MaxNum = reportService.GetReportColNum(rpId);
            ViewBag.colsNum = comService.GetTotal("reportcols", "where ReportId='" + rpId + "'");
            ViewBag.rpId = rpId;
            ViewBag.rpName = comService.GetSingleField("SELECT ReportName FROM reportbasic WHERE ReportId='" + rpId + "' ");

            string OrderNo = "";
            string Type = "1";
            string Height = "0.8";
            ReportRowsEntity model = reportRowsService.GetByWhereFirst("WHERE Id=" + rowId + " ");
            if (model != null)
            {
                OrderNo = model.OrderNo.ToString();
                Type = model.Type == 0 ? "1" : model.Type.ToString();
                Height = model.Height == 0 ? "0.8" : model.Height.ToString();
            }
            ViewBag.Type = Type;
            ViewBag.OrderNo = OrderNo;
            ViewBag.Height = Height;

            return View();
        }

        [HttpGet]
        public ActionResult UpdateCellsPattern(string rpId, string rowId, int colN, int num, string value)
        {
            if (reportRowsService.EditRowsEntity(rowId, colN, num, value))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }

        [HttpGet]
        public ActionResult GetRowCellsList(string rpId, int rowId, int colN)
        {
            try
            {
                IEnumerable<dynamic> list = reportRowsService.GetRowsEntity(rpId, rowId, colN);
                if (list == null)
                {
                    return Json(new { msg = "数据不存在", count = -1 });
                }
                else
                {
                    return Json(new { code = 0, msg = "", count = 99999, data = list });
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        [HttpPost]
        public ActionResult Refresh(string rpId)
        {
            int ColsN = (int)comService.GetTotal("reportcols", "WHERE ReportId='" + rpId + "'");
            List<ControlEntity> list = reportColsService.GetColEntityList(rpId, 1, ColsN);
            return Json(SuccessTip("", list, ColsN.ToString()));
        }

        [HttpGet]
        public ActionResult SetArrange(string n, string id)
        {
            ViewBag.n = string.IsNullOrEmpty(n) ? "0" : n;
            ViewBag.postion = "第" + n + "列";
            ViewBag.ParmNum = comService.GetTotal("reportparm", "WHERE ReportId='" + id + "' AND ColNum=" + n + "");
            ViewBag.rpId = id;
            ViewBag.rpName = comService.GetSingleField("SELECT ReportName FROM reportbasic WHERE ReportId='" + id + "' ");
            ViewBag.IndexSort = indexStatsService.GetIndexSort(true);
            ViewBag.ParmSort = indexParmService.GetParmList();

            string ColWidth = "";
            string ColType = "string";
            ReportColsEntity model = reportColsService.GetByWhereFirst("WHERE ReportId='" + id + "' AND ColNum=" + n + "");
            if (model != null)
            {
                ColWidth = model.ColWidth.ToString();
                ColType = model.ColType;
            }
            ViewBag.ColType = ColType;
            ViewBag.ColWidth = ColWidth;

            return View();
        }

        [HttpGet]
        public ActionResult SetLine(string rowId, string rpId)
        {
            ViewBag.rowId = string.IsNullOrEmpty(rowId) ? "0" : rowId;
            ViewBag.postion = reportRowsService.GetPostion(int.Parse(rowId));
            ViewBag.ParmNum = comService.GetTotal("reportparm", "WHERE ReportId='" + rpId + "' AND RowId=" + rowId + "");
            ViewBag.rpId = rpId;
            ViewBag.rpName = comService.GetSingleField("SELECT ReportName  FROM reportbasic WHERE ReportId='" + rpId + "' ");
            ViewBag.IndexSort = indexStatsService.GetIndexSort(true);
            ViewBag.ParmSort = indexParmService.GetParmList();

            string OrderNo = "";
            string Type = "1";
            string Height = "0.8";
            ReportRowsEntity model = reportRowsService.GetByWhereFirst("WHERE Id=" + rowId + " ");
            if (model != null)
            {
                OrderNo = model.OrderNo.ToString();
                Type = model.Type == 0 ? "1" : model.Type.ToString();
                Height = model.Height == 0 ? "0.8" : model.Height.ToString();
            }
            ViewBag.Type = Type;
            ViewBag.OrderNo = OrderNo;
            ViewBag.Height = Height;

            return View();
        }

        [HttpGet]
        public ActionResult SetCells(int rowId, string rpId, int colN)
        {
            ViewBag.postion = reportRowsService.GetPostion(rowId) + "第" + colN.ToString() + "列";
            ViewBag.rowId = rowId;
            ViewBag.colN = colN;
            ViewBag.ParmNum = comService.GetTotal("reportparm", "WHERE ReportId='" + rpId + "' AND RowId=" + rowId + " AND ColNum=" + colN + "");
            ViewBag.rpId = rpId;
            ViewBag.rpName = comService.GetSingleField("SELECT ReportName  FROM reportbasic WHERE ReportId='" + rpId + "' ");
            ViewBag.IndexSort = indexStatsService.GetIndexSort(true);
            ViewBag.ParmSort = indexParmService.GetParmList();

            return View(reportRowsService.GetCellsEntity(rpId, rowId, colN));
        }

        [HttpPost]
        public ActionResult EditArrange(string rpId, string colNum, string sort, string value, string text)
        {
            value = value == null ? "" : value;
            text = text == null ? "" : text;

            if (reportService.EditArrangeByReportId(rpId, colNum, sort, value, text))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }

        [HttpPost]
        public ActionResult EditLine(string rpId, string rowId, string sort, string value, string text)
        {
            if (reportService.EditLineByRowId(rpId, rowId, sort, value, text))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }

        [HttpPost]
        public ActionResult DelLine(string rpId, string rowId, string dynId)
        {
            if (reportService.DelLineByRowId(rpId, rowId, dynId))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("删除失败"));
            }
        }

        [HttpPost]
        public ActionResult BatchDelLine(string rpId, string idsStr)
        {
            if (reportService.BatchDelLine(rpId, idsStr))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("删除失败"));
            }
        }

        [HttpPost]
        public ActionResult EditCells(string rpId, string rowId, int colNum, string sort, string value, string text, string state)
        {
            if (reportService.EditCellsByRowIdAndColNum(rpId, colNum, rowId, sort, value, text, state))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }

        [HttpPost]
        public ActionResult AddArrangeParm(string rpId, int colNum, string parmId, string parmValue, bool isDel)
        {
            parmValue = string.IsNullOrEmpty(parmValue) ? "" : parmValue;
            int num = reportParmService.AddArrangeParm(rpId, colNum, parmId, parmValue, isDel);
            return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
        }

        [HttpPost]
        public ActionResult AddLineParm(string rpId, int rowId, string parmId, string parmValue, bool isDel)
        {
            parmValue = string.IsNullOrEmpty(parmValue) ? "" : parmValue;
            int num = reportParmService.AddLineParm(rpId, rowId, parmId, parmValue, isDel);
            return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
        }

        [HttpPost]
        public ActionResult AddCellsParm(string rpId, int rowId, int colNum, string parmId, string parmValue, bool isDel)
        {
            parmValue = string.IsNullOrEmpty(parmValue) ? "" : parmValue;
            int num = reportParmService.AddCellsParm(rpId, rowId, colNum, parmId, parmValue, isDel);
            return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
        }

        [HttpGet]
        public JsonResult BatchDelParm(string idsStr)
        {
            string id = "";
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    if (arr[i].ToString().Trim() != "")
                    {
                        id = arr[i].ToString().Trim();
                        string where = "WHERE Id=" + id + "";
                        reportParmService.DeleteByWhere(where);
                    }
                }
            }
            return Json(SuccessTip("删除成功"));
        }

        [HttpPost]
        public ActionResult GetReportParmByColNum(PageInfoEntity pageInfo, string rpId, string colNum)
        {
            pageInfo.field = "ParmId ";
            long total = 0;

            ReportParmEntity model = new ReportParmEntity();
            model.ReportId = rpId;
            model.ColNum = string.IsNullOrEmpty(colNum) ? 0 : int.Parse(colNum);

            IEnumerable<dynamic> list = reportParmService.GetPageByFilter(ref total, pageInfo, model);

            return Json(new { code = 0, msg = "", count = total, data = list });
        }

        [HttpPost]
        public ActionResult GetReportParmByRowID(PageInfoEntity pageInfo, string rpId, string rowId)
        {
            pageInfo.field = "ParmId ";
            long total = 0;

            ReportParmEntity model = new ReportParmEntity();
            model.ReportId = rpId;
            model.RowId = string.IsNullOrEmpty(rowId) ? 0 : int.Parse(rowId);

            IEnumerable<dynamic> list = reportParmService.GetPageByFilter(ref total, pageInfo, model);

            return Json(new { code = 0, msg = "", count = total, data = list });
        }

        [HttpPost]
        public ActionResult GetReportParmByRowIDAndColNum(PageInfoEntity pageInfo, string rpId, string rowId, string colNum)
        {
            pageInfo.field = "ParmId ";
            long total = 0;

            ReportParmEntity model = new ReportParmEntity();
            model.ReportId = rpId;
            model.RowId = string.IsNullOrEmpty(rowId) ? 0 : int.Parse(rowId);
            model.ColNum = string.IsNullOrEmpty(colNum) ? 0 : int.Parse(colNum);

            IEnumerable<dynamic> list = reportParmService.GetPageByFilter(ref total, pageInfo, model);

            return Json(new { code = 0, msg = "", count = total, data = list });
        }

        [HttpPost]
        public ActionResult GetList(PageInfoEntity pageInfo, string rpId)
        {
            pageInfo.field = "Type,OrderNo ";
            long total = 0;
            try
            {
                IEnumerable<dynamic> list = reportRowsService.GetPageByFilter(ref total, null, pageInfo, "where ReportId='" + rpId + "'");
                reportRowsService.GetDeaftValue(ref list);

                return Json(new { code = 0, msg = "", count = total, data = list });
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}