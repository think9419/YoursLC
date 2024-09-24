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
    public class ReportController : BaseController
    {
        private ComService comService = new ComService();
        private SortService sortService = new SortService();
        private ReportService reportService = new ReportService();
        private ReportRowsService reportRowsService = new ReportRowsService();
        private ReportDYNParmService reportDYNParmService = new ReportDYNParmService();
        private ReportParmQueryService reportParmQueryService = new ReportParmQueryService();
        private ReportColsService reportColsService = new ReportColsService();
        private ChangeModel changeModel = new ChangeModel();
        private TbBasicService tbServer = new TbBasicService();

        //分类
        private SelectList SortList
        { get { return new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_report"), "SortID", "SortName"); } }

        public JsonResult GetModelType(string rpid)
        {
            return Json(reportService.GetReportModelTypeById(rpid));
        }

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            ViewBag.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");//不能删
            return base.Index(id);
        }

        [HttpGet]
        public ActionResult GetReportList(PageInfoEntity pageInfo, string sortid, string key)
        {
            ReportBasicEntity model = new ReportBasicEntity();
            pageInfo.field = "ReportId";
            string where = "where 1=1 ";
            if (!string.IsNullOrEmpty(sortid))
            {
                where = "where 1=1 and ReportSort='" + sortid + "'";
            }
            string _keywords = key == null ? "" : key;
            if (_keywords != "")
            {
                where += " and (ReportId like @ReportName OR ReportName like @ReportName) ";
                model.ReportName = string.Format("%{0}%", _keywords);
            }

            try
            {
                var list = reportService.GetReportList(model, true, where);
                var result = new { code = 0, msg = "", count = 999999, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult GetReportListOnlyModel(PageInfoEntity pageInfo, string sortid, string key)
        {
            ReportBasicEntity model = new ReportBasicEntity();
            pageInfo.field = "ReportId";
            string where = "where 1=1 ";
            if (!string.IsNullOrEmpty(sortid))
            {
                where = "where 1=1 and ReportSort='" + sortid + "'";
            }
            string _keywords = key == null ? "" : key;
            if (_keywords != "")
            {
                where += " and (ReportId like @ReportName OR ReportName like @ReportName) ";
                model.ReportName = string.Format("%{0}%", _keywords);
            }

            try
            {
                var list = reportService.GetReportList(model, false, where);
                var result = new { code = 0, msg = "", count = 999999, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult GetReportTree()
        {
            List<TreeGridEntity> list = reportService.GetReportTreeList();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            var result = SuccessTip("操作成功", json);

            return Json(result);
        }

        [HttpPost]
        public ActionResult ChangeModel(string idsStr, string isRelease)
        {
            DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
            isRelease = string.IsNullOrEmpty(isRelease) ? "" : isRelease;
            try
            {
                changeModel.ChangeReportModel(dtRecord, isRelease, idsStr);
                RecordCodeService.Add(dtRecord);

                return Json(SuccessTip("操作成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult DYNParmList(string rpId)
        {
            ViewBag.RpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            return View();
        }

        [HttpPost]
        public ActionResult Test(string rpId)
        {
            //有查询参数
            if (comService.GetTotal("reportparmquery", "where ReportId='" + rpId + "'") > 0)
            {
                var result = SuccessTip("", "1");
                return Json(result);
            }
            else
            {
                var result = SuccessTip("", "0");
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult QueryParmList(string rpId)
        {
            ViewBag.RpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            return View();
        }

        [HttpGet]
        public ActionResult ParmAssignList(string rpId)
        {
            ViewBag.RpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            return View();
        }

        [HttpGet]
        public ActionResult AddDYNParm(string rpId)
        {
            ViewBag.RpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.SelectList = reportDYNParmService.GetSelectList(rpId);
            return View();
        }

        [HttpGet]
        public ActionResult EditDYNParm(string rpId, int id)
        {
            ViewBag.RpId = rpId;
            ViewBag.Id = id;
            ReportDYNParmEntity model = reportDYNParmService.GetById(id);
            if (model != null)
            {
                string rowId = comService.GetSingleField("select id  FROM reportrows WHERE DynamicId='" + id + "' ");
                model.Postion = reportRowsService.GetPostion(rowId == "" ? 0 : int.Parse(rowId));
                ViewBag.SelectList = reportDYNParmService.GetSelectList(rpId);
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpGet]
        public ActionResult AddQueryParm(string rpId)
        {
            ViewBag.RpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.Guid = System.Guid.NewGuid().ToString("N");
            ViewBag.SelectList = reportParmQueryService.GetSelectList(rpId);
            return View();
        }

        [HttpGet]
        public ActionResult EditQueryParm(string rpId, int id)
        {
            ViewBag.RpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.Id = id;
            var model = reportParmQueryService.GetById(id);
            if (model != null)
            {
                ViewBag.SelectList = reportParmQueryService.GetSelectList(rpId);
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpGet]
        public ActionResult QueryParmItem(string frm, string rpId, string parmid, string listid)
        {
            IndexParmService indexParmService = new IndexParmService();
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.parmId = parmid;
            ViewBag.frm = frm;
            ViewBag.listid = listid;

            string type = "string";
            var model = indexParmService.GetByWhereFirst("where ParmId=@ParmId", new { ParmId = parmid });
            if (model != null)
            {
                type = model.DataType;
            }
            ViewBag.parmType = type;

            return View();
        }

        [HttpPost]
        public ActionResult GetModel(string id)
        {
            var model = reportService.GetReportModel(id);
            if (model != null)
            {
                reportService.GetModelExa(id, ref model);
                var menuJson = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var result = SuccessTip("操作成功", menuJson);
                return Json(result);
            }
            else
            {
                var result = ErrorTip("对象为空，请刷新后再操作！");
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult AddReport(string frm)
        {
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.SortList = SortList;
            ViewBag.ColNum = reportService.GetColNumList();
            ViewBag.FontStyle = FontStyleList.GetList();

            return View();
        }

        [HttpGet]
        public ActionResult EditReport(string rpId)
        {
            ViewBag.rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            ViewBag.SortList = SortList;
            ViewBag.FontStyle = FontStyleList.GetList();

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
        public ActionResult AddReport(ReportBasicEntity entity, IEnumerable<ReportColsEntity> list)
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
                string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                entity.CreateUser = userid;
                err = reportService.AddReport(entity, list);

                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("操作成功"));
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
        public ActionResult EditReport(ReportBasicEntity entity, IEnumerable<ReportColsEntity> list)
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

        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.SortList = SortList;
            var model = reportService.GetByWhereFirst("where ReportId=@ReportId", new { ReportId = id });
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpGet]
        public ActionResult AddReportRows(string rpId)
        {
            ViewBag.rpId = rpId;
            return View();
        }

        [HttpPost]
        public ActionResult AddReportRows(string rpId, int type, int order)
        {
            try
            {
                string err = reportRowsService.AddRows(rpId, type, order);
                if (err == "")
                {
                    return Json(SuccessTip("操作成功"));
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
        public ActionResult EditReportCols(string rpId, string frm)
        {
            ViewBag.rpId = rpId;
            ViewBag.frm = frm;

            return View();
        }

        [HttpPost]
        public ActionResult EditColWidthAndType(string rpId, int num, decimal width, string type)
        {
            string err = "";

            if (width > 30 || width < 1)
            {
                err = "宽度应在1-30之间";
            }

            if (err == "")
            {
                if (reportColsService.EditReportColWidthAndType(rpId, num.ToString(), width, type) == 0)
                {
                    err += " 编辑失败";
                }
            }

            if (err == "")
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpPost]
        public ActionResult EditRow(string rpId, int rowId, int? order, decimal? height, string type)
        {
            height = height == null ? 1 : height;
            order = order == null ? 1 : order;
            if (height > 10 || height < 0.3M)
            {
                return Json(ErrorTip("高度应在0.3-10之间"));
            }
            if (order <= 0)
            {
                return Json(ErrorTip("序号应大于0"));
            }

            string err = reportRowsService.EditRowsOrderAndType(rpId, rowId, order, height, type);

            if (err == "")
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpPost]
        public ActionResult InsertReportColsBehind(string rpId, int num)
        {
            try
            {
                string err = reportColsService.InsertColsBehind(rpId, num);

                if (err == "")
                {
                    int ColsN = (int)comService.GetTotal("reportcols", "where ReportId='" + rpId + "'");
                    List<ControlEntity> list = reportColsService.GetColEntityList(rpId, num, ColsN);
                    return Json(SuccessTip("操作成功", list, ColsN.ToString()));
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
        public ActionResult InsertReportColsFront(string rpId, int num)
        {
            try
            {
                string err = reportColsService.InsertColsFront(rpId, num);
                if (err == "")
                {
                    int ColsN = (int)comService.GetTotal("reportcols", "where ReportId='" + rpId + "'");
                    List<ControlEntity> list = reportColsService.GetColEntityList(rpId, num, ColsN);
                    return Json(SuccessTip("操作成功", list, ColsN.ToString()));
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
        public JsonResult GetColNumber(string rpId, int iCount)
        {
            List<ReportColsEntity> list = new List<ReportColsEntity>();
            for (int i = 1; i <= iCount; i++)
            {
                list.Add(new ReportColsEntity { ColNum = i, ColWidth = 3, ColType = "string" });
            }

            var result = new { code = 0, msg = "", count = list.Count, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetReportColList(string rpId)
        {
            string str;
            string strType = "<option value='string'>string</option> <option value='int'>int</option> <option value='decimal'>decimal</option> <option value='date'>date</option> <option value='img'>img</option>";

            IEnumerable<ReportColsEntity> list = reportColsService.GetByWhere("where ReportId='" + rpId + "'", null, null, "order by ColNum");

            foreach (ReportColsEntity obj in list)
            {
                str = string.IsNullOrEmpty(obj.ColType) ? "string" : obj.ColType;
                obj.ColType_Exa = strType.Replace("<option value='" + str + "'>", "<option value='" + str + "' selected>");
            }

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetColsList(string rpId)
        {
            var list = reportColsService.GetByWhere("where ReportId='" + rpId + "'", null, null, "ORDER BY ColNum");

            var result = new { code = 0, msg = "", count = 99999, data = list };
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetReportParmAssign(string rpId, string type, string parmId)
        {
            rpId = string.IsNullOrEmpty(rpId) ? "" : rpId;
            type = string.IsNullOrEmpty(type) ? "" : type;
            parmId = string.IsNullOrEmpty(parmId) ? "" : parmId;

            var list = reportService.GetReportParmAssignList(rpId, type, parmId);

            var result = new { code = 0, msg = "", count = 99999, data = list };

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetTbIndexTips(string str)
        {
            return Json(TbIndexService.GetTbIndexExplain(str));
        }

        [HttpGet]
        public ActionResult Warn(string idsStr, string from)
        {
            string err = AppSet.CheckConnection();
            if (!string.IsNullOrEmpty(err))
            {
                return Json(ErrorTip(err));
            }

            ViewBag.tbid = idsStr;
            ViewBag.from = string.IsNullOrEmpty(from) ? "" : from;
            return View();
        }

        [HttpPost]
        public ActionResult GetWarnList(string idsStr, string from)
        {
            try
            {
                CheckCodeBuild _check = new CheckCodeBuild();
                List<WarnEntity> list = _check.GetReportsWarning(idsStr, CurrentUser);

                var result = new { code = 0, msg = "", count = list.Count, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Err：" + ex.Message, count = -1 });
            }
        }

        [HttpPost]
        public JsonResult DelReport(string rpId)
        {
            try
            {
                reportService.DelReportALL(rpId);
                return Json(SuccessTip("删除成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult DelReportCols(string rpId, int num)
        {
            try
            {
                string err = reportColsService.DelCols(rpId, num);
                if (err == "")
                {
                    int colsN = (int)comService.GetTotal("reportcols", "where ReportId='" + rpId + "'");
                    List<ControlEntity> list = reportColsService.GetColEntityList(rpId, num, colsN);
                    return Json(SuccessTip("操作成功", list, colsN.ToString()));
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
        public ActionResult BatchDelReportParm(string idsStr)
        {
            try
            {
                reportService.DelReportParm(idsStr);
                return Json(SuccessTip("操作成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}