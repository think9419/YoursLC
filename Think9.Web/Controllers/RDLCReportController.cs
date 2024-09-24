using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System;
using System.Data;
using System.IO;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Com;

namespace Think9.Controllers.Basic
{
    [Area("Com")]
    public class RDLCReportController : BaseController
    {
        private ComService comService = new ComService();

        public ActionResult ExportPdf(string listid, string fwid, string isOriginal, CurrentUserEntity user = null)
        {
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            if (string.IsNullOrEmpty(fwid) || string.IsNullOrEmpty(listid))
            {
                return Json("参数错误");
            }

            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();
            var report = new LocalReport();

            var pathRdlc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Reports\\" + tbid.Replace("tb_", "") + ".rdlc");
            string pathUserImg = Think9.Services.Base.BaseConfig.GetUserImgPath();
            string imgNoExist = Think9.Services.Base.BaseConfig.GetImgNoExistPath();

            try
            {
                DataTable dtMain = PageCom.GetMainTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);
                DataTable dtGrid = PageCom.GetGridTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);

                if (string.IsNullOrEmpty(err))
                {
                    using var fs = new FileStream(pathRdlc, FileMode.Open);
                    report.LoadReportDefinition(fs);
                    report.EnableExternalImages = true;
                    report.EnableHyperlinks = true;
                    report.DataSources.Add(new ReportDataSource("dbMain", dtMain));
                    report.DataSources.Add(new ReportDataSource("dbGrid", dtGrid));

                    //可添加报表参数
                    //var parameters = new[] { new ReportParameter("prm", "test") };
                    //report.SetParameters(parameters);

                    //pdf尺寸
                    string width = "22cm";
                    string heigh = "29.7cm";
                    string deviceInfo =
                                     "<DeviceInfo>" +
                                     "  <OutputFormat>PDF</OutputFormat>" +
                                     "  <PageWidth>" + width + "</PageWidth>" +
                                     "  <PageHeight>" + heigh + "</PageHeight>" +
                                     "  <MarginTop>1.5cm</MarginTop>" +
                                     "  <MarginLeft>0.5cm</MarginLeft>" +
                                     "  <MarginRight>0.5cm</MarginRight>" +
                                     "  <MarginBottom>1.5cm</MarginBottom>" +
                                     "</DeviceInfo>";
                    byte[] pdf = report.Render("PDF", deviceInfo);

                    return File(pdf, "application/pdf");
                }
                else
                {
                    return Json(err);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex.ToString()));
            }
        }

        public ActionResult ExportDocx(string listid, string fwid, string isOriginal, CurrentUserEntity user = null)
        {
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            if (string.IsNullOrEmpty(fwid) || string.IsNullOrEmpty(listid))
            {
                return Json("参数错误");
            }

            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();
            var report = new LocalReport();

            var pathRdlc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Reports\\" + tbid.Replace("tb_", "") + ".rdlc");
            string pathUserImg = Think9.Services.Base.BaseConfig.GetUserImgPath();
            string imgNoExist = Think9.Services.Base.BaseConfig.GetImgNoExistPath();

            try
            {
                DataTable dtMain = PageCom.GetMainTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);
                DataTable dtGrid = PageCom.GetGridTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);

                if (string.IsNullOrEmpty(err))
                {
                    using var fs = new FileStream(pathRdlc, FileMode.Open);
                    report.LoadReportDefinition(fs);
                    report.EnableExternalImages = true;
                    report.EnableHyperlinks = true;
                    report.DataSources.Add(new ReportDataSource("dbMain", dtMain));
                    report.DataSources.Add(new ReportDataSource("dbGrid", dtGrid));

                    //可添加报表参数
                    //var parameters = new[] { new ReportParameter("prm", "test") };
                    //report.SetParameters(parameters);

                    byte[] docx = report.Render("WORDOPENXML");
                    return File(docx, "application/msword", "Export.docx");
                }
                else
                {
                    return Json(err);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult ExportXlsx(string listid, string fwid, string isOriginal, CurrentUserEntity user = null)
        {
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            if (string.IsNullOrEmpty(fwid) || string.IsNullOrEmpty(listid))
            {
                return Json("参数错误");
            }

            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();
            var report = new LocalReport();

            var pathRdlc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Reports\\" + tbid.Replace("tb_", "") + ".rdlc");
            string pathUserImg = Think9.Services.Base.BaseConfig.GetUserImgPath();
            string imgNoExist = Think9.Services.Base.BaseConfig.GetImgNoExistPath();

            try
            {
                DataTable dtMain = PageCom.GetMainTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);
                DataTable dtGrid = PageCom.GetGridTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);

                if (string.IsNullOrEmpty(err))
                {
                    using var fs = new FileStream(pathRdlc, FileMode.Open);
                    report.LoadReportDefinition(fs);
                    report.EnableExternalImages = true;
                    report.EnableHyperlinks = true;
                    report.DataSources.Add(new ReportDataSource("dbMain", dtMain));
                    report.DataSources.Add(new ReportDataSource("dbGrid", dtGrid));

                    //可添加报表参数
                    //var parameters = new[] { new ReportParameter("prm", "test") };
                    //report.SetParameters(parameters);
                    byte[] xlsx = report.Render("EXCELOPENXML");
                    return File(xlsx, "application/msexcel", "Export.xlsx");
                }
                else
                {
                    return Json(err);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult ExportHtml(string listid, string fwid, string isOriginal, CurrentUserEntity user = null)
        {
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            if (string.IsNullOrEmpty(fwid) || string.IsNullOrEmpty(listid))
            {
                return Json("参数错误");
            }

            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();
            var report = new LocalReport();

            var pathRdlc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Reports\\" + tbid.Replace("tb_", "") + ".rdlc");
            string pathUserImg = Think9.Services.Base.BaseConfig.GetUserImgPath();
            string imgNoExist = Think9.Services.Base.BaseConfig.GetImgNoExistPath();

            try
            {
                DataTable dtMain = PageCom.GetMainTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);
                DataTable dtGrid = PageCom.GetGridTbDt(ref err, fwid, listid, GetHostUrl(HttpContext.Request), pathUserImg, imgNoExist, user);

                if (string.IsNullOrEmpty(err))
                {
                    using var fs = new FileStream(pathRdlc, FileMode.Open);
                    report.LoadReportDefinition(fs);
                    report.EnableExternalImages = true;
                    report.EnableHyperlinks = true;
                    report.DataSources.Add(new ReportDataSource("dbMain", dtMain));
                    report.DataSources.Add(new ReportDataSource("dbGrid", dtGrid));

                    //可添加报表参数
                    //var parameters = new[] { new ReportParameter("prm", "test") };
                    //report.SetParameters(parameters);

                    byte[] _byte = report.Render("HTML5");//HTML4.0/HTML5/MHTML(works on Windows, Linux and Mac OS)
                    return File(_byte, "text/html");
                }
                else
                {
                    return Json(err);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}