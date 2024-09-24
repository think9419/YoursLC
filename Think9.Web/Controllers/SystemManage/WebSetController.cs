using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Controllers.Basic
{
    public class WebSetController : BaseController
    {
        // GET: SysSet/WebSet
        public override ActionResult Index(int? id)
        {
            return View(new WebEntity().GetWebInfo());
        }

        [HttpPost]
        public ActionResult Index(WebEntity model)
        {
            try
            {
                new WebEntity().SetWebInfo(model);
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Error:" + ex.Message;
                return View(new WebEntity().GetWebInfo());
            }
            ViewBag.Msg = "编辑成功！";
            return View(new WebEntity().GetWebInfo());
        }
    }
}