using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Controllers.Basic
{
    public class DevSetController : BaseController
    {
        // GET: SysSet/DevSet
        public override ActionResult Index(int? id)
        {
            return View(new DevEntity().GetDevInfo());
        }

        [HttpPost]
        public ActionResult Index(DevEntity model)
        {
            try
            {
                new DevEntity().SetDevInfo(model);
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Error:" + ex.Message;
                return View(new DevEntity().GetDevInfo());
            }
            ViewBag.Msg = "编辑成功！";
            return View(new DevEntity().GetDevInfo());
        }
    }
}