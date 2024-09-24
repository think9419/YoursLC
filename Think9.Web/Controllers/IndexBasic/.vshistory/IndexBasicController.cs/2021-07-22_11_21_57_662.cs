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
    public class IndexBasicController : BaseController
    {
        //zzz
        public IndexBasicService IndexBasicService = new IndexBasicService();
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
        public JsonResult List(IndexBsicModel model, PageInfo pageInfo)
        {
            var result = IndexBasicService.GetListByFilter(model, pageInfo);
            return Json(result);
        }

    }
}