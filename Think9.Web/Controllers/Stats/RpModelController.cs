using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Linq;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class RpModelController : BaseController
    {
        private SortService sortService = new SortService();

        public override ActionResult Index(int? id)
        {
            ViewBag.SortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_report"), "SortID", "SortName");
            return base.Index(id);
        }
    }
}