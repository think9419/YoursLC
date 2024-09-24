using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class RuleSingleController : BaseController
    {
        private RuleSingleService ruleSingleService = new RuleSingleService();
        private RuleServiceBasic ruleService = new RuleServiceBasic();
        private RuleListService ruleListService = new RuleListService();

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SelectSourceList = ruleService.GetSelectSourceList();
            return View();
        }

        [HttpPost]
        public ActionResult Add(RuleSingleEntity model)
        {
            string err = "";
            if (model.DbID != 0)
            {
                model.TbId = model.TbId_Exa;
            }
            if (string.IsNullOrEmpty(model.TbId))
            {
                return Json(ErrorTip("未选择取值数据表"));
            }
            if (string.IsNullOrEmpty(model.ValueFiled))
            {
                return Json(ErrorTip("未选择Value字段"));
            }
            if (string.IsNullOrEmpty(model.TxtFiled))
            {
                return Json(ErrorTip("未选择Text字段"));
            }
            if (string.IsNullOrEmpty(model.OrderFiled))
            {
                return Json(ErrorTip("未选择排序字段"));
            }

            model.ValueFiled = BasicHelp.GetSplitStrByChar(model.ValueFiled, ".", 1);//获取分割后字符
            model.TxtFiled = BasicHelp.GetSplitStrByChar(model.TxtFiled, ".", 1);//获取分割后字符
            if (model.DbID != 0)
            {
                model.TbId = model.TbId_Exa;
            }

            if (string.IsNullOrEmpty(model.TbId))
            {
                return Json(ErrorTip("未选择取值数据表"));
            }

            try
            {
                model.LimitStr = model.LimitStr == null ? "" : BaseUtil.ReplaceHtml(model.LimitStr);
                err = ruleSingleService.Add(model);
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (err == "")
            {
                return Json(SuccessTip("新建成功"));
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

            try
            {
                var model = ruleSingleService.GetByWhereFirst(where, param);
                var modelList = ruleListService.GetByWhereFirst(where, param);
                if (model != null && modelList != null)
                {
                    ViewBag.SelectSourceList = ruleService.GetSelectValueListByTbid(model.DbID, model.TbId);
                    ruleSingleService.SetDefault(ref model);
                    model.Name = modelList.RuleName;
                    model.DbID_Exa = ExternalDbService.GetName(model.DbID);

                    return View(model);
                }
                else
                {
                    return Json("数据不存在！");
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(RuleSingleEntity model, string id)
        {
            string err = "";
            try
            {
                model.LimitStr = model.LimitStr == null ? "" : BaseUtil.ReplaceHtml(model.LimitStr);
                err = ruleSingleService.Edit(model);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("编辑成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return Json(ErrorTip(err));
            }
        }

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

        [HttpGet]
        public JsonResult GetSelectValueFieldByTb(string id, string dbId)
        {
            try
            {
                if (dbId == "0")
                {
                    return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "" : id));
                }
                return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public JsonResult GetSelectTxtFieldByTb(string id, string dbId)
        {
            try
            {
                if (dbId == "0")
                {
                    return Json(ruleService.GetSelectTxtFieldListByTbid(id == null ? "" : id));
                }
                return Json(ruleService.GetSelectTxtFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetSelectOrderFieldByTb(string id, string dbId)
        {
            try
            {
                if (dbId == "0")
                {
                    return Json(ruleService.GetSelectOrderFieldListByTbid(id == null ? "" : id));
                }
                return Json(ruleService.GetSelectOrderFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetConditionFieldList(string id, string dbId)
        {
            try
            {
                string prefix = "";
                if (dbId == "0")
                {
                    return Json(ruleService.GetConditionFieldListByTbid(id == null ? "" : id));
                }
                return Json(ruleService.GetConditionFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}