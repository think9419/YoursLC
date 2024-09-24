using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;
using Think9.Models;
using Think9.Util;

namespace Think9.Services.Base
{
    public class BaseController : Controller
    {
        protected const string SuccessText = "操作成功！";
        protected const string ErrorText = "操作失败！";

        protected string Trace = "";

        public string GetHostUrl(HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host).ToString();
        }

        public JsonResult ChangeListId(string listid)
        {
            return Json(ListIdService.ChangeListId(long.Parse(listid)).ToString());
        }

        public CurrentUserEntity CurrentUser
        {
            get { return new OperatorProvider(HttpContext).GetCurrent(); }
        }

        public CurrentUserEntity GetCurrentUser()
        {
            return new OperatorProvider(HttpContext).GetCurrent();
        }

        public CurrentUserEntity GetTempUser()
        {
            CurrentUserEntity tempUser = new CurrentUserEntity();
            tempUser.UserId = -1;
            tempUser.Account = "!temp";
            tempUser.RealName = "!temp";
            tempUser.HeadIcon = "";
            tempUser.DeptNo = "top";
            tempUser.DeptNoStr = ";top;";
            tempUser.DeptName = "临时用户";
            tempUser.UpDeptNo = "top";
            tempUser.RoleName = "临时用户";
            tempUser.RoleNo = "!temp";
            tempUser.RoleId = -1;

            return tempUser;
        }

        // GET: Base
        public virtual ActionResult Index(int? id)
        {
            ComService comService = new ComService();
            int _menuId = MenuIdService.GetOriginalMenuId(id);

            int roleid = CurrentUser == null ? 0 : CurrentUser.RoleId;
            if (comService.GetDataTable("SELECT sys_module.Id FROM sys_roleauthorize INNER JOIN sys_module  ON sys_roleauthorize.ModuleId = sys_module.Id WHERE sys_roleauthorize.RoleId = " + roleid + " and sys_module.Id = " + _menuId + " ").Rows.Count > 0)
            {
                return View();
            }
            else
            {
                return View("~/Views/Login/LoginAgain.cshtml");
            }
        }

        public virtual ActionResult TableList(int? id, string tbid = "", string isMobile = "")
        {
            ComService comService = new ComService();
            int _menuId = MenuIdService.GetOriginalMenuId(id);

            int roleid = CurrentUser == null ? 0 : CurrentUser.RoleId;
            if (comService.GetDataTable("SELECT sys_module.Id FROM sys_roleauthorize INNER JOIN sys_module  ON sys_roleauthorize.ModuleId = sys_module.Id WHERE sys_roleauthorize.RoleId = " + roleid + " and sys_module.Id = " + _menuId + " ").Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(tbid))
                {
                    if (isMobile == "y")
                    {
                        return View("~/Views/" + tbid.Replace("tb_", "") + "/MobileList.cshtml");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return Json("参数错误");
                }
            }
            else
            {
                return View("~/Views/Login/LoginAgain.cshtml");
            }
        }

        public bool Check(int? id)
        {
            ComService comService = new ComService();
            int _menuId = MenuIdService.GetOriginalMenuId(id);
            int roleid = CurrentUser == null ? 0 : CurrentUser.RoleId;
            if (comService.GetDataTable(@"SELECT sys_module.Id FROM sys_roleauthorize INNER JOIN sys_module  ON sys_roleauthorize.ModuleId = sys_Module.Id WHERE sys_roleauthorize.RoleId = " + roleid + " and sys_Module.Id = " + _menuId + " ").Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Check(int? id, CurrentUserEntity user)
        {
            ComService comService = new ComService();
            int _menuId = MenuIdService.GetOriginalMenuId(id);
            int roleid = user == null ? 0 : user.RoleId;
            if (comService.GetDataTable(@"SELECT sys_module.Id FROM sys_roleauthorize INNER JOIN sys_module  ON sys_roleauthorize.ModuleId = sys_module.Id WHERE sys_roleauthorize.RoleId = " + roleid + " and sys_module.Id = " + _menuId + " ").Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult SuccessTip(string message = SuccessText)
        {
            return new AjaxResult { state = ResultType.success.ToString(), message = message };
        }

        /// <summary>
        /// 操作成功 并传递数值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult SuccessTip(string message, string extra)
        {
            return new AjaxResult { extra = extra, state = ResultType.success.ToString(), message = message };
        }

        /// <summary>
        /// 操作成功 并传递数值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult SuccessTip(string message, string extra, string extra2)
        {
            return new AjaxResult { extra = extra, extra2 = extra2, state = ResultType.success.ToString(), message = message };
        }

        /// <summary>
        /// 操作成功 并传递数值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult SuccessTip(string message, List<ControlEntity> list, string extra)
        {
            return new AjaxResult { extra = extra, list = list, state = ResultType.success.ToString(), message = message };
        }

        /// <summary>
        /// 操作成功 并传递数值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult SuccessTip(string message, List<ControlEntity> list, string extra, string extra2)
        {
            return new AjaxResult { extra = extra, extra2 = extra2, list = list, state = ResultType.success.ToString(), message = message };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult ErrorTip(string message = ErrorText)
        {
            return new AjaxResult { state = ResultType.error.ToString(), message = message };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult ErrorTip(Exception ex, string location = "")
        {
            string message = string.Empty;
            if (ex != null)
            {
                message = ex.GetType().Name;
                if (ex.GetType() == typeof(SystemException))
                {
                    message = "其他用户可处理的异常的基本类 ";
                }
                if (ex.GetType() == typeof(ArgumentNullException))
                {
                    message = "一个空参数传递给方法，该方法不能接受该参数 ";
                }
                if (ex.GetType() == typeof(ArgumentOutOfRangeException))
                {
                    message = "参数值超出范围 ";
                }
                if (ex.GetType() == typeof(ArithmeticException))
                {
                    message = "试图在数组中存储错误类型的对象 ";
                }
                if (ex.GetType() == typeof(BadImageFormatException))
                {
                    message = "图形的格式错误 ";
                }
                if (ex.GetType() == typeof(DllNotFoundException))
                {
                    message = "找不到引用的DLL ";
                }
                if (ex.GetType() == typeof(InvalidCastException))
                {
                    message = "使用无效的类 ";
                }
                if (ex.GetType() == typeof(InvalidOperationException))
                {
                    message = "方法的调用时间错误 ";
                }
                if (ex.GetType() == typeof(MissingMemberException))
                {
                    message = "访问一个无效版本的DLL ";
                }
                if (ex.GetType() == typeof(NotFiniteNumberException))
                {
                    message = "对象不是一个有效的成员 ";
                }
                if (ex.GetType() == typeof(NotSupportedException))
                {
                    message = "调用的方法在类中没有实现 ";
                }
                if (ex.GetType() == typeof(OutOfMemoryException))
                {
                    message = "内存空间不够 ";
                }
                if (ex.GetType() == typeof(PlatformNotSupportedException))
                {
                    message = "平台不支持某个特定属性时抛出该错误 ";
                }
                if (ex.GetType() == typeof(StackOverflowException))
                {
                    message = "堆栈溢出 ";
                }
                if (ex.GetType() == typeof(SEHException))
                {
                    message = "Win32结构异常处理信息的异常 ";
                }
                if (ex.GetType() == typeof(SqlException))
                {
                    message = "SQL操作异常 ";
                }

                if (ex.GetType() == typeof(DivideByZeroException))
                {
                    message = "尝试除以零 ";
                }
                if (ex.GetType() == typeof(MySqlException))
                {
                    message = "MySqlException异常 ";
                }
                if (ex.GetType() == typeof(OverflowException))
                {
                    message = "算术运算超出范围 ";
                }
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    message = "尝试使用空引用 ";
                }
                if (ex.GetType() == typeof(IndexOutOfRangeException))
                {
                    message = "尝试访问数组或集合的不存在的索引 ";
                }
                if (ex.GetType() == typeof(ArgumentException))
                {
                    message = "方法的参数是非法的 ";
                }
                if (ex.GetType() == typeof(FormatException))
                {
                    message = "参数格式错误 ";
                }
                if (ex.GetType() == typeof(System.IO.IOException))
                {
                    message = "I/O 操作失败 ";
                }

                if (!string.IsNullOrEmpty(location))
                {
                    message += "：" + location;
                }

                message += " " + ex.Message;
                message += Environment.NewLine;
                Exception originalException = ex.GetOriginalException();
                if (originalException != null)
                {
                    if (originalException.Message != ex.Message)
                    {
                        message += originalException.Message;
                        message += Environment.NewLine;
                    }
                }
            }

            return new AjaxResult { state = ResultType.error.ToString(), message = message };
        }

        protected string GetErr(Exception ex, string location = "")
        {
            string message = ex.GetType().Name;
            if (ex.GetType() == typeof(SystemException))
            {
                message = "其他用户可处理的异常的基本类 ";
            }
            if (ex.GetType() == typeof(ArgumentNullException))
            {
                message = "一个空参数传递给方法，该方法不能接受该参数 ";
            }
            if (ex.GetType() == typeof(ArgumentOutOfRangeException))
            {
                message = "参数值超出范围 ";
            }
            if (ex.GetType() == typeof(ArithmeticException))
            {
                message = "试图在数组中存储错误类型的对象 ";
            }
            if (ex.GetType() == typeof(BadImageFormatException))
            {
                message = "图形的格式错误 ";
            }
            if (ex.GetType() == typeof(DllNotFoundException))
            {
                message = "找不到引用的DLL ";
            }
            if (ex.GetType() == typeof(InvalidCastException))
            {
                message = "使用无效的类 ";
            }
            if (ex.GetType() == typeof(InvalidOperationException))
            {
                message = "方法的调用时间错误 ";
            }
            if (ex.GetType() == typeof(MissingMemberException))
            {
                message = "访问一个无效版本的DLL ";
            }
            if (ex.GetType() == typeof(NotFiniteNumberException))
            {
                message = "对象不是一个有效的成员 ";
            }
            if (ex.GetType() == typeof(NotSupportedException))
            {
                message = "调用的方法在类中没有实现 ";
            }
            if (ex.GetType() == typeof(OutOfMemoryException))
            {
                message = "内存空间不够 ";
            }
            if (ex.GetType() == typeof(PlatformNotSupportedException))
            {
                message = "平台不支持某个特定属性时抛出该错误 ";
            }
            if (ex.GetType() == typeof(StackOverflowException))
            {
                message = "堆栈溢出 ";
            }
            if (ex.GetType() == typeof(SEHException))
            {
                message = "Win32结构异常处理信息的异常 ";
            }
            if (ex.GetType() == typeof(SqlException))
            {
                message = "SQL操作异常 ";
            }

            if (ex.GetType() == typeof(DivideByZeroException))
            {
                message = "尝试除以零 ";
            }
            if (ex.GetType() == typeof(MySqlException))
            {
                message = "MySqlException异常 ";
            }
            if (ex.GetType() == typeof(OverflowException))
            {
                message = "算术运算超出范围 ";
            }
            if (ex.GetType() == typeof(NullReferenceException))
            {
                message = "尝试使用空引用 ";
            }
            if (ex.GetType() == typeof(IndexOutOfRangeException))
            {
                message = "尝试访问数组或集合的不存在的索引 ";
            }
            if (ex.GetType() == typeof(ArgumentException))
            {
                message = "方法的参数是非法的 ";
            }
            if (ex.GetType() == typeof(FormatException))
            {
                message = "参数格式错误 ";
            }
            if (ex.GetType() == typeof(System.IO.IOException))
            {
                message = "I/O 操作失败 ";
            }

            if (!string.IsNullOrEmpty(location))
            {
                message += "：" + location;
            }

            if (ex != null)
            {
                message += " " + ex.Message;
                message += Environment.NewLine;
                Exception originalException = ex.GetOriginalException();
                if (originalException != null)
                {
                    if (originalException.Message != ex.Message)
                    {
                        message += originalException.Message;
                        message += Environment.NewLine;
                        message += originalException.StackTrace;
                    }
                }
                //message += ex.StackTrace;
                //message += Environment.NewLine;
            }

            return message.Replace("\u00601", "").Replace("\u0027", "").Replace("\r\n", " ").Replace("\u0022", "").Replace("\u0026", "");
        }
    }
}