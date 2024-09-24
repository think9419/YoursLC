using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.IO;
using System.Linq;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbModelController : BaseController
    {
        private SortService sortService = new SortService();

        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public TbModelController()
        {
        }

        public override ActionResult Index(int? id)
        {
            ViewBag.SortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");
            return base.Index(id);
        }
    }
}