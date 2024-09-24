using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class RuleListController : BaseController
    {
        private TbIndexService tbIndexService = new TbIndexService();
        private RuleListService ruleListService = new RuleListService();

        public override ActionResult Index(int? id)
        {
            ViewBag.ClassID = "1";

            return base.Index(id);
        }

        [HttpGet]
        public JsonResult GetList(RuleListEntity model, PageInfoEntity pageInfo, string classid, string key)
        {
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string _key = key == null ? "" : key;
            string where = "where RuleType='" + classid + "'";
            if (_key != "")
            {
                where += " and RuleName like @RuleName ";
                model.RuleName = string.Format("%{0}%", _key);
            }

            long total = 0;
            IEnumerable<dynamic> list = ruleListService.GetPageListBySearch(ref total, model, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpGet]
        public ActionResult PUSearchIndex(string id)
        {
            string _key = id == null ? "" : id;
            ViewBag.tbid = _key.Replace("#table#", "");
            return View();
        }

        [HttpGet]
        public ActionResult GetIndexByTbID(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tbIndexService.GetIndexByTbID(tbid) };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetSysParameter()
        {
            var result = new { code = 0, msg = "", count = 999999, data = SysParameter.GetList() };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetSysParameter2(string type)
        {
            var result = new { code = 0, msg = "", count = 999999, data = SysParameter.GetList(type) };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetFileType()
        {
            var result = new { code = 0, msg = "", count = 999999, data = SysParameter.GetFileType() };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetImgType()
        {
            var result = new { code = 0, msg = "", count = 999999, data = SysParameter.GetImgType() };
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            ruleListService.DeleteRule(id);
            return Json(SuccessTip("删除成功，已关联该数据规范的录入表指标需重新指定数据规范！"));
        }

        [HttpGet]
        public JsonResult BatchDel(string idsStr)
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
                        ruleListService.DeleteRule(id);
                    }
                }
            }
            return Json(SuccessTip("删除成功，已关联该数据规范的录入表指标需重新指定数据规范！"));
        }

        [HttpPost]
        public ActionResult RuleListTreeSelect()
        {
            List<TreeGridEntity> list = ruleListService.GetRuleTreeSelect();

            var menuJson = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            var result = SuccessTip("操作成功", menuJson);

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetRuleName(string tbid, string indexid)
        {
            return Json(SuccessTip("", ruleListService.GetRuleName(tbid, indexid)));
        }

        [HttpPost]
        public JsonResult UpNameByID(string id, string name)
        {
            RuleListEntity model = new RuleListEntity();
            model.RuleName = name;
            model.RuleId = id;

            string where = "where RuleId='" + model.RuleId + "'";
            string updateFields = "RuleName";

            var result = ruleListService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");
            return Json(result);
        }
    }
}