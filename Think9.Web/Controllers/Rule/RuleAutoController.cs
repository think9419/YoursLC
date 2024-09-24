using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class RuleAutoController : BaseController
    {
        private ComService comService = new ComService();
        private RuleAutoService ruleAutoService = new RuleAutoService();
        private RuleListService ruleListService = new RuleListService();

        public ActionResult List(string rid)
        {
            string err = "";
            string name = "自动编号";

            RuleListEntity model = new RuleListEntity();
            model.RuleId = rid;

            //已经传过来一个id 如无则新增
            if (ruleListService.GetTotal("where RuleId=@RuleId", new { RuleId = model.RuleId }) == 0)
            {
                ruleListService.GetDefaultModel(ref model, "2", "自动编号");

                if (ruleListService.Insert(model))
                {
                    //生成自动编号数据建表语句
                    string sql = comService.GetCreatRuleAutoTbStr(model.RuleId);
                    try
                    {
                        comService.ExecuteSql(sql);
                    }
                    catch (Exception ex)
                    {
                        err = ex.Message;
                    }
                }
                else
                {
                    err = "新增失败";
                }
            }
            else
            {
                model = ruleListService.GetByWhereFirst("where RuleId=@RuleId", new { RuleId = rid });
                if (model != null)
                {
                    name = model.RuleName;
                }
                else
                {
                    err = "数据不存在";
                }
            }

            if (err == "")
            {
                ViewBag.RuleID = model.RuleId;
                ViewBag.RuleName = name;
                return View(model);
            }
            else
            {
                return Json(err);
            }
        }

        [HttpGet]
        public JsonResult GetList(string id, RuleAutoEntity model, PageInfoEntity pageInfo)
        {
            long total = 0;
            pageInfo.field = "AutoOrder";

            IEnumerable<dynamic> list
                = ruleAutoService.GetItemList(id, model, pageInfo, ref total);
            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpGet]
        public ActionResult AddItem(string rid)
        {
            RuleAutoEntity RuleAutoModel = new RuleAutoEntity();
            RuleAutoModel.RuleId = rid;

            return View(RuleAutoModel);
        }

        [HttpPost]
        public JsonResult AddItem(RuleAutoEntity model)
        {
            string err = ruleAutoService.Check(model);
            if (err == "")
            {
                var result = ruleAutoService.Insert(model) ? SuccessTip("添加成功") : ErrorTip("添加失败");
                return Json(result);
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            string where = "where AutoOrder=" + id + "";
            var result = ruleAutoService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("删除失败");
            return Json(result);
        }
    }
}