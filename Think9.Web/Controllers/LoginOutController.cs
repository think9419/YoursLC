using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    public class LoginOutController : Controller
    {
        private LogonLogService logService = new LogonLogService();

        public ActionResult Index()
        {
            try
            {
                return View("~/Views/Login/Index.cshtml");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}