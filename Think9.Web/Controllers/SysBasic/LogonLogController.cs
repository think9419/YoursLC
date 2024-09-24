using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class LogonLogController : BaseController
    {
        private LogonLogService logService = new LogonLogService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
            //return View();
        }

        [HttpGet]
        public JsonResult List(LogonLogEntity model, PageInfoEntity pageInfo)
        {
            pageInfo.field = "CreateTime";
            pageInfo.order = "desc";

            long total = 0;
            IEnumerable<dynamic> list = logService.GetPageByFilter(ref total, model, pageInfo);

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            var result = logService.DeleteById(id) ? SuccessTip("删除成功") : ErrorTip("操作失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult BatchDel(string idsStr)
        {
            var idsArray = idsStr.Substring(0, idsStr.Length - 1).Split(',');
            var result = logService.DeleteByIds(idsArray) ? SuccessTip("批量删除成功") : ErrorTip("批量删除失败");
            return Json(result);
        }
    }
}