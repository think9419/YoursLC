using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Think9.Model;
using Think9.Service.SystemOrganize;
using Think9.Service.SystemManage;
using Think9.Web.Controllers;

namespace Think9.Web.Areas.SystemOrganize.Controllers
{
    public class ModuleController : BaseController
    {
        //public IModuleService ModuleService2 { get; set; }

        //zzz
        public ModuleService ModuleService = new ModuleService();

        // GET: Permissions/Module
        public override ActionResult Index(int? id)
        {
            base.Index(id);
            return View();
        }

        [HttpGet]
        public JsonResult List()
        {
            var list = ModuleService.GetAll();
            var result = new { code = 0, count = list.Count(), data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetModuleList()
        {
            object result = ModuleService.GetModuleList(Operator.RoleId);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetModuleTreeSelect()
        {
            var result = ModuleService.GetModuleTreeSelect();
            return Json(result);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(ModuleModel model)
        {
            model.FontFamily = "t9-icon";
            model.CreateTime = DateTime.Now;
            model.CreateUserId = Operator.UserId;
            model.UpdateTime = DateTime.Now;
            model.UpdateUserId = Operator.UserId;
            var result = ModuleService.Insert(model) ? SuccessTip("添加成功") : ErrorTip("添加失败");
            return Json(result);
        }

        public ActionResult Edit(int id)
        {
            var model = ModuleService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModuleModel model)
        {
            model.UpdateTime = DateTime.Now;
            model.UpdateUserId = Operator.UserId;
            var result = ModuleService.UpdateById(model) ? SuccessTip("修改成功") : ErrorTip("修改失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            var result = ModuleService.DeleteById(id) ? SuccessTip("删除成功") : ErrorTip("删除失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult ModuleButtonList(int roleId)
        {
            var list = ModuleService.GetModuleButtonList(roleId);
            var result = new { code = 0, count = list.Count(), data = list };
            return Json(result);
        }
    }
}