using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Flow;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbValueCheckController : BaseController
    {
        private TbValueCheck tbValueCheckService = new TbValueCheck();
        private TbBasicService tbBasicService = new TbBasicService();
        private FlowService flowService = new FlowService();

        public ActionResult List(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        //选择适用范围
        [HttpGet]
        public ActionResult PUflow(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }

        //打开校验页面
        [HttpGet]
        public ActionResult PUFormula(string tbid, string fid, string some)
        {
            ViewBag.tbid = tbid;
            ViewBag.formid = fid;
            ViewBag.some = some;
            return View();
        }

        [HttpGet]
        public JsonResult GetList(TbValueCheckEntity model, PageInfoEntity pageInfo, string tbid)
        {
            pageInfo.field = "IOrder";
            var result = tbValueCheckService.GetPageByFilter(model, pageInfo, "where TbId='" + tbid + "'");
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetSelectFlowList(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = flowService.GetUseableFlowListForTbValueCheck(tbid) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetSelectIndexList(string tbid, string some)
        {
            some = some == null ? "" : some;
            var result = new { code = 0, msg = "", count = 999999, data = tbValueCheckService.GetSelectIndexList(tbid, some) };

            return Json(result);
        }

        [HttpGet]
        public ActionResult Add(string tbid)
        {
            ViewBag.tbid = tbid;
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model != null)
            {
                ViewBag.tbname = model.TbName;
                return View();
            }
            else
            {
                return Json("错误：当前录入表对象为空");
            }
        }

        [HttpPost]
        public ActionResult Add(TbValueCheckEntity model, string tbid)
        {
            model.isUse = "1";
            model.LeftValue = model.LeftValue == null ? "" : BaseUtil.ReplaceHtml(model.LeftValue);
            model.RightValue = model.RightValue == null ? "" : BaseUtil.ReplaceHtml(model.RightValue);
            var result = tbValueCheckService.Insert(model) ? SuccessTip("操作成功") : ErrorTip("操作失败");
            return Json(result);
        }

        [HttpGet]
        public ActionResult Edit(string tbid, string id)
        {
            ViewBag.tbid = tbid;
            ViewBag.id = id;

            string err = "";
            TbValueCheckEntity model = tbValueCheckService.GetByWhereFirst("where Id=" + id + "");
            TbBasicEntity mTb = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (mTb != null)
            {
                if (model != null)
                {
                    //model.LeftValue = model.LeftValue == null ? "" : BaseUtil.ReplaceHtml(model.LeftValue);
                    //model.RightValue = model.RightValue == null ? "" : BaseUtil.ReplaceHtml(model.RightValue);
                    model.TbNmae = mTb.TbName;
                    model.FlowStr_Exa = flowService.GetFlowStrForTbValueCheck(tbid, model.FlowStr);
                }
                else
                {
                    err = "错误：当前对象为空";
                }
            }
            else
            {
                err = "错误：当前录入表对象为空";
            }

            if (string.IsNullOrEmpty(err))
            {
                return View(model);
            }
            else
            {
                return Json(err);
            }
        }

        [HttpPost]
        public ActionResult Edit(TbValueCheckEntity model, string id)
        {
            try
            {
                string where = "where id=" + id + "";
                string filed = "LeftValue,Compare,RightValue,Explain,NullCase,IOrder,isUse,FlowStr";
                model.LeftValue = model.LeftValue == null ? "" : BaseUtil.ReplaceHtml(model.LeftValue);
                model.RightValue = model.RightValue == null ? "" : BaseUtil.ReplaceHtml(model.RightValue);
                var result = tbValueCheckService.UpdateByWhere(where, filed, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult Delete(TbValueCheckEntity model, string id)
        {
            string where = "where id=" + id + "";
            var result = tbValueCheckService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("操作失败");
            return Json(result);
        }
    }
}