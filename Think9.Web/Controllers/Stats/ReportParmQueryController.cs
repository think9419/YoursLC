using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Stats;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class ReportParmQueryController : BaseController
    {
        private ComService comService = new ComService();
        private ReportParmQueryService reportParmQueryService = new ReportParmQueryService();
        private IndexParmService indexParmService = new IndexParmService();

        [HttpPost]
        public ActionResult GetParmQueryList(string rpId)
        {
            rpId = rpId == null ? "" : rpId;

            var result = new { code = 0, msg = "", count = 999999, data = reportParmQueryService.GetQueryParmList(rpId) };
            return Json(result);
        }

        public ActionResult GetParmQueryItem(string listid, string frm)
        {
            listid = listid == null ? "0" : listid;
            frm = frm == null ? "" : frm;

            var result = new { code = 0, msg = "", count = 999999, data = reportParmQueryService.GetItemList(listid, frm) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add(string rpId)
        {
            ViewBag.frm = string.IsNullOrEmpty(rpId) ? "" : rpId;

            return View();
        }

        [HttpPost]
        public ActionResult AddParmQuery(string rpId, string listid, ReportParmQueryEntity model)
        {
            try
            {
                model.ReportId = rpId;
                string err = reportParmQueryService.AddQueryParm(rpId, listid, model);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip("操作失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult AddItem(string frm, string rpid, string listid, string parmid, valueTextEntity model)
        {
            string err = reportParmQueryService.AddItem(rpid, frm, listid, parmid, model);
            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("", reportParmQueryService.GetItemOrderNo(frm, listid).ToString()));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        public ActionResult DelItem(string frm, string id)
        {
            string err = reportParmQueryService.DeleteItem(frm, id);
            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpPost]
        public ActionResult EditParmQuery(ReportParmQueryEntity model, string listid)
        {
            try
            {
                model.ListId = string.IsNullOrEmpty(listid) ? 0 : int.Parse(listid);
                var result = reportParmQueryService.EditQueryParm(model) > 0 ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("操作失败");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult DeleteParmQuery(string id)
        {
            var result = reportParmQueryService.DeleteQueryParm(id) > 0 ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("操作失败");
            return Json(result);
        }
    }
}