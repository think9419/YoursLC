using Microsoft.AspNetCore.Mvc;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbIndexController : BaseController
    {
        private TbIndexService tbIndexService = new TbIndexService();

        public JsonResult GetTbIndexTips(string str)
        {
            return Json(TbIndexService.GetTbIndexExplain(str));
        }

        public JsonResult GetTbStatsFieldList(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tbIndexService.GetStatsFieldListByTb(tbid) };
            return Json(result);
        }
    }
}