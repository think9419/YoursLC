using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Com;

namespace Think9.Controllers.Basic
{
    [Area("SysFlow")]
    public class PrcsNextController : BaseController
    {
        private FlowPrcsNextService flowPrcsNextService = new FlowPrcsNextService();
        private SmsService smsService = new SmsService();
        private ComService comService = new ComService();

        [HttpGet]
        public ActionResult Next(string listid, string flid)
        {
            string msg = "";
            string sql = "select * FROM flowrunlist where listid=" + listid + " ";
            DataTable dt = comService.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                msg = "编号为 - " + dt.Rows[0]["ruNumber"].ToString() + " 名称为 - " + dt.Rows[0]["runName"].ToString() + "已交由您办理";
            }
            ViewBag.Msg = msg;
            ViewBag.ListId = listid;
            ViewBag.FwId = flid;

            CurrentPrcsEntity mPrcs = Think9.Services.Com.FlowCom.GetCurrentStept(flid, listid);

            if (mPrcs.flowType == "1")
            {
                return View("~/Areas/SysFlow/PrcsNext/PrcsNext1.cshtml");//固定
            }
            else
            {
                return View("~/Areas/SysFlow/PrcsNext/PrcsNext2.cshtml");//自由
            }
        }

        //固定流程选择下一步骤
        [HttpPost]
        public ActionResult GetStepList(string listid, string flid)
        {
            listid = listid == null ? "0" : listid;
            var result = new { code = 0, msg = "", count = 999999, data = flowPrcsNextService.GetAllPrcsList(flid) };
            return Json(result);
        }

        //固定流程返回下一步骤
        [HttpPost]
        public ActionResult GetNextStepList(string listid, string flid)
        {
            listid = listid == null ? "0" : listid;
            var result = new { code = 0, msg = "", count = 999999, data = flowPrcsNextService.GetNextPrcsList(listid, flid) };
            return Json(result);
        }

        //固定流程返回回退步骤
        [HttpPost]
        public ActionResult GetBackStepList(string listid, string flid)
        {
            listid = listid == null ? "0" : listid;
            var result = new { code = 0, msg = "", count = 999999, data = flowPrcsNextService.GetBackPrcsList(listid, flid) };
            return Json(result);
        }

        //固定流程 转交
        [HttpPost]
        public ActionResult FlowPrcsNext01(string listid, string from, IEnumerable<valueTextEntity> list)
        {
            string err = "";
            string fwid = BasicHelp.GetValueFrmList(list, "classid", "fwid").Trim();
            string selectUser = BasicHelp.GetValueFrmList(list, "classid", "selectUserId").Trim();
            string nextId = BasicHelp.GetValueFrmList(list, "classid", "selectPrcsId").Trim();
            string msg = BasicHelp.GetValueFrmList(list, "classid", "msg").Trim();
            string remarks = BasicHelp.GetValueFrmList(list, "classid", "remarks").Trim();

            string isSelectUser = BasicHelp.GetValueFrmList(list, "classid", "ckSelectUser").Trim();
            if (isSelectUser == "true")
            {
                selectUser = "";
            }

            string ispublic = "2";
            if (BasicHelp.GetValueFrmList(list, "classid", "isPublic").Trim() == "true")
            {
                ispublic = "1";
            }

            try
            {
                err = FlowCom.SetFlowPrcsNext01(CurrentUser, listid, nextId, selectUser, remarks, ispublic, from);

                if (err == "")
                {
                    string account = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    //提醒下一步骤办理人
                    if (BasicHelp.GetValueFrmList(list, "classid", "ckmsg11").Trim() == "true")
                    {
                        smsService.AddSms(account, selectUser, msg, 99);
                    }
                    //提醒本流程的发起人
                    if (BasicHelp.GetValueFrmList(list, "classid", "ckmsg12").Trim() == "true")
                    {
                        smsService.SendSmsToCreateUser(account, listid, selectUser, 99);
                    }

                    if (string.IsNullOrEmpty(from))
                    {
                        Record.Add(account, listid.ToString(), fwid, "#流程转交#");
                    }
                    else
                    {
                        Record.Add(account, listid.ToString(), fwid, "#数据管理##流程转交#");
                    }

                    if (nextId == "0")
                    {
                        Record.Add("system", listid.ToString(), fwid, "#流程结束##锁定数据#");
                    }

                    return Json(SuccessTip("转交成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        //固定流程 回退
        [HttpPost]
        public ActionResult FlowPrcsBack01(string listid, IEnumerable<valueTextEntity> list)
        {
            string fwid = BasicHelp.GetValueFrmList(list, "classid", "fwid").Trim();
            string selectUserV = BasicHelp.GetValueFrmList(list, "classid", "selectUserId").Trim();
            string selectPrcsId = BasicHelp.GetValueFrmList(list, "classid", "selectPrcsId").Trim();
            string msg = BasicHelp.GetValueFrmList(list, "classid", "msg").Trim();
            string remarks = BasicHelp.GetValueFrmList(list, "classid", "remarks").Trim();
            string ispublic = "2";
            if (BasicHelp.GetValueFrmList(list, "classid", "isPublic").Trim() == "true")
            {
                ispublic = "1";
            }

            try
            {
                string err = FlowCom.SetFlowPrcsBack01(CurrentUser, listid, selectPrcsId, selectUserV, remarks, ispublic);
                if (err == "")
                {
                    string account = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    //提醒下一步骤办理人
                    if (BasicHelp.GetValueFrmList(list, "classid", "ckmsg21").Trim() == "true")
                    {
                        smsService.AddSms(account, selectUserV, msg, 99);
                    }
                    //提醒本流程的发起人
                    if (BasicHelp.GetValueFrmList(list, "classid", "ckmsg22").Trim() == "true")
                    {
                        smsService.SendSmsToCreateUser(account, listid, selectUserV, 99);
                    }

                    Record.Add(account, listid.ToString(), fwid, "#流程回退#");
                    return Json(SuccessTip("回退成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        //自由流程 转交
        [HttpPost]
        public ActionResult FlowPrcsNext02(string listid, string from, IEnumerable<valueTextEntity> list)
        {
            string err = "";
            string nextprcid = "";
            string selectUserV = BasicHelp.GetValueFrmList(list, "classid", "selectUserV").Trim();
            string msg = BasicHelp.GetValueFrmList(list, "classid", "msg01").Trim();
            string remarks = BasicHelp.GetValueFrmList(list, "classid", "remarks01").Trim();
            string fwid = BasicHelp.GetValueFrmList(list, "classid", "fwid").Trim();

            string ispublic = "2";
            if (BasicHelp.GetValueFrmList(list, "classid", "isPublic").Trim() == "true")
            {
                ispublic = "1";
            }

            if (string.IsNullOrEmpty(selectUserV))
            {
                err = "请选择办理人！";
            }
            else
            {
                if (selectUserV.Replace(";", "").Replace(" ", "") == "")
                {
                    err = "请选择办理人！";
                }
            }

            try
            {
                if (err == "")
                {
                    err = FlowCom.SetFlowPrcsNext02(CurrentUser, listid, nextprcid, selectUserV, remarks, ispublic, from);
                }

                if (err == "")
                {
                    string account = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    //提醒下一步骤办理人
                    if (BasicHelp.GetValueFrmList(list, "classid", "ckmsg11").Trim() == "true")
                    {
                        smsService.AddSms(account, selectUserV, msg, 99);
                    }
                    //提醒本流程的发起人
                    if (BasicHelp.GetValueFrmList(list, "classid", "ckmsg12").Trim() == "true")
                    {
                        smsService.SendSmsToCreateUser(account, listid, selectUserV, 99);
                    }

                    if (string.IsNullOrEmpty(from))
                    {
                        Record.Add(account, listid.ToString(), fwid, "#流程转交#");
                    }
                    else
                    {
                        Record.Add(account, listid.ToString(), fwid, "#数据管理##流程转交#");
                    }
                    return Json(SuccessTip("转交成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        //自由流程
        [HttpPost]
        public ActionResult FlowPrcsFinish(string listid, string from, IEnumerable<valueTextEntity> list)
        {
            string selectUserV = BasicHelp.GetValueFrmList(list, "classid", "selectUserV").Trim();
            string msg = BasicHelp.GetValueFrmList(list, "classid", "msg01").Trim();
            string remarks = BasicHelp.GetValueFrmList(list, "classid", "remarks01").Trim();
            string selectid = "0";
            string fwid = BasicHelp.GetValueFrmList(list, "classid", "fwid").Trim();

            string ispublic = "2";
            if (BasicHelp.GetValueFrmList(list, "classid", "isPublic").Trim() == "true")
            {
                ispublic = "1";
            }

            try
            {
                string err = FlowCom.SetFlowPrcsNext02(CurrentUser, listid, selectid, selectUserV, remarks, ispublic, from);
                if (err == "")
                {
                    Record.Add("system", listid.ToString(), fwid, "#流程结束##锁定数据#");
                    return Json(SuccessTip("转交成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}