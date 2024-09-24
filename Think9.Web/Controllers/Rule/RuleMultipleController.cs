using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class RuleMultipleController : BaseController
    {
        private RuleMultipleService ruleMultipleService = new RuleMultipleService();
        private RuleServiceBasic ruleService = new RuleServiceBasic();
        private RuleListService ruleListService = new RuleListService();
        private ComService comService = new ComService();

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Guid = Think9.Services.Basic.CreatCode.NewGuid();
            ViewBag.SelectSourceList = ruleService.GetSelectSourceList();

            return View();
        }

        [HttpPost]
        public ActionResult Add(RuleMultipleEntity model, string id)
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

            long total = comService.GetTotal("sys_temp", "where Guid='" + id + "'");
            if (total == 0)
            {
                return Json(ErrorTip("未添加列表字段"));
            }
            if (total > 10)
            {
                return Json(ErrorTip("列表字段最多不超过10列"));
            }

            try
            {
                model.LimitStr = model.LimitStr == null ? "" : BaseUtil.ReplaceHtml(model.LimitStr);
                err = ruleMultipleService.Add(model, id);
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
            string where = "where RuleId=@RuleId";
            object param = new { RuleId = rid };
            try
            {
                var model = ruleMultipleService.GetByWhereFirst(where, param);
                var modelList = ruleListService.GetByWhereFirst(where, param);
                if (model != null && modelList != null)
                {
                    ViewBag.id = rid;
                    ViewBag.SelectSourceList = ruleService.GetSelectValueListByTbid(model.DbID, model.TbId);
                    ruleMultipleService.SetDefault(ref model);
                    model.Name = modelList.RuleName;
                    model.showDetails = model.showDetails == null ? "2" : model.showDetails;
                    model.DbID_Exa = ExternalDbService.GetName(model.DbID);
                    return View(model);
                }
                else
                {
                    return Json(ErrorTip("数据不存在！！！"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(RuleMultipleEntity model, string id)
        {
            string err = "";

            long total = comService.GetTotal("rulemultiplefiled", "where RuleId='" + model.RuleId + "'");
            if (total == 0)
            {
                return Json(ErrorTip("未添加列表字段"));
            }
            if (total > 10)
            {
                return Json(ErrorTip("列表字段最多不超过10列"));
            }

            try
            {
                model.LimitStr = model.LimitStr == null ? "" : BaseUtil.ReplaceHtml(model.LimitStr);
                err = ruleMultipleService.Edit(model);
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (err == "")
            {
                return Json(SuccessTip("编辑成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpPost]
        public ActionResult AddListFiledTemp(string id, string value, string text)
        {
            string err = "";
            text = BasicHelp.GetSplitStrByChar(text, " - ", 1);//获取分割后字符
            if (text.Length > 10)
            {
                text = text.Substring(0, 10);
            }
            value = Regex.Replace(value, "#table#", "", RegexOptions.IgnoreCase);

            try
            {
                err = ruleMultipleService.AddListFiledTemp(id, value, text);
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

        [HttpPost]
        public ActionResult AddListFiled(string id, string value, string text, string tbid)
        {
            string err = "";
            text = BasicHelp.GetSplitStrByChar(text, " - ", 1);//获取分割后字符
            if (text.Length > 15)
            {
                text = text.Substring(0, 15);
            }
            value = Regex.Replace(value, "#table#", "", RegexOptions.IgnoreCase);

            try
            {
                err = ruleMultipleService.AddListFiled(id, value, text, tbid);
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
        public ActionResult DeleteFiledTemp(string id)
        {
            string err = ruleMultipleService.DelListFiledTemp(id);

            if (err == "")
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult DeleteFiled(string rid, string indexid)
        {
            string err = ruleMultipleService.DelListFiled(rid, indexid);

            if (err == "")
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult GetListFiledTemp(string id)
        {
            var result = new { code = 0, msg = "", count = 999999, data = ruleMultipleService.GetListFiledTemp(id) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetListFiled(string id)
        {
            var result = new { code = 0, msg = "", count = 999999, data = ruleMultipleService.GetListFiled(id) };
            return Json(result);
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
                List<valueTextEntity> list = new List<valueTextEntity>();
                list.Add(new valueTextEntity { ClassID = "err", Value = "", Text = ex.Message });
                return Json(list);
            }
        }

        [HttpGet]
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
                List<valueTextEntity> list = new List<valueTextEntity>();
                list.Add(new valueTextEntity { ClassID = "err", Value = "", Text = ex.Message });
                return Json(list);
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
                List<valueTextEntity> list = new List<valueTextEntity>();
                list.Add(new valueTextEntity { ClassID = "err", Value = "", Text = ex.Message });
                return Json(list);
            }
        }

        [HttpGet]
        public JsonResult GetConditionFieldList(string id, string dbId)
        {
            try
            {
                if (dbId == "0")
                {
                    return Json(ruleService.GetConditionFieldListByTbid(id == null ? "" : id));
                }
                return Json(ruleService.GetConditionFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                List<valueTextEntity> list = new List<valueTextEntity>();
                list.Add(new valueTextEntity { ClassID = "err", Value = "", Text = ex.Message });
                return Json(list);
            }
        }
    }
}