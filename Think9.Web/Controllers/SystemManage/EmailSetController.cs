using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Controllers.Basic
{
    public class EmailSetController : BaseController
    {
        // GET: SysSet/EmailSet
        public override ActionResult Index(int? id)
        {
            return View(new EmailEntity().GetEmailInfo());
        }

        [HttpPost]
        public ActionResult Index(EmailEntity model)
        {
            try
            {
                new EmailEntity().SetEmailInfo(model);
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Error:" + ex.Message;
                return View(new EmailEntity().GetEmailInfo());
            }
            ViewBag.Msg = "编辑成功！";
            return View(new EmailEntity().GetEmailInfo());
        }
    }
}