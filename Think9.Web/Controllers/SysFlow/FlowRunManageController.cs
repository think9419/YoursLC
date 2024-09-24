using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Com;

namespace Think9.Controllers.Basic
{
    [Area("SysFlow")]
    public class FlowRunManageController : BaseController
    {
        private Think9.Services.Com.FlowRunList runListService = new Think9.Services.Com.FlowRunList();
        private Think9.Services.Flow.FlowService flowService = new Think9.Services.Flow.FlowService();
        private ComService comService = new ComService();
        private Think9.Services.Table.TbBasicService tbBasicService = new Think9.Services.Table.TbBasicService();
        private string str;

        public override ActionResult Index(int? id)
        {
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            //用户可管理的录入表
            ViewBag.SelectFW = flowService.GetManageFlowListByUserId(userid);

            return View("List01");
        }

        [HttpGet]
        public ActionResult Next(string listid, string flid, string type)
        {
            string msg = "";
            string sql = "select * FROM flowrunlist where listid=" + listid + " ";
            DataTable dt = comService.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                msg = "编号为--" + dt.Rows[0]["ruNumber"].ToString() + "名称为--" + dt.Rows[0]["runName"].ToString() + "已交由您办理";
            }

            ViewBag.Msg = msg;
            ViewBag.ListId = listid;
            ViewBag.FwId = flid;

            if (type == "1")
            {
                return View("Next1");//固定
            }
            else
            {
                return View("Next2");//自由
            }
        }

        /// <summary>
        /// 返回数据列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageInfo"></param>
        /// <param name="fwid"></param>
        /// <param name="userid"></param>
        /// <param name="islock"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetManageList(FlowRunListEntity model, PageInfoEntity pageInfo, string fwid, string userid, string islock, string key)
        {
            if (CurrentUser == null)
            {
                return Json(new { msg = "<span style='color: #FE7300;'>超时 请重新登录</span>", count = -1 });
            }

            fwid = fwid == null ? "" : fwid;
            if (fwid == "" || fwid == "--")
            {
                return Json(new { msg = "<span style='color: #FE7300;'>请选择录入表 录入表管理/权限设置可设置用户管理权限</span>", count = -1 });
            }

            userid = userid == null ? "" : userid;
            key = key == null ? "" : key;
            if (fwid.StartsWith("bi_"))
            {
                long total = 0;
                List<FlowRunListEntity> list = runListService.GetManageDataList(ref total, pageInfo, CurrentUser, fwid, userid, islock, key);

                var result = new { code = 0, msg = "", count = total, data = list };
                return Json(result);
            }
            else
            {
                return Json(runListService.GetManageDataList(model, pageInfo, CurrentUser, fwid, userid, islock, key));
            }
        }

        public ActionResult GetManageFlowList(string listid, string fwid)
        {
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            var result = new { code = 0, msg = "", count = 999999, data = flowService.GetManageFlowListByUserId(userid) };
            return Json(result);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="idsStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchDel(string idsStr)
        {
            string id = "";
            int icount = 0;
            int ierr = 0;
            string tbid = "";
            string fwid = "";
            string islock = ""; //0未锁定1已锁定
            string[] arr = BaseUtil.GetStrArray(idsStr, "#");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    string[] arr2 = BaseUtil.GetStrArray(arr[i].ToString().Trim(), ";");//
                    if (arr2[0] != null && arr2[1] != null)
                    {
                        id = arr2[0].ToString().Trim();
                        fwid = arr2[1].ToString().Trim();
                        tbid = fwid.Replace("fw_", "tb_").Replace("bi_", "tb_");
                        if (fwid.StartsWith("bi_"))
                        {
                            islock = comService.GetSingleField("select isLock  FROM " + fwid.Replace("bi_", "tb_") + " WHERE listid= " + id);
                        }
                        else
                        {
                            islock = comService.GetSingleField("select isLock  FROM flowrunlist WHERE listid= " + id);
                        }

                        if (islock == "0")
                        {
                            AttachmentService.DelAttachment(long.Parse(id), fwid);//删除附件

                            //删除flowrunlist、flowrunprcslist表关联数据
                            if (fwid.StartsWith("fw_"))
                            {
                                comService.ExecuteSql("delete from flowrunlist where ListId = " + id + "");
                                comService.ExecuteSql("delete from flowrunprcslist where ListId = " + id + "");
                            }

                            //删除自动编号
                            Think9.Services.Com.AutoNo.DelAutoNo(id, fwid);

                            comService.ExecuteSql("delete from " + tbid + " where ListId = " + id + "");

                            foreach (TbBasicEntity obj in tbBasicService.GetByWhere("where ParentId='" + tbid + "'", null, "TbId"))
                            {
                                comService.ExecuteSql("delete from " + obj.TbId + " where ListId = " + id + "");
                            }
                            Record.Add(CurrentUser == null ? "!NullEx" : CurrentUser.Account, id, fwid, "#删除数据##数据管理页面删除#");

                            icount++;
                        }
                        else
                        {
                            ierr++;
                        }
                    }
                }
            }

            if (ierr == 0)
            {
                return Json(SuccessTip("删除成功"));
            }
            else
            {
                string show = "删除" + icount.ToString() + "数据 " + " 失败" + ierr.ToString() + "数据 已锁定数据请解锁后再删除 ";
                return Json(ErrorTip(show));
            }
        }

        [HttpPost]
        public JsonResult BatchLock(string idsStr, string flag)
        {
            string id = "";
            int icount = 0;
            int ierr = 0;
            string tbid = "";
            string fwid = "";
            string[] arr = BaseUtil.GetStrArray(idsStr, "#");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    string[] arr2 = BaseUtil.GetStrArray(arr[i].ToString().Trim(), ";");//
                    if (arr2[0] != null && arr2[1] != null)
                    {
                        id = arr2[0].ToString().Trim();
                        fwid = arr2[1].ToString().Trim();
                        tbid = fwid.Replace("fw_", "tb_").Replace("bi_", "tb_");

                        //删除flowrunlist、flowrunprcslist表关联数据
                        if (fwid.StartsWith("fw_"))
                        {
                            comService.ExecuteSql("update flowrunlist set isLock='" + flag + "' where ListId = " + id + "");
                        }
                        else
                        {
                            comService.ExecuteSql("update " + tbid + " set isLock='" + flag + "' where ListId = " + id + "");
                        }

                        str = "#锁定数据#";
                        if (flag == "0")
                        {
                            str = "#解锁数据#";
                        }
                        Record.Add(CurrentUser == null ? "!NullEx" : CurrentUser.Account, id, fwid, str);

                        icount++;
                    }
                }
            }

            if (ierr == 0)
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                string show = "操作失败" + ierr.ToString() + "数据 ";
                return Json(ErrorTip(show));
            }
        }
    }
}