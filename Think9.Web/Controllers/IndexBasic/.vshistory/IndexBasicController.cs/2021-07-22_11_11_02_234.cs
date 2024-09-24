using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using Think9.Model;
using Think9.Service.SystemOrganize;
using Think9.Service.SystemManage;
using Think9.Service.InTable;
using Think9.Web.Controllers;

namespace Think9.Web.Areas.SystemOrganize.Controllers
{
    public class IndexBasic : BaseController
    {
        //zzz
        public IndexBasic IndexBasicService = new IndexBasic();
        public UserService UserService = new UserService();

        public ItemsDetailService ItemsDetailService = new ItemsDetailService();

        public SelectList RoleTypeList { get { return new SelectList(ItemsDetailService.GetAll("Id,ItemName,ItemId", "ORDER BY SortCode ASC").Where(x => x.ItemId == 3), "Id", "ItemName"); } }

        // GET: Permissions/Role
        public override ActionResult Index(int? id)
        {
            base.Index(id);
            return View();
        }

        [HttpGet]
        public JsonResult List(RoleModel model, PageInfo pageInfo)
        {
            var result = IndexBasicService.GetListByFilter(model, pageInfo);
            return Json(result);
        }

        public ActionResult Add()
        {
            ViewBag.RoleTypeList = RoleTypeList;
            return View();
        }

        [HttpPost]
        public ActionResult Add(RoleModel model)
        {
            string where = "where EnCode=@EnCode";
            //string sql = string.Format("SELECT COUNT(1) FROM `sys_Role` where `EnCode` ='" + model.EnCode + "'", "Role");
            if (IndexBasicService.GetTotal(where, new { EnCode = model.EnCode}) > 0)
            {
                var result = ErrorTip("添加失败!已存在相同角色编码");
                return Json(result);
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.CreateUserId = Operator.UserId;
                model.UpdateTime = DateTime.Now;
                model.UpdateUserId = Operator.UserId;

                var result = IndexBasicService.Insert(model) ? SuccessTip("添加成功") : ErrorTip("添加失败");
                return Json(result);
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.RoleTypeList = RoleTypeList;
            var model = IndexBasicService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(RoleModel model)
        {
            model.UpdateTime = DateTime.Now;
            model.UpdateUserId = Operator.UserId;
            var result = IndexBasicService.UpdateById(model) ? SuccessTip("修改成功") : ErrorTip("修改失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            string where = "where RoleId=@RoleId";
            if (UserService.GetTotal(where, new { RoleId = id }) > 0)
            {
                var result = ErrorTip("不能删除!还存在该角色的用户");
                return Json(result);
            }
            else
            {
                var result = IndexBasicService.DeleteById(id) ? SuccessTip("删除成功") : ErrorTip("删除失败");
                return Json(result);
            }
        }

        public ActionResult Assign(int id)
        {
            ViewBag.RoleId = id;
            return View();
        }
    }
}