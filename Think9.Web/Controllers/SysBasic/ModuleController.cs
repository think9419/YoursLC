using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class ModuleController : BaseController
    {
        private ModuleService moduleService = new ModuleService();
        private ComService comService = new ComService();
        private string split = Think9.Services.Base.BaseConfig.ComSplit;//字符分割 用于多选项的分割等

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        //左侧菜单 启动时调用
        [HttpGet]
        public JsonResult GetLeftTree(string mobile)
        {
            mobile = string.IsNullOrEmpty(mobile) ? "n" : mobile;
            if (CurrentUser != null)
            {
                object result = moduleService.GetModuleList(CurrentUser.RoleId, mobile);
                return Json(result);
            }
            else
            {
                return Json(null);
            }
        }

        [HttpGet]
        public JsonResult GetList()
        {
            string str = "";
            var list = moduleService.GetAll();

            DataTable dtRoleModule = comService.GetDataTable("select * from sys_roleauthorize");
            DataTable dtRole = comService.GetDataTable("select * from sys_role");
            foreach (ModuleEntity obj in list)
            {
                str = "";
                foreach (DataRow dr in dtRole.Rows)
                {
                    if (comService.GetTotal("sys_roleauthorize", "where RoleId=" + dr["id"].ToString().Trim() + " and ModuleId=" + obj.Id + "") > 0)
                    {
                        str += dr["FullName"].ToString().Trim() + " ";
                    }
                }
                obj.RoleStr = str;
            }

            var result = new { code = 0, count = list.Count(), data = list };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add()
        {
            DataTable dt = DataTableHelp.NewValueTextDt();
            string sql = "select * from sys_role";
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                DataRow row = dt.NewRow();
                row["ClassID"] = "sys_role";
                row["Value"] = dr["Id"].ToString();
                row["Text"] = dr["FullName"].ToString();
                dt.Rows.Add(row);
            }
            ViewBag.Split = split;//字符分割 checkbox多选时使用
            ViewBag.RoleList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
            ViewBag.SelectList = moduleService.GetSelectTreeList();
            ViewBag.Frm = "list";
            return View(new ModuleEntity());
        }

        [HttpPost]
        public ActionResult Add(ModuleEntity model)
        {
            int id = 0;
            model.FontFamily = "";
            model.UpdateTime = DateTime.Now;
            model.Icon = model.Icon == null ? "" : model.Icon;
            model.UrlAddress = model.UrlAddress == null ? "" : model.UrlAddress;

            id = moduleService.InsertReturnID(model);
            if (id > 0)
            {
                RoleAuthorizeService Service = new RoleAuthorizeService();
                if (!string.IsNullOrEmpty(model.RoleStr))
                {
                    string[] arr = BaseUtil.GetStrArray(model.RoleStr, split);//
                    for (int i = 0; i < arr.GetLength(0); i++)
                    {
                        if (arr[i] != null && arr[i].ToString().Trim() != "")
                        {
                            RoleAuthorizeEntity obj = new RoleAuthorizeEntity { ButtonId = 0, ModuleId = id, RoleId = int.Parse(arr[i].ToString().Trim()) };

                            Service.Insert(obj);
                        }
                    }
                }
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json("演示模式下不能编辑菜单！");
            }
            else
            {
                DataTable dt = DataTableHelp.NewValueTextDt();
                DataTable dtRole = comService.GetDataTable("select * from sys_role");
                foreach (DataRow dr in dtRole.Rows)
                {
                    DataRow row = dt.NewRow();
                    row["ClassID"] = "sys_role";
                    row["Value"] = dr["Id"].ToString();
                    row["Text"] = dr["FullName"].ToString();
                    dt.Rows.Add(row);
                }
                ViewBag.Split = split;//字符分割 checkbox多选时使用
                ViewBag.RoleList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
                ViewBag.SelectList = moduleService.GetSelectTreeList();
                ViewBag.Frm = "list";

                var model = moduleService.GetById(id);
                if (model != null)
                {
                    ViewBag.PId = model.ParentId;

                    string str = split;
                    foreach (DataRow dr in dtRole.Rows)
                    {
                        if (comService.GetTotal("sys_roleauthorize", "where RoleId=" + dr["id"].ToString().Trim() + " and ModuleId=" + id + "") > 0)
                        {
                            str += dr["id"].ToString().Trim() + split;
                        }
                    }
                    model.RoleStr = str;
                    return View(model);
                }
                else
                {
                    return Json("数据不存在！");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(ModuleEntity model)
        {
            model.UpdateTime = DateTime.Now;
            model.Icon = model.Icon == null ? "" : model.Icon;
            model.UrlAddress = model.UrlAddress == null ? "" : model.UrlAddress;
            string err = this.CheckParent(model.ParentId.ToString(), model.Id.ToString());
            if (err == "")
            {
                err = moduleService.UpdateById(model) ? "" : "编辑失败";
                if (err == "")
                {
                    RoleAuthorizeService Service = new RoleAuthorizeService();
                    comService.ExecuteSql("delete from sys_roleauthorize where ModuleId=" + model.Id + "");

                    if (!string.IsNullOrEmpty(model.RoleStr))
                    {
                        string[] arr = BaseUtil.GetStrArray(model.RoleStr, split);//
                        for (int i = 0; i < arr.GetLength(0); i++)
                        {
                            if (arr[i] != null && arr[i].ToString().Trim() != "")
                            {
                                RoleAuthorizeEntity obj = new RoleAuthorizeEntity { ButtonId = 0, ModuleId = model.Id, RoleId = int.Parse(arr[i].ToString().Trim()) };

                                Service.Insert(obj);
                            }
                        }
                    }
                }
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
        public JsonResult Delete(string id, string idsStr)
        {
            idsStr = string.IsNullOrEmpty(idsStr) ? "" : idsStr;
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json(ErrorTip("演示模式下不能删除菜单！"));
            }
            else
            {
                string err = "";
                string[] arr = BaseUtil.GetStrArray(idsStr, ".");//
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (arr[i] != null)
                    {
                        if (arr[i].ToString().Trim() != "")
                        {
                            string _id = arr[i].ToString().Trim();

                            moduleService.DeleteById(int.Parse(_id));
                            comService.ExecuteSql("delete from sys_roleauthorize where ModuleId=" + _id + "");
                        }
                    }
                }

                err = moduleService.DeleteById(int.Parse(id)) ? "" : "操作失败";
                if (err == "")
                {
                    comService.ExecuteSql("delete from sys_roleauthorize where ModuleId=" + id + "");
                }
                if (err == "")
                {
                    return Json(SuccessTip("删除成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
        }

        [HttpGet]
        public JsonResult BatchDel(string idsStr)
        {
            string id = "";
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json(ErrorTip("演示模式下不能删除菜单！"));
            }
            else
            {
                var idsArray = idsStr.Substring(0, idsStr.Length - 1).Split(',');
                string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (arr[i] != null)
                    {
                        if (arr[i].ToString().Trim() != "")
                        {
                            id = arr[i].ToString().Trim();
                        }
                    }
                }
                return Json(SuccessTip("删除成功"));
            }
        }

        [HttpGet]
        public JsonResult ModuleButtonList(int roleId)
        {
            var list = moduleService.GetModuleButtonList(roleId);
            var result = new { code = 0, count = list.Count(), data = list };
            return Json(result);
        }

        public ActionResult Icon()
        {
            return View();
        }

        private string CheckParent(string selectParentID, string id)
        {
            string err = "";
            if (selectParentID == id)
            {
                err = "不能选择自己作为上级！";
            }
            else
            {
                DataTable dtAll = comService.GetDataTable("select id,FullName as name,ParentId from sys_module order by OrderNo");

                string strAllUpDep = moduleService.GetHigherUpsIDStr(dtAll, selectParentID);

                if (strAllUpDep.Contains("." + id + "."))
                {
                    err = "上级菜单选择错误！";
                }
            }
            return err;
        }

        public ActionResult GetLowerLevelStr(string id)
        {
            string lowerLevel = ".";
            DataTable dtAll = comService.GetDataTable("select * from sys_module");
            moduleService.GetLowerLevel(dtAll, id, ref lowerLevel);
            if (lowerLevel == ".")
            {
                lowerLevel = "";
            }
            return Json(SuccessTip("", lowerLevel));
        }
    }
}