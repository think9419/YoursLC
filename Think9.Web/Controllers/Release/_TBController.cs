using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Think9.Controllers.Basic;
using Think9.Models;
using Think9.Roslyn;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Com;

/// <summary>
/// 发布模式下调用
/// </summary>

namespace Think9.Controllers.Basicprop
{
    public class _TBController : BaseController
    {
        private GeneralTB generalTB = new GeneralTB();
        private ComService comService = new ComService();
        private _TBService tBService = new _TBService();
        private readonly string _split = Think9.Services.Base.BaseConfig.ComSplit;//字符分割 用于多选项的分割等

        private CurrentUserEntity GetUser()
        {
            return GetCurrentUser();
        }

        private string GetUserId()
        {
            var user = GetUser();
            return user == null ? "!NullEx" : user.Account;
        }

        [HttpPost]
        public ActionResult ListButClick(string fwid, string idsStr, string frm, string btnId)
        {
            string err = "";
            try
            {
                List<string> resultList = generalTB.ListButClick(ref err, fwid, idsStr, frm, btnId, GetUser());
                Record.AddResultList("system", "", fwid, resultList, "List页面自定义按钮处理");
                if (!string.IsNullOrEmpty(err))
                {
                    return Json(ErrorTip("List页面自定义按钮处理：" + err));
                }
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.ListButClick(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.ListButClick(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.ListButClick(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.ListButClick(...)→" + DebugApp.Trace));
            }
            return Json(SuccessTip("操作成功"));
        }

        public ActionResult Add(string fwid, string btnId, string frm, string disableStr, string mobile)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.Add(fwid, frm, disableStr, mobile, user, HttpContext.Request);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.Add(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Add(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.Add(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Add(...)→" + DebugApp.Trace));
            }
        }

        public ActionResult Edit(string fwid, string btnId, string frm, string disableStr, string mobile)
        {
            CurrentUserEntity user = GetUser();
            //根据传递的参数（通常为业务主键）确定listid
            long listid = PageCom.GeListIdByHttpRequest(fwid.Replace("fw_", "tb_").Replace("bi_", "tb_"), HttpContext.Request);
            if (listid == 0)
            {
                return Json("根据传递的参数找不到数据");
            }
            if (listid == -100)
            {
                return Json("找到多个数据，依据传入的参数应能确定唯一数据");
            }
            try
            {
                return generalTB.Edit(fwid, listid, frm, disableStr, mobile, user, HttpContext.Request);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.Edit(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Edit(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.Edit(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Edit(...)→" + DebugApp.Trace));
            }
        }

        public ActionResult Show(string fwid, string btnId, string frm, string mobile, string type)
        {
            string err = "";
            //根据的传递参数（为业务主键）确定listid
            long listid = PageCom.GeListIdByHttpRequest(fwid.Replace("fw_", "tb_").Replace("bi_", "tb_"), HttpContext.Request);
            if (listid == 0)
            {
                return Json("根据传递的参数找不到数据");
            }
            if (listid == -100)
            {
                return Json("找到多个数据，依据传入的参数应能确定唯一数据");
            }
            type = string.IsNullOrEmpty(type) ? "html" : type;
            mobile = string.IsNullOrEmpty(mobile) ? "n" : mobile;
            if (mobile == "y")
            {
                ViewBag.ListId = listid;
                ViewBag.UserId = GetUserId();
                return View("~/Views/" + fwid.Replace("bi_", "").Replace("fw_", "") + "/MobileDetail.cshtml");
            }

            FileContentResult file = null;
            RdlcDeviceEntity device = tBService.GetRdlcDevice(GetUser(), listid.ToString(), fwid, comService.GetSingleField("select TbName  FROM  tbbasic  WHERE FlowId='" + fwid + "' "), GetHostUrl(HttpContext.Request));
            if (!string.IsNullOrEmpty(device.Err))
            {
                return Json(ErrorTip(device.Err));
            }
            if (type == "html" || type == "_html")
            {
                byte[] _byte = RDLCReport.ExportHtml(ref err, device);
                file = File(_byte, "text/html");
            }
            if (type == "pdf")
            {
                //录入表管理/前端设置/pdf文档尺寸可设置pdf文档大小，以适应不同文档和打印尺寸
                byte[] _byte = RDLCReport.ExportPdf(ref err, device);
                file = File(_byte, "application/pdf");
            }

            if (string.IsNullOrEmpty(err))
            {
                return file;
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        #region list列表

        /// <summary>
        /// 发布模式下点击左侧菜单触发，菜单链接路径为"_TB?tbid=表编码"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ActionResult Index(int? id)
        {
            string tbid = HttpContext.Request.Query["tbid"].ToString();

            string mobile = string.IsNullOrEmpty(HttpContext.Request.Query["mobile"]) ? "n" : HttpContext.Request.Query["mobile"].ToString().ToLower();

            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.List(id, tbid, mobile, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", tbid, "_TBController.Index(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Index(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", tbid, "_TBController.Index(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Index(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 添加前判断及处理，列表页面点击新增数据按钮触发
        /// </summary>
        /// <param name="type">类型add</param>
        /// <param name="listid">基础信息表自增长，一般录入表对应flowrunlist中的listid</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BeforeAdd(string type, string listid, string fwid)
        {
            long newid = 0;
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.BeforeAdd(ref newid, type, listid, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(newid.ToString(), fwid, "_TBController.BeforeAdd(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.BeforeAdd(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(newid.ToString(), fwid, "_TBController.BeforeAdd(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.BeforeAdd(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 编辑前判断及处理，列表页面点击编辑按钮触发
        /// </summary>
        /// <param name="type">类型edit</param>
        /// <param name="listid">基础信息表自增长，一般录入表对应flowrunlist中的listid</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BeforeEdit(string type, string listid, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.BeforeEdit(type, listid, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.BeforeEdit(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.BeforeEdit(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.BeforeEdit(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.BeforeEdit(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 数据查看-列表页面点击查看按钮触发
        /// </summary>
        /// <param name="listid">基础信息表自增长，一般录入表与flowrunlist中的listid对应关联</param>
        /// <param name="isOriginal">listid是否原始id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail(string listid, string fwid, string mobile)
        {
            mobile = string.IsNullOrEmpty(mobile) ? "n" : mobile;
            ViewBag.UserId = GetUserId();
            if (mobile == "y")//移动端
            {
                ViewBag.ListId = ListIdService.GetOriginalListId(listid).ToString();
                return View("~/Views/" + fwid.Replace("bi_", "").Replace("fw_", "") + "/MobileDetail.cshtml");
            }

            TbBasicEntity model = PageCom.GetDetailButon(listid, fwid);//按钮设置，可在录入表管理/页面按钮中设置
            ViewBag.ButPdf = model.ButPDFDetails;//Pdf是否显示 y显示
            ViewBag.ButDoc = model.ButDOCDetails;//DOC是否显示 y显示
            ViewBag.ButExcel = model.ButExcelDetails;//Excel是否显示 y显示
            ViewBag.ButAtt = model.ButAtt;//附件按钮是否显示 y显示
            ViewBag.ButAttTxt = model.ButAttTxt;//附件按钮标题
            ViewBag.ListId = listid;
            return View("~/Views/" + fwid.Replace("bi_", "").Replace("fw_", "") + "/Detail.cshtml");
        }

        /// <summary>
        /// 查看详细，详细页面点击按钮触发，将rdlc模板填充数据后转化为pdf等
        /// </summary>
        /// <param name="type">类别pdf、doc、excel</param>
        /// <param name="listid">基础信息表自增长，一般录入表对应flowrunlist中的listid</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDetail(string type, string listid, string fwid, string isOriginal)
        {
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();
            string err = "";
            FileContentResult file = null;
            string tbName = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE FlowId='" + fwid + "' ");
            RdlcDeviceEntity device = tBService.GetRdlcDevice(GetUser(), listid, fwid, tbName, GetHostUrl(HttpContext.Request));
            if (!string.IsNullOrEmpty(device.Err))
            {
                return Json(ErrorTip(device.Err));
            }

            try
            {
                if (type == "html" || type == "_html")
                {
                    byte[] _byte = RDLCReport.ExportHtml(ref err, device);
                    file = File(_byte, "text/html");
                }

                if (type == "pdf")
                {
                    //录入表管理/前端设置/pdf文档尺寸可设置pdf文档大小，以适应不同文档和打印尺寸
                    byte[] _byte = RDLCReport.ExportPdf(ref err, device);
                    file = File(_byte, "application/pdf");
                }

                if (type == "doc")
                {
                    byte[] _byte = RDLCReport.ExportDocx(ref err, device);
                    file = File(_byte, "application/msword", tbName + DateTime.Today.ToShortDateString() + ".docx");
                }

                if (type == "excel")
                {
                    byte[] _byte = RDLCReport.ExportXlsx(ref err, device);
                    file = File(_byte, "application/msexcel", tbName + DateTime.Today.ToShortDateString() + ".xlsx");
                }

                if (file == null)
                {
                    return Json(ErrorTip("参数错误"));
                }

                if (string.IsNullOrEmpty(err))
                {
                    return file;
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.GetDetail(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.GetDetail(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.GetDetail(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.GetDetail(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 查看详细，移动端列表页面点击详细按钮触发
        /// </summary>
        /// <param name="listid">基础信息表自增长，一般录入表对应flowrunlist中的listid</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMobileDetail(string listid, string fwid, string isOriginal)
        {
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            CurrentUserEntity user = GetUser();
            try
            {
                listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();
                return generalTB.GetMobileDetail(listid, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.GetMobileDetail(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.GetMobileDetail(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.GetMobileDetail(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.GetMobileDetail(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 点击打印按钮触发，将rdlc模板填充数据后转化pdf
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="listid">数据id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Print(string type, string listid, string fwid)
        {
            string err = "";
            try
            {
                RdlcDeviceEntity device = tBService.GetRdlcDevice(GetUser(), listid.ToString(), fwid, comService.GetSingleField("select TbName  FROM  tbbasic  WHERE FlowId='" + fwid + "' "), GetHostUrl(HttpContext.Request));
                if (!string.IsNullOrEmpty(device.Err))
                {
                    return Json(ErrorTip(device.Err));
                }
                byte[] _byte = RDLCReport.ExportPdf(ref err, device);
                FileContentResult file = File(_byte, "application/pdf");
                return file;
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.Print(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Print(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.Print(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Print(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 数据删除（可处理软删除），列表页面点击删除按钮触发
        /// </summary>
        /// <param name="listid">基础信息表自增长，一般录入表对应flowrunlist的listid</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Delete(string listid, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.Delete(listid, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.Delete(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Delete(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.Delete(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Delete(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 批量删除，列表页面点击批量删除按钮触发
        /// </summary>
        /// <param name="idsStr">listid逗号间隔</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchDel(string idsStr, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.BatchDel(idsStr, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.BatchDel(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.BatchDel(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.BatchDel(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.BatchDel(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 数据查询，列表页面点击查询按钮触发
        /// </summary>
        /// <param name="model">封装查询条件</param>
        /// <param name="pageInfo">页面信息，包括行数、排序等</param>
        /// <param name="searchTag">选项卡带出的查询字符</param>
        /// <param name="isAll">为all显示所有</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPageList(PageInfoEntity pageInfo, string fwid, string searchTag, string isAll, string json)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.GetPageList(pageInfo, fwid, searchTag, isAll, json, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.GetPageList(...)→" + DebugApp.Trace, innerException);
                return Json(new { msg = GetErr(innerException, "_TBController.GetPageList(...)→" + DebugApp.Trace), count = -1 });
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.GetPageList(...)→" + DebugApp.Trace, ex);
                return Json(new { msg = GetErr(ex, "_TBController.GetPageList(...)→" + DebugApp.Trace), count = -1 });
            }
        }

        /// <summary>
        /// 批量导出pdf，列表页面点击批量导出按钮触发
        /// </summary>
        /// <param name="idsStr">listid逗号间隔</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MergeExport(string idsStr, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.MergeExport(idsStr, fwid, user, GetHostUrl(HttpContext.Request));
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.MergeExport(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.MergeExport(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.MergeExport(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.MergeExport(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 数据导入，点击数据导入按钮触发
        /// </summary>
        /// <param name="fwid"></param>
        /// <param name="filename">文件名</param>
        /// <param name="list">源字段与目标字段的对应列表</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportData(string fwid, string filename, List<InfoListEntity> list)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.ImportData(fwid, filename, list, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.ImportData(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.ImportData(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.ImportData(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.ImportData(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 导出EXCEL,点击导出按钮触发
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportData(string fwid, string json, string searchTag)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.ExportData(searchTag, fwid, json, user, GetHostUrl(HttpContext.Request));
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.ExportData(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.ExportData(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.ExportData(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.ExportData(...)→" + DebugApp.Trace));
            }
        }

        #endregion list列表

        #region Form编辑

        /// <summary>
        /// 录入页面显示
        /// </summary>
        /// <param name="frm">从哪里跳转来?：List页面或其他页面跳转(默认list)</param>
        /// <param name="type">add或edit</param>
        /// <param name="listid">基础信息表自增长，一般录入表与flowrunlist中的listid对应关联</param>
        /// <param name="pid">当前流程步骤id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form(string frm, string type, string listid, string pid, string mobile, string fwid)
        {
            mobile = string.IsNullOrEmpty(mobile) ? "n" : mobile.ToLower();
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.Form(frm, type, listid, pid, fwid, mobile, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.Form(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Form(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.Form(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Form(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 下拉选择或者弹出选择后触发调用，完成数据读取功能
        /// 数据读取在录入表管理/数据读取中自定义
        /// </summary>
        /// <param name="controlslist">封装主表控件的list，包括id与value</param>
        /// <param name="gridlist">子表数据列表</param>
        /// <param name="id"></param>
        /// <param name="tbid">_main或子表编码</param>
        /// <param name="indexid">下拉或者弹出选择(触发控件)对应的指标编码</param>
        /// <param name="value">下拉或者弹出选择(触发控件)对应的控件Value</param>
        /// <returns>返回List<ControlEntity>前端再解析</returns>
        [HttpPost]
        public ActionResult AfterControlSelect(IEnumerable<ControlEntity> controlslist, IEnumerable<ControlEntity> gridlist, string id, string tbid, string indexid, string value, string fwid, string listid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.AfterControlSelect(controlslist, gridlist, id, tbid, indexid, value, fwid, listid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.AfterControlSelect(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.AfterControlSelect(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.AfterControlSelect(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.AfterControlSelect(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 数据保存，Form页面点击保存按钮触发
        /// </summary>
        /// <param name="model">主表数据</param>
        /// <param name="gridlist">子表数据</param>
        /// <param name="listid">listid=0表示增加</param>
        /// <param name="prcno">当前流程步骤编码</param>
        /// <param name="type">add或edit</param>
        /// <param name="att">附件id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveData(IEnumerable<GridListEntity> gridlist, long listid, string prcno, string type, string att, string fwid, string json)
        {
            string err = "";
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.SaveData(gridlist, listid, prcno, type, att, fwid, json, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.SaveData(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.SaveData(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.SaveData(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.SaveData(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 转交下一步--保存数据并流程转交，点击转交按钮触发
        /// </summary>
        /// <param name="model">主表数据model</param>
        /// <param name="gridlist">子表数据列表</param>
        /// <param name="listid">listid=0表示增加</param>
        /// <param name="prcno">当前流程步骤编码</param>
        /// <param name="type">add或edit</param>
        /// <param name="att">附件id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NextStep(IEnumerable<GridListEntity> gridlist, long listid, string prcno, string type, string att, string fwid, string json)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.NextStep(gridlist, listid, prcno, type, att, fwid, json, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.NextStep(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.NextStep(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.NextStep(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.NextStep(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 结束|提交--保存数据并结束流程，点击结束|提交按钮触发
        /// </summary>
        /// <param name="model">主表数据model</param>
        /// <param name="gridlist">子表数据</param>
        /// <param name="listid">listid=0表示增加</param>
        /// <param name="prcno">当前流程步骤编码</param>
        /// <param name="type">add或edit</param>
        /// <param name="att">附件id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Finish(IEnumerable<GridListEntity> gridlist, long listid, string prcno, string type, string att, string fwid, string json)
        {
            if (type == "add")
            {
                return Json(ErrorTip("首次保存不能提交|结束，请保存后再操作"));
            }

            prcno = "_finish";
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.Finish(gridlist, listid, prcno, type, att, fwid, json, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.Finish(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.Finish(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.Finish(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.Finish(...)→" + DebugApp.Trace));
            }
        }

        #endregion Form编辑

        #region 子表处理

        /// <summary>
        /// 子表获取数据，前端子表table.render调用此函数
        /// </summary>
        /// <param name="tbid">子表编码</param>
        /// <param name="listid">主表数据id</param>
        /// <param name="from">add或edit，用于控制子表指标编辑时锁定</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetGridList(string fwid, string tbid, long listid, string from, string mobile)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                mobile = string.IsNullOrEmpty(mobile) ? "" : mobile;
                return generalTB.GetGridList(fwid, tbid, listid, from, user, mobile);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.GetGridList(...)→" + DebugApp.Trace, innerException);
                var result = new { msg = GetErr(innerException, "_TBController.GetGridList(...)→" + DebugApp.Trace), count = -1 };
                return Json(result);
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.GetGridList(...)→" + DebugApp.Trace, ex);
                var result = new { msg = GetErr(ex, "_TBController.GetGridList(...)→" + DebugApp.Trace), count = -1 };
                return Json(result);
            }
        }

        /// <summary>
        /// 子表添加一条数据，点击子表列表右侧添加按钮触发
        /// </summary>
        /// <param name="controlslist">主表控件list,循环后得到model</param>
        /// <param name="list">子表数据列表（只取首行）</param>
        /// <param name="tbid">子表编码</param>
        /// <param name="listid">主表id：基础信息表对应主表listid，一般录入表对应表flowrunlist中的listid</param>
        [HttpPost]
        public ActionResult AddGrid(IEnumerable<GridListEntity> list, string fwid, string tbid, long listid, IEnumerable<ControlEntity> controlslist)
        {
            long newid = 0;
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.AddGrid(ref newid, list, fwid, tbid, listid, controlslist, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.AddGrid(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.AddGrid(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(newid.ToString(), fwid, "_TBController.AddGrid(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.AddGrid(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 子表插入空数据，点击子表列表上部新增行按钮触发
        /// </summary>
        /// <param name="controlslist">主表控件list,循环后得到model</param>
        /// <param name="grid">子表id</param>
        /// <param name="listid">主表数据id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddGridNull(IEnumerable<ControlEntity> controlslist, string fwid, string grid, long listid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.AddGridNull(controlslist, fwid, grid, listid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.AddGridNull(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.AddGridNull(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.AddGridNull(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.AddGridNull(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        ///子表删除一条数据，点击子表列表右侧删除按钮触发
        /// </summary>
        /// <param name="controlslist">主表控件list,循环后得到model</param>
        /// <param name="flag">#子表编码#id#行号#</param>
        /// <param name="listid">主表id：基础信息表对应主表listid，一般录入表对应表flowrunlist中的listid</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelGrid(IEnumerable<ControlEntity> controlslist, string flag, string listid, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.DelGrid(controlslist, flag, listid, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.DelGrid(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.DelGrid(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.DelGrid(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.DelGrid(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 批量删除子表数据，点击子表列表上部删除按钮触发
        /// </summary>
        /// <param name="controlslist">主表控件list,循环后得到model</param>
        /// <param name="listid">主表数据主键</param>
        /// <param name="tbid">子表id</param>
        /// <param name="idsStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchDelGrid(IEnumerable<ControlEntity> controlslist, string listid, string tbid, string idsStr, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.BatchDelGrid(controlslist, listid, tbid, idsStr, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.BatchDelGrid(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.BatchDelGrid(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.BatchDelGrid(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.BatchDelGrid(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 编辑子表数据，点击子表列表上部保存按钮触发
        /// </summary>
        /// <param name="controlslist">主表控件list,循环后得到model</param>
        /// <param name="gridlist">子表数据</param>
        /// <param name="grid">子表id</param>
        /// <param name="listid">主表数据id</param>
        /// <param name="prcno">当前流程步骤编码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditGrid(IEnumerable<ControlEntity> controlslist, IEnumerable<GridListEntity> gridlist, long listid, string fwid, string grid, string prcno)
        {
            string err = "";
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.EditGrid(controlslist, gridlist, listid, fwid, grid, prcno, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), fwid, "_TBController.EditGrid(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.EditGrid(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), fwid, "_TBController.EditGrid(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.EditGrid(...)→" + DebugApp.Trace));
            }
        }

        #endregion 子表处理

        #region 弹出处理

        /// <summary>
        /// 弹出数据选择页面
        /// </summary>
        /// <param name="tbid">_main或子表编码</param>
        /// <param name="indexid">触发弹出页面的指标编码</param>
        /// <param name="id">子表数据列表id或空</param>
        /// <param name="strv">子表哪一列触发如v1、v2</param>
        /// <param name="from">list或edit，list时会解除条件参数中包含的系统指标，如用户登录名等</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PopUpPage(string tbid, string indexid, string id, string strv, string from, string fwid)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.PopUpPage(tbid, indexid, id, strv, from, fwid, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.PopUpPage(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.PopUpPage(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.PopUpPage(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.PopUpPage(...)→" + DebugApp.Trace));
            }
        }

        /// <summary>
        /// 弹出页面获取数据列表
        /// </summary>
        /// <param name="type">为1表示当前页打开，要以search为关键词搜索</param>
        /// <param name="pageInfo">页面信息，包括行数、排序等</param>
        /// <param name="indexid">主表触发时为指标编码，子表触发时为子表编码+指标编码</param>
        /// <param name="list">封装查询条件</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPopUpPageList(PageInfoEntity pageInfo, string type, string fwid, string indexid, string from, string keyword, IEnumerable<valueTextEntity> list)
        {
            type = string.IsNullOrEmpty(type) ? "" : type;
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.GetPopUpPageList(pageInfo, type, keyword, fwid, indexid, from, list, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", fwid, "_TBController.GetPopUpPageList(...)→" + DebugApp.Trace, innerException);
                var result = new { msg = GetErr(innerException, "_TBController.GetPopUpPageList(...)→" + DebugApp.Trace), count = -1 };
                return Json(result);
            }
            catch (Exception ex)
            {
                Record.AddErr("", fwid, "_TBController.GetPopUpPageList(...)→" + DebugApp.Trace, ex);
                var result = new { msg = GetErr(ex, "_TBController.GetPopUpPageList(...)→" + DebugApp.Trace), count = -1 };
                return Json(result);
            }
        }

        /// <summary>
        /// 弹出数据选择页面点击选择确定后调用 默认什么也不做--可自定义
        /// </summary>
        /// <param name="tbid">_main或子表编码</param>
        /// <param name="indexid">触发弹出页面的指标编码，子表时为子表编码（去前缀）加指标编码</param>
        /// <param name="id">子表数据列表id或空</param>
        /// <param name="value">选择的值</param>
        /// <param name="v">子表弹出时使用 第几列</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AfterPopUpSelect(string tbid, string indexid, string id, string value, string v)
        {
            //默认什么也不做--可自定义
            if (tbid == "_main")//主表
            {
                //if (indexid == "xxx")
                //{
                //}
            }
            else//子表
            {
                //if (indexid == "xxx")
                //{
                //}
            }

            return Json("");
        }

        #endregion 弹出处理

        #region 移动端处理

        /// <summary>
        /// 子表录入页面显示，移动端点击新增子表数据或编辑数据时将被调用
        /// </summary>
        /// <param name="type">add或edit</param>
        /// <param name="listid">基础信息表自增长，一般录入表与flowrunlist中的listid对应关联</param>
        /// <param name="id">子表主键</param>
        /// <param name="tbid">子表编码</param>
        /// <param name="frm">页面状态add或edit，主要用于判断指标编辑时锁定</param>
        /// <returns></returns>
        public ActionResult GridForm(string fwid, string prcno, string type, string flag, string listid, string id, string grid, string frm)
        {
            if (type == "edit")
            {
                if (string.IsNullOrEmpty(flag))
                {
                    return Json(ErrorTip("参数错误"));
                }
                flag = flag.Replace("!", "#");
                BasicHelp.GetTbAndIdByFlag(flag, ref grid, ref id);//编辑时从flag获取grid和id
            }
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.GridForm(fwid, prcno, type, flag, listid, id, grid, frm, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr(listid.ToString(), grid, "_TBController.GridForm(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.GridForm(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr(listid.ToString(), grid, "_TBController.GridForm(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.GridForm(...)→" + DebugApp.Trace));
            }
        }

        public ActionResult SaveSingleGrid(string fwid, string type, string flag, IEnumerable<ControlEntity> controlsList, IEnumerable<ControlEntity> gridlist, long listid, string grid, string prcno)
        {
            CurrentUserEntity user = GetUser();
            try
            {
                return generalTB.SaveSingleGrid(fwid, type, flag, controlsList, gridlist, listid, grid, prcno, user);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                Record.AddErr("", grid, "_TBController.SaveSingleGrid(...)→" + DebugApp.Trace, innerException);
                return Json(ErrorTip(innerException, "_TBController.SaveSingleGrid(...)→" + DebugApp.Trace));
            }
            catch (Exception ex)
            {
                Record.AddErr("", grid, "_TBController.SaveSingleGrid(...)→" + DebugApp.Trace, ex);
                return Json(ErrorTip(ex, "_TBController.SaveSingleGrid(...)→" + DebugApp.Trace));
            }
        }

        #endregion 移动端处理
    }
}