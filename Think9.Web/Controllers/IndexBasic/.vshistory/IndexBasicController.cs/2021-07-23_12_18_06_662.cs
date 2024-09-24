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
        public IndexBasicService IndexBasicService = new IndexBasicService();
        public UserService UserService = new UserService();

        public ItemsDetailService ItemsDetailService = new ItemsDetailService();

        public SortService SortService = new SortService();

        public SelectList SortList { get { return new SelectList(SortService.GetAll("SortID,SortName", "ORDER BY SortCode").Where(x => x.ClassID == "A01"), "SortID", "SortName"); } }

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