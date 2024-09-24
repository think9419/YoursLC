using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Util.Helper;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class UserController : BaseController
    {
        private UserService userService = new UserService();
        private ComService comService = new ComService();
        private OrganizeService organizeService = new OrganizeService();
        private RoleService roleService = new RoleService();

        public override ActionResult Index(int? id)
        {
            ViewBag.SelectList = organizeService.GetSelectTreeList();
            ViewBag.RoleList = new SelectList(roleService.GetAll("Id,FullName", "ORDER BY OrderNo ASC"), "Id", "FullName");
            ViewBag.Password = Think9.Services.Base.Configs.GetValue("InitUserPwd");//默认密码

            return base.Index(id);
        }

        [HttpPost]
        public JsonResult GetPageListBySearch(UserEntity model, PageInfoEntity pageInfo, string key, string dep, string enabled, string roleId)
        {
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string where = "where 1=1 ";
            if (!string.IsNullOrEmpty(key))
            {
                where += " and (Account like @Account OR RealName like @Account) ";
                model.Account = string.Format("%{0}%", key);
            }
            if (!string.IsNullOrEmpty(dep))
            {
                where += " and (DeptNo = @DeptNo) ";
                model.DeptNo = dep;
            }
            if (!string.IsNullOrEmpty(enabled))
            {
                where += " and (enabledMark = @EnabledMark) ";
                model.EnabledMark = int.Parse(enabled);
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                where += " and (roleId = @roleId) ";
                model.RoleId = int.Parse(roleId);
            }

            long total = 0;
            IEnumerable<dynamic> list = userService.GetPageByFilter(ref total, model, pageInfo, where);

            string sql = "select * from sys_organize";
            DataTable dt = comService.GetDataTable(sql);

            sql = "select * from sys_role";
            DataTable dtRole = comService.GetDataTable(sql);

            foreach (Object obj in list)
            {
                if (obj is UserEntity)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["EnCode"].ToString() == ((UserEntity)obj).DeptNo)
                        {
                            ((UserEntity)obj).DepartmentName = dr["FullName"].ToString();
                            break;
                        }
                    }

                    foreach (DataRow dr in dtRole.Rows)
                    {
                        if (dr["id"].ToString() == ((UserEntity)obj).RoleId.ToString())
                        {
                            ((UserEntity)obj).RoleName = dr["FullName"].ToString();
                            break;
                        }
                    }
                }
            }
            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        public ActionResult Add()
        {
            ViewBag.OrganizeList = new SelectList(organizeService.GetAll("Id,EnCode,FullName", "ORDER BY OrderNo ASC"), "EnCode", "FullName");
            ViewBag.RoleList = new SelectList(roleService.GetAll("Id,FullName", "ORDER BY OrderNo ASC"), "Id", "FullName");
            ViewBag.Password = Think9.Services.Base.Configs.GetValue("InitUserPwd");
            return View();
        }

        [HttpPost]
        public ActionResult Add(UserEntity model)
        {
            model.UserPassword = Md5.md5(Think9.Services.Base.Configs.GetValue("InitUserPwd"), 32);
            model.UpdateTime = DateTime.Now;
            model.EnabledMark = 0;
            if (comService.GetTotal("sys_users", "where Account='" + model.Account + "' ") > 0)
            {
                return Json(ErrorTip("已存在相同用户名"));
            }

            var result = userService.Insert(model) ? SuccessTip("操作成功") : ErrorTip("操作失败");
            return Json(result);
        }

        public ActionResult Edit(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json("演示模式下不能编辑用户！");
            }
            else
            {
                ViewBag.OrganizeList = new SelectList(organizeService.GetAll("Id,EnCode,FullName", "ORDER BY OrderNo ASC"), "EnCode", "FullName");
                ViewBag.RoleList = new SelectList(roleService.GetAll("Id,FullName", "ORDER BY OrderNo ASC"), "Id", "FullName");
                var model = userService.GetById(id);
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    return Json("数据不存在！");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(UserEntity model)
        {
            model.UpdateTime = DateTime.Now;
            string updateFields = "RealName,Gender,Birthday,MobilePhone,Email,WeChat,DeptNo,RoleId,EnabledMark,UpdateTime";
            var result = userService.UpdateById(model, updateFields) ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json("演示模式下不能删除用户！");
            }
            else
            {
                var result = userService.DeleteById(id) ? SuccessTip("删除成功") : ErrorTip("删除失败");
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult BatchDel(string idsStr)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json("演示模式下不能删除用户！");
            }
            else
            {
                var idsArray = idsStr.Substring(0, idsStr.Length - 1).Split(',');
                var result = userService.DeleteByIds(idsArray) ? SuccessTip("批量删除成功") : ErrorTip("批量删除失败");
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult InitPwd(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json(ErrorTip("演示模式下不能编辑密码！"));
            }
            else
            {
                var pwd = Think9.Services.Base.Configs.GetValue("InitUserPwd");
                var initPwd = Md5.md5(pwd, 32);
                UserEntity model = new UserEntity { Id = id, UserPassword = initPwd };
                var result = userService.UpdateById(model, "UserPassword") ? SuccessTip("重置密码成功，新密码:" + pwd) : ErrorTip("重置密码失败");
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult ModifyUserPwd(ModifyPwdEntity model)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json(ErrorTip("演示模式下不能编辑密码！"));
            }
            else
            {
                int userId = CurrentUser.UserId;
                var result = ErrorTip("出现异常，密码编辑失败");
                if (userService.LoginOn(model.UserName, Md5.md5(model.OldPassword, 32)) == null)
                {
                    result = ErrorTip("旧密码不正确");
                }
                else
                {
                    result = userService.ModifyUserPwd(model, userId) > 0 ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
                }
                return Json(result);
            }
        }
    }
}