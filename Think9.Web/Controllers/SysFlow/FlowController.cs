using Microsoft.AspNetCore.Mvc;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Flow;

namespace Think9.Controllers.Basic
{
    [Area("SysFlow")]
    public class FlowController : BaseController
    {
        private FlowService flowService = new FlowService();
        private ComService comService = new ComService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public JsonResult GetList(FlowEntity model, PageInfoEntity pageInfo, string key)
        {
            pageInfo.field = "flowid";
            string where = "WHERE  LEFT(flowid,3) = 'fw_' AND flowType='1'";
            string _keywords = key == null ? "" : key;
            if (_keywords != "")
            {
                where += " AND (FlowName LIKE @FlowName OR FlowId LIKE @FlowName) ";
                model.FlowName = string.Format("%{0}%", _keywords);
            }

            var result = flowService.GetPageByFilter(model, pageInfo, where);
            return Json(result);
        }

        public ActionResult Edit(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(FlowEntity model)
        {
            return Json(SuccessTip("操作成功"));
        }
    }
}