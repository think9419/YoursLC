using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class RuleTreeController : BaseController
    {
        private RuleTreeService ruleTreeService = new RuleTreeService();
        private RuleServiceBasic ruleService = new RuleServiceBasic();
        private RuleListService ruleListService = new RuleListService();

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SelectSourceList = ruleService.GetSelectSourceList();
            return View();
        }

        [HttpPost]
        public ActionResult Add(RuleTreeEntity model)
        {
            string err = "";
            model.ValueFiled = BasicHelp.GetSplitStrByChar(model.ValueFiled, ".", 1);//获取分割后字符
            model.TxtFiled = BasicHelp.GetSplitStrByChar(model.TxtFiled, ".", 1);//获取分割后字符
            model.LimitStr = model.LimitStr == null ? "" : BaseUtil.ReplaceHtml(model.LimitStr);

            try
            {
                err = ruleTreeService.Add(model);
            }
            catch (Exception ex)
            {
                err = ex.Message;
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

        [HttpGet]
        public ActionResult Edit(string rid)
        {
            ViewBag.id = rid;
            string where = "where RuleId=@RuleId";
            object param = new { RuleId = rid };
            var model = ruleTreeService.GetByWhereFirst(where, param);
            var modelList = ruleListService.GetByWhereFirst(where, param);
            if (model != null && modelList != null)
            {
                ViewBag.SelectSourceList = ruleService.GetSelectValueListByTbid(model.DbID, model.TbId);
                ruleTreeService.SetDefault(ref model);
                model.Name = modelList.RuleName;
                model.DbID_Exa = ExternalDbService.GetName(model.DbID);
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(RuleTreeEntity model, string id)
        {
            string err = "";
            model.ValueFiled = BasicHelp.GetSplitStrByChar(model.ValueFiled, ".", 1);//获取分割后字符
            model.TxtFiled = BasicHelp.GetSplitStrByChar(model.TxtFiled, ".", 1);//获取分割后字符

            try
            {
                model.LimitStr = model.LimitStr == null ? "" : BaseUtil.ReplaceHtml(model.LimitStr);
                err = ruleTreeService.Edit(model);
            }
            catch (Exception ex)
            {
                err = ex.Message;
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

        [HttpGet]
        public JsonResult GetSelectValueFieldByTb(string id)
        {
            return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "" : id));
        }

        [HttpGet]
        public JsonResult GetSelectValueFieldByTb2(string id, string dbId)
        {
            return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "0" : id, dbId));
        }

        //[HttpGet]
        //public JsonResult GetSelectValueFieldByTb3(string id, string dbId)
        //{
        //    if (dbId == "0")
        //    {
        //        return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "" : id));
        //    }
        //    else
        //    {
        //        return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "0" : id, dbId));
        //    }
        //}

        [HttpGet]
        public JsonResult GetSelectTxtFieldByTb(string id)
        {
            return Json(ruleService.GetSelectTxtFieldListByTbid(id == null ? "" : id));
        }

        [HttpGet]
        public JsonResult GetSelectOrderFieldByTb(string id)
        {
            return Json(ruleService.GetSelectOrderFieldListByTbid(id == null ? "" : id));
        }

        [HttpGet]
        public JsonResult GetConditionFieldList(string id)
        {
            return Json(ruleService.GetConditionFieldListByTbid(id == null ? "" : id));
        }
    }
}