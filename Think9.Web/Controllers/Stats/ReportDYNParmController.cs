using Microsoft.AspNetCore.Mvc;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Stats;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class ReportDYNParmController : BaseController
    {
        private ComService comService = new ComService();
        private ReportDYNParmService reportDYNParmService = new ReportDYNParmService();
        private IndexParmService indexParmService = new IndexParmService();

        //[HttpGet]
        //public ActionResult DYNParmList(string id)
        //{
        //    ViewBag.RpId = string.IsNullOrEmpty(id) ? "" : id;
        //    return View();
        //}

        [HttpPost]
        public ActionResult GetList(string rpId)
        {
            rpId = rpId == null ? "" : rpId;

            var result = new { code = 0, msg = "", count = 999999, data = reportDYNParmService.GetList(rpId) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add(string rpId)
        {
            ViewBag.frm = string.IsNullOrEmpty(rpId) ? "" : rpId;

            return View();
        }

        [HttpPost]
        public ActionResult Add(string rpId, ReportDYNParmEntity model)
        {
            model.ReportId = rpId;
            var result = reportDYNParmService.Add(model) ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("操作失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult Edit(ReportDYNParmEntity model)
        {
            var result = reportDYNParmService.Edit(model) > 0 ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("操作失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult Remove(string id)
        {
            var result = reportDYNParmService.Delete(id) > 0 ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("操作失败");
            return Json(result);
        }
    }
}