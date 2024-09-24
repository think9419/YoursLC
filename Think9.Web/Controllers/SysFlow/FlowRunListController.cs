using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Flow;

namespace Think9.Controllers.Basic
{
    [Area("SysFlow")]
    public class FlowRunListController : BaseController
    {
        private Think9.Services.Com.FlowRunList runListService = new Think9.Services.Com.FlowRunList();
        private Think9.Services.Flow.FlowService flowService = new Think9.Services.Flow.FlowService();
        private ComService comService = new ComService();
        private Think9.Services.Flow.FlowRunPrcsListService prcsListService = new Think9.Services.Flow.FlowRunPrcsListService();

        public override ActionResult Index(int? id)
        {
            ViewBag.SelectList = flowService.GetUseableFlowList();
            return View("List01");
        }

        //记录查看
        public ActionResult RecordList(string listid, string fwid, string mobile)
        {
            ViewBag.ListId = listid;
            ViewBag.FwId = fwid;
            mobile = string.IsNullOrEmpty(mobile) ? "n" : mobile.ToLower();
            if (mobile == "y")
            {
                return View("MobileListRecordList");
            }
            else
            {
                return View();
            }
        }

        //工作办理
        public ActionResult BeforeWorkHand(string fwid, string listid)
        {
            string err;
            string pid = "";
            CurrentUserEntity SelfUser = CurrentUser;
            if (SelfUser != null)
            {
                CurrentPrcsEntity mPrcs = Think9.Services.Com.FlowCom.GetCurrentStept(fwid, listid);
                pid = mPrcs == null ? "-1" : mPrcs.PrcsId;//当前流程步骤id
                err = mPrcs.ERR;
                if (string.IsNullOrEmpty(err))
                {
                    err = Think9.Services.Com.CheckCom.CheckedBeforeEdit(fwid, listid, mPrcs, SelfUser);
                    if (string.IsNullOrEmpty(err))
                    {
                        err = FlowCom.TakeOverPrcs(SelfUser, mPrcs);
                    }
                }
                else
                {
                    if (mPrcs.runFlag != "1")
                    {
                        err = "当前流程非待接手状态";
                    }
                }
            }
            else
            {
                err = "当前用户对象为空，请重新登录";
            }

            if (err == "")
            {
                fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
                string model = "1"; //模式？0不确定1发布模式2调试模式
                string str = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_").Replace("tb_", "");
                string str2 = "/" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_").Replace("tb_", "");
                if (comService.GetTotal("sys_module", "where UrlAddress='" + str + "' OR UrlAddress='" + str2 + "' ") > 0)
                {
                    model = "2"; //模式？0不确定1发布模式2调试模式
                }
                return Json(SuccessTip("", pid, model));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        /// <summary>
        /// 返回待接手
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageInfo"></param>
        /// <param name="flid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetToBeReceivedList(FlowRunListEntity model, PageInfoEntity pageInfo, string flid, string key)
        {
            if (CurrentUser == null)
            {
                return Json(new { msg = "<span style='color: #FE7300;'>超时 请重新登录</span>", count = -1 });
            }

            try
            {
                pageInfo.returnFields = "listid,ruNumber,FlowId,isLock,runName,createUser, isFinish,relatedId,flowType,currentPrcsId,runFlag,currentPrcsName,createTime";
                pageInfo.field = "listid";
                pageInfo.order = "desc";
                string where = FlowCom.GetToBeReceivedWhere(CurrentUser, flid, key);
                var result = runListService.GetPageByFilter(model, pageInfo, where);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        /// <summary>
        /// 返回办理中
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageInfo"></param>
        /// <param name="flid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetInProcessList(FlowRunListEntity model, PageInfoEntity pageInfo, string flid, string key)
        {
            pageInfo.returnFields = "listid, ruNumber,FlowId,isLock, runName, createUser, isFinish,relatedId,flowType,currentPrcsId,runFlag,currentPrcsName,createTime";
            pageInfo.field = "listid";
            pageInfo.order = "desc";
            try
            {
                string where = FlowCom.GetWorkNowWhere(CurrentUser, key, flid);
                var result = runListService.GetPageByFilter(model, pageInfo, where);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        //流程记录
        public ActionResult GetFlowRunPrcsList(string listid, string fwid)
        {
            try
            {
                IEnumerable<FlowRunPrcsListEntity> list = prcsListService.GetByWhere(" where listid=" + listid + " and FlowId='" + fwid + "' ", null, null, "order by id");
                var result = new { code = 0, msg = "", count = list.Count(), data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        //操作记录
        public ActionResult GetFlowRunRecord(string listid, string fwid)
        {
            try
            {
                IEnumerable<RecordRunEntity> list = DataTableHelp.ToEnumerable<RecordRunEntity>(comService.GetDataTable("select * from recordrun where FlowId='" + fwid + "' and listid = '" + listid + "' order by OperateTime desc"));
                var result = new { code = 0, msg = "", count = list.Count(), data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }
    }
}