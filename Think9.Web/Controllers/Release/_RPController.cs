using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Roslyn;
using Think9.Services.Base;

/// <summary>
/// 发布模式下调用
/// </summary>
namespace Think9.Controllers.Basic
{
    public class _RPController : BaseController
    {
        private ComService comService = new ComService();
        private GeneralRPController generalRP = new GeneralRPController();

        private CurrentUserEntity GetUser()
        {
            return GetCurrentUser();
        }

        private string GetErr(Exception ex)
        {
            string err = "";

            if (ex != null)
            {
                if (ex.InnerException != null)
                {
                    err += ex.InnerException.Message + " ";
                }
                err += ex.Message.ToString() + " ";
                err += ex.StackTrace.ToString() + " ";
            }

            return GetErr(err);
        }

        private string GetErr(string err)
        {
            return err.Replace("\u00601", "").Replace("\u0027", "").Replace("\r\n", "").Replace("\u0022", "").Replace("\u0026", "");
        }

        public override ActionResult Index(int? id)
        {
            string rpid = HttpContext.Request.Query["rpid"].ToString();
            CurrentUserEntity user = GetUser();
            try
            {
                return generalRP.List(id, rpid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", rpid, "_RPController.Index(...)→" + DebugApp.Trace, innerException);
                return Json(new { msg = GetErr(innerException, "_RPController.Index(...)→" + DebugApp.Trace), count = -1 });
            }
            catch (Exception ex)
            {
                Record.AddErr("", rpid, "_RPController.Index(...)→" + DebugApp.Trace, ex);
                return Json(new { msg = GetErr(ex, "_RPController.Index(...)→" + DebugApp.Trace), count = -1 });
            }
        }

        public ActionResult Show()
        {
            string rpid = HttpContext.Request.Query["rpid"].ToString();
            CurrentUserEntity user = GetUser();
            try
            {
                return generalRP.Show(rpid, user, HttpContext.Request);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", rpid, "_RPController.Index(...)→" + DebugApp.Trace, innerException);
                return Json(new { msg = GetErr(innerException, "_RPController.Index(...)→" + DebugApp.Trace), count = -1 });
            }
            catch (Exception ex)
            {
                Record.AddErr("", rpid, "_RPController.Index(...)→" + DebugApp.Trace, ex);
                return Json(new { msg = GetErr(ex, "_RPController.Index(...)→" + DebugApp.Trace), count = -1 });
            }
        }

        public ActionResult GetReport(string rpid, string type)
        {
            type = String.IsNullOrEmpty(type) ? "html" : type;
            CurrentUserEntity user = GetUser();
            try
            {
                List<ControlEntity> list = new List<ControlEntity>();
                string sql = @"SELECT  reportparmquery.ReportId, ReportParmQuery.ParmId, IndexParm.ParmName, IndexParm.DataType FROM  ReportParmQuery INNER JOIN IndexParm ON ReportParmQuery.ParmId = IndexParm.ParmId where ReportParmQuery.ReportId='" + rpid + "'";

                foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                {
                    list.Add(new ControlEntity { ControlID = "" + dr["ParmId"].ToString().Replace("@", "") + "", ControlValue = HttpContext.Request.Query[dr["ParmId"].ToString().Replace("@", "")].ToString() });
                }

                return generalRP.GetReport(rpid, type, list, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", rpid, "_RPController.GetReport(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_RPController.GetReport(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", rpid, "_RPController.Index(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_RPController.GetReport(...)→" + DebugApp.Trace));
            }
        }

        [HttpPost]
        public JsonResult GetPageList(string rpid, IEnumerable<ControlEntity> controlList, PageInfoEntity pageInfo)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalRP.GetPageList(rpid, controlList, pageInfo, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", rpid, "_RPController.GetPageList(...)→" + DebugApp.Trace, innerException);
                return Json(new { msg = GetErr(innerException, "_RPController.GetPageList(...)→" + DebugApp.Trace), count = -1 });
            }
            catch (Exception ex)
            {
                Record.AddErr("", rpid, "_RPController.GetPageList(...)→" + DebugApp.Trace, ex);
                return Json(new { msg = GetErr(ex, "_RPController.GetPageList(...)→" + DebugApp.Trace), count = -1 });
            }
        }

        [HttpPost]
        public JsonResult ExportData(string rpid, IEnumerable<ControlEntity> controlList, PageInfoEntity pageInfo)
        {
            string err = "";
            CurrentUserEntity user = GetUser();
            try
            {
                return generalRP.ExportData(rpid, controlList, pageInfo, GetHostUrl(HttpContext.Request));
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", rpid, "_RPController.ExportData(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_RPController.ExportData(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", rpid, "_RPController.Index(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_RPController.ExportData(...)→" + DebugApp.Trace));
            }
        }
    }
}