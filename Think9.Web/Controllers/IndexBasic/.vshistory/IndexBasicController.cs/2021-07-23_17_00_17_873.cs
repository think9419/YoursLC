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

        public SortService SortService = new SortService();
        public IndexDtaeTypeService IndexDtaeTypeService = new IndexDtaeTypeService();

        public SelectList SortList { get { return new SelectList(SortService.GetAll("SortID,SortName", "ORDER BY SortCode").Where(x => x.ClassID == "A01"), "SortID", "SortName"); } }

        public SelectList IndexDtaeTypeList { get { return new SelectList(IndexDtaeTypeService.GetAll("TypeId,TypeName", "ORDER BY TypeOrder"), "TypeId", "TypeName"); } }

        // GET: Permissions/Role
        public override ActionResult Index(int? id)
        {
            base.Index(id);
            return View();
        }

        [HttpGet]
        public JsonResult List(IndexBasicModel model, PageInfo pageInfo)
        {
            var result = IndexBasicService.GetListByFilter(model, pageInfo);
            return Json(result);
        }

        public ActionResult Add()
        {
            ViewBag.SortList = SortList;
            ViewBag.DtaeTypeList = IndexDtaeTypeList;
            return View();
        }

    }
}