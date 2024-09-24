using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class RoleController : BaseController
    {
        private RoleService roleService = new RoleService();
        private ComService comService = new ComService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public JsonResult List(RoleEntity model, PageInfoEntity pageInfo)
        {
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";
            var result = roleService.GetPageByFilter(model, pageInfo);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetRoleList()
        {
            var result = new { code = 0, count = 999999, data = roleService.GetByWhere() };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(RoleEntity model)
        {
            string where = "where EnCode=@EnCode";
            if (comService.GetTotal("sys_Role", where, new { EnCode = model.EnCode }) > 0)
            {
                return Json(ErrorTip("添加失败!已存在相同角色编码"));
            }
            else
            {
                model.UpdateTime = DateTime.Now;
                var result = roleService.Insert(model) ? SuccessTip("新建成功") : ErrorTip("新建失败");
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json("演示模式下不能编辑角色！");
            }
            else
            {
                var model = roleService.GetById(id);
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
        public ActionResult Edit(RoleEntity model)
        {
            model.UpdateTime = DateTime.Now;
            var result = roleService.UpdateById(model) ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json(ErrorTip("演示模式下不能删除角色！"));
            }
            else
            {
                string where = "where RoleId=@RoleId";
                if (comService.GetTotal("sys_users", where, new { RoleId = id }) > 0)
                {
                    return Json(ErrorTip("不能删除!还存在该角色的用户"));
                }
                else
                {
                    var result = roleService.DeleteById(id) ? SuccessTip("删除成功") : ErrorTip("删除失败");
                    comService.ExecuteSql("delete from sys_roleauthorize where RoleId=" + id + "");
                    return Json(result);
                }
            }
        }

        [HttpGet]
        public ActionResult Assign(int id)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json("演示模式下不能编辑角色！");
            }
            else
            {
                ViewBag.RoleId = id;
                return View();
            }
        }
    }
}