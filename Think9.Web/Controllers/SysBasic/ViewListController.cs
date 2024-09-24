using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class ViewListController : BaseController
    {
        private ComService comService = new ComService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public JsonResult GetList(PageInfoEntity pageInfo)
        {
            List<valueTextEntity> list = new List<valueTextEntity>();
            DataTable dbView = comService.GetViewsList();//
            foreach (DataRow dr in dbView.Rows)
            {
                list.Add(new valueTextEntity { Value = "" + dr["Name"].ToString(), Text = dr["Create_time"].ToString() });
            }
            var result = new { code = 0, msg = "", count = 9999999, data = list };
            return Json(result);
        }
    }
}