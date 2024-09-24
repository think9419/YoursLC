using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Util.Helper;

namespace Think9.Controllers.Basic
{
    public class LoginController : Controller
    {
        private HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
        private UserService userService = new UserService();
        private OrganizeService organizeService = new OrganizeService();
        private RoleService roleService = new RoleService();
        private LogonLogService logService = new LogonLogService();

        private ExtraDbService extraDb = new ExtraDbService();

        private ILogger<LoginController> Logger { set; get; }

        // GET: Login
        public ActionResult Index()
        {
            string str = "V" + Assembly.GetAssembly(typeof(HomeController)).GetName().Version.Major.ToString() + "." + Assembly.GetAssembly(typeof(HomeController)).GetName().Version.Minor.ToString();
            if (Assembly.GetAssembly(typeof(HomeController)).GetName().Version.Build == 0)
            {
                ViewBag.Version = str;
            }
            else
            {
                ViewBag.Version = str + "." + Assembly.GetAssembly(typeof(HomeController)).GetName().Version.Build.ToString();
            }

            return View(new WebEntity().GetWebInfo());
        }

        [HttpGet]
        public ActionResult GetAuthCode()
        {
            try
            {
                return File(new VerifyCode(HttpContext).GetVerifyCode(), @"image/Gif");
            }
            catch (Exception e)
            {
                throw new Exception("获取验证码异常");
            }
        }

        [HttpPost]
        public ActionResult LoginOn(string username, string password, string captcha)
        {
            string ip = WebIp.Ip;

            LogonLogEntity logEntity = new LogonLogEntity();
            var OperatorProvider = new OperatorProvider(HttpContext);
            logEntity.LogType = DbLogType.Login.ToString();

            string err = "";
            try
            {
                if (OperatorProvider.WebHelper.GetSession("session_verifycode").IsEmpty() || Md5.md5(captcha.ToLower(), 16) != OperatorProvider.WebHelper.GetSession("session_verifycode"))
                {
                    throw new Exception("验证码错误");
                }
                UserEntity userEntity = userService.LoginOn(username, Md5.md5(password, 32));
                if (userEntity != null)
                {
                    if (userEntity.EnabledMark == 1)
                    {
                        throw new Exception("账号被禁用，请联系管理员");
                    }
                    CurrentUserEntity CurrentUser = new CurrentUserEntity();
                    CurrentUser.UserId = userEntity.Id;
                    CurrentUser.Account = userEntity.Account;
                    CurrentUser.RealName = userEntity.RealName;
                    CurrentUser.HeadIcon = userEntity.HeadIcon;
                    CurrentUser.RoleId = userEntity.RoleId;

                    //var ip = this.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    //if (string.IsNullOrEmpty(ip))
                    //{
                    //	ip = this.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    //}
                    //CurrentUser.LoginIPAddressName = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                    CurrentUser.LoginIPAddress = WebHelper.GetIP();
                    CurrentUser.LoginIPAddressName = WebHelper.GetstringIpAddress(CurrentUser.LoginIPAddress);

                    CurrentUser.DeptNo = userEntity.DeptNo;
                    CurrentUser.DeptNoStr = organizeService.GetDeptUpNOStr(userEntity.DeptNo);

                    var mOrganize = organizeService.GetByWhereFirst("where EnCode=@EnCode", new { EnCode = userEntity.DeptNo });
                    if (mOrganize != null)
                    {
                        CurrentUser.DeptName = mOrganize.FullName;
                        CurrentUser.UpDeptNo = mOrganize.ParentId;
                    }
                    else
                    {
                        CurrentUser.DeptName = "";
                        CurrentUser.UpDeptNo = "";
                        err = "用户所属的部门单位不存在";
                    }

                    var mRole = roleService.GetById(userEntity.RoleId);
                    if (mRole != null)
                    {
                        CurrentUser.RoleName = mRole.FullName;
                        CurrentUser.RoleNo = mRole.EnCode;
                        CurrentUser.RoleId = mRole.Id;
                    }
                    else
                    {
                        err = "用户所属的角色不存在";
                    }

                    if (string.IsNullOrEmpty(err))
                    {
                        OperatorProvider.AddCurrent(CurrentUser);
                        logEntity.Account = userEntity.Account;
                        logEntity.RealName = userEntity.RealName;
                        logEntity.Description = "登录成功";
                        logService.WriteDbLog(logEntity, CurrentUser.LoginIPAddress, CurrentUser.LoginIPAddressName);

                        return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功" }.ToJson());
                    }
                    else
                    {
                        throw new Exception(err);
                    }
                }
                else
                {
                    throw new Exception("用户名或密码错误");
                }
            }
            catch (Exception ex)
            {
                logEntity.Account = username;
                logEntity.RealName = username;
                logEntity.Description = "" + ex.Message;
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = ex.Message }.ToJson());
            }
        }

        [HttpGet]
        public ActionResult LoginOut()
        {
            var OperatorProvider = new OperatorProvider(HttpContext);
            logService.WriteDbLog(new LogonLogEntity
            {
                LogType = DbLogType.Exit.ToString(),
                Account = OperatorProvider.GetCurrent().Account,
                RealName = OperatorProvider.GetCurrent().RealName,
                Description = "安全退出系统",
            }, HttpContext.Connection.RemoteIpAddress.ToString(), HttpContext.Connection.RemoteIpAddress.ToString());
            OperatorProvider.WebHelper.ClearSession();
            OperatorProvider.RemoveCurrent();
            return RedirectToAction("LoginAgain", "Login");
        }

        [HttpGet]
        public ActionResult LoginAgain()
        {
            return View();
        }

        public ActionResult Error()
        {
            ViewData["StatusCode"] = HttpContext.Response.StatusCode;
            return View();
        }
    }
}