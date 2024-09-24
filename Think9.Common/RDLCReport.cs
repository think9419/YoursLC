using Microsoft.Reporting.NETCore;
using System;
using System.Data;
using System.IO;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class RDLCReport
    {
        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public static byte[] ExportPdf(ref string err, RdlcDeviceEntity device)
        {
            string width = "22cm";//pdf宽度 默认值22cm
            string heigh = "29.7cm";//pdf高度 默认值29.7cm
            string top = "1.5cm";//pdf边距
            string left = "0.5cm";//pdf边距
            string right = "0.5cm";//pdf边距
            string bottom = "1.5cm";//pdf边距
            if (device != null)
            {
                width = device.Width == null ? "22cm" : device.Width.ToString() + "cm";
                heigh = device.Heigh == null ? "29.7cm" : device.Heigh.ToString() + "cm";
                top = device.Top == null ? "1.5cm" : device.Top.ToString() + "cm";
                left = device.Left == null ? "0.5cm" : device.Left.ToString() + "cm";
                right = device.Right == null ? "0.5cm" : device.Right.ToString() + "cm";
                bottom = device.Bottom == null ? "1.5cm" : device.Bottom.ToString() + "cm";
            }

            var report = new LocalReport();
            try
            {
                using var fs = new FileStream(device.PathRdlc, FileMode.Open);
                report.LoadReportDefinition(fs);
                report.EnableExternalImages = true;
                report.EnableHyperlinks = true;
                report.DataSources.Add(new ReportDataSource("dbMain", device.MainDt));
                report.DataSources.Add(new ReportDataSource("dbGrid", device.GridDt));

                //可添加报表参数
                //var parameters = new[] { new ReportParameter("prm", "test") };
                //report.SetParameters(parameters);

                //pdf尺寸
                string deviceInfo =
                                 "<DeviceInfo>" +
                                 "  <OutputFormat>PDF</OutputFormat>" +
                                 "  <PageWidth>" + width + "</PageWidth>" +
                                 "  <PageHeight>" + heigh + "</PageHeight>" +
                                 "  <MarginTop>" + top + "</MarginTop>" +
                                 "  <MarginLeft>" + left + "</MarginLeft>" +
                                 "  <MarginRight>" + right + "</MarginRight>" +
                                 "  <MarginBottom>" + bottom + "</MarginBottom>" +
                                 "</DeviceInfo>";
                byte[] pdf = report.Render("PDF", deviceInfo);
                return pdf;
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        public static byte[] ExportXlsx(ref string err, RdlcDeviceEntity device)
        {
            var report = new LocalReport();

            try
            {
                using var fs = new FileStream(device.PathRdlc, FileMode.Open);
                report.LoadReportDefinition(fs);
                report.EnableExternalImages = true;
                report.EnableHyperlinks = true;
                report.DataSources.Add(new ReportDataSource("dbMain", device.MainDt));
                report.DataSources.Add(new ReportDataSource("dbGrid", device.GridDt));

                //可添加报表参数
                //var parameters = new[] { new ReportParameter("prm", "test") };
                //report.SetParameters(parameters);
                byte[] xlsx = report.Render("EXCELOPENXML");
                return xlsx;
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        public static byte[] ExportExcelList(ref string err, RdlcDeviceEntity device)
        {
            var report = new LocalReport();

            try
            {
                using var fs = new FileStream(device.PathRdlc, FileMode.Open);
                report.LoadReportDefinition(fs);
                report.EnableExternalImages = true;
                report.EnableHyperlinks = true;
                report.DataSources.Add(new ReportDataSource("dbMain", device.MainDt));
                report.DataSources.Add(new ReportDataSource("dbGrid", device.GridDt));

                //可添加报表参数
                //var parameters = new[] { new ReportParameter("prm", "test") };
                //report.SetParameters(parameters);
                byte[] xlsx = report.Render("EXCELOPENXML");
                return xlsx;
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        public static byte[] ExportHtml(ref string err, RdlcDeviceEntity device)
        {
            var report = new LocalReport();

            try
            {
                using var fs = new FileStream(device.PathRdlc, FileMode.Open);
                report.LoadReportDefinition(fs);
                report.EnableExternalImages = true;
                report.EnableHyperlinks = true;
                report.DataSources.Add(new ReportDataSource("dbMain", device.MainDt));
                report.DataSources.Add(new ReportDataSource("dbGrid", device.GridDt));

                //可添加报表参数
                //var parameters = new[] { new ReportParameter("prm", "test") };
                //report.SetParameters(parameters);
                byte[] xlsx = report.Render("HTML5");//HTML4.0/HTML5/MHTML(works on Windows, Linux and Mac OS)
                return xlsx;
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        public static byte[] ExportDocx(ref string err, RdlcDeviceEntity device)
        {
            var report = new LocalReport();

            try
            {
                using var fs = new FileStream(device.PathRdlc, FileMode.Open);
                report.LoadReportDefinition(fs);
                report.EnableExternalImages = true;
                report.EnableHyperlinks = true;
                report.DataSources.Add(new ReportDataSource("dbMain", device.MainDt));
                report.DataSources.Add(new ReportDataSource("dbGrid", device.GridDt));

                //可添加报表参数
                //var parameters = new[] { new ReportParameter("prm", "test") };
                //report.SetParameters(parameters);
                byte[] docx = report.Render("WORDOPENXML");
                return docx;
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="err"></param>
        /// <param name="dtMain">主表数据</param>
        /// <param name="dtGrid">子表数据</param>
        /// <param name="pathRdlc">rdl模板文件</param>
        /// <param name="pathUserImg">图片所在文件夹</param>
        /// <param name="imgNoExist">图片不存在时的替代</param>
        /// <param name="listid">数据ID</param>
        /// <param name="fwid">流程id</param>
        /// <param name="width">pdf宽度</param>
        /// <param name="heigh">pdf高度</param>
        /// <param name="top">pdf边距</param>
        /// <param name="left">pdf边距</param>
        /// <param name="right">pdf边距</param>
        /// <param name="bottom">pdf边距</param>
        /// <returns></returns>byte[]
        public static byte[] ExportPdf(ref string err, DataTable dtMain, DataTable dtGrid, string pathRdlc, string pathUserImg, string imgNoExist, string listid, string fwid, RdlcDeviceEntity device = null)
        {
            string width = "22cm";
            string heigh = "29.7cm";
            string top = "1.5cm";
            string left = "0.5cm";
            string right = "0.5cm";
            string bottom = "1.5cm";
            if (device != null)
            {
                width = device.Width == null ? "22cm" : device.Width.ToString() + "cm";
                heigh = device.Heigh == null ? "29.7cm" : device.Heigh.ToString() + "cm";
                top = device.Top == null ? "1.5cm" : device.Top.ToString() + "cm";
                left = device.Left == null ? "0.5cm" : device.Left.ToString() + "cm";
                right = device.Right == null ? "0.5cm" : device.Right.ToString() + "cm";
                bottom = device.Bottom == null ? "1.5cm" : device.Bottom.ToString() + "cm";
            }

            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            var report = new LocalReport();

            try
            {
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
                    string deviceInfo =
                                     "<DeviceInfo>" +
                                     "  <OutputFormat>PDF</OutputFormat>" +
                                     "  <PageWidth>" + width + "</PageWidth>" +
                                     "  <PageHeight>" + heigh + "</PageHeight>" +
                                     "  <MarginTop>" + top + "</MarginTop>" +
                                     "  <MarginLeft>" + left + "</MarginLeft>" +
                                     "  <MarginRight>" + right + "</MarginRight>" +
                                     "  <MarginBottom>" + bottom + "</MarginBottom>" +
                                     "</DeviceInfo>";
                    byte[] pdf = report.Render("PDF", deviceInfo);
                    return pdf;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="err"></param>
        /// <param name="dtMain">主表数据</param>
        /// <param name="dtGrid">子表数据</param>
        /// <param name="pathRdlc">rdl模板文件</param>
        /// <param name="pathUserImg">图片所在文件夹</param>
        /// <param name="imgNoExist">图片不存在时的替代</param>
        /// <param name="listid">数据ID</param>
        /// <param name="fwid">流程id</param>
        /// <param name="width">pdf宽度</param>
        /// <param name="heigh">pdf高度</param>
        /// <param name="top">pdf边距</param>
        /// <param name="left">pdf边距</param>
        /// <param name="right">pdf边距</param>
        /// <param name="bottom">pdf边距</param>
        /// <returns></returns>byte[]
        public static byte[] ExportPdf(ref string err, DataTable dtMain, DataTable dtGrid, string pathRdlc, string pathUserImg, string imgNoExist, RdlcDeviceEntity device = null)
        {
            var report = new LocalReport();

            string width = "22cm";
            string heigh = "29.7cm";
            string top = "1.5cm";
            string left = "0.5cm";
            string right = "0.5cm";
            string bottom = "1.5cm";
            if (device != null)
            {
                width = device.Width == null ? "22cm" : device.Width.ToString() + "cm";
                heigh = device.Heigh == null ? "29.7cm" : device.Heigh.ToString() + "cm";
                top = device.Top == null ? "1.5cm" : device.Top.ToString() + "cm";
                left = device.Left == null ? "0.5cm" : device.Left.ToString() + "cm";
                right = device.Right == null ? "0.5cm" : device.Right.ToString() + "cm";
                bottom = device.Bottom == null ? "1.5cm" : device.Bottom.ToString() + "cm";
            }

            try
            {
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
                    string deviceInfo =
                                     "<DeviceInfo>" +
                                     "  <OutputFormat>PDF</OutputFormat>" +
                                     "  <PageWidth>" + width + "</PageWidth>" +
                                     "  <PageHeight>" + heigh + "</PageHeight>" +
                                     "  <MarginTop>" + top + "</MarginTop>" +
                                     "  <MarginLeft>" + left + "</MarginLeft>" +
                                     "  <MarginRight>" + right + "</MarginRight>" +
                                     "  <MarginBottom>" + bottom + "</MarginBottom>" +
                                     "</DeviceInfo>";
                    byte[] pdf = report.Render("PDF", deviceInfo);
                    return pdf;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                err = BaseUtil.GetErrStr(ex);
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="err"></param>
        /// <param name="dtMain">主表数据</param>
        /// <param name="dtGrid">子表数据</param>
        /// <param name="pathRdlc">rdl模板文件</param>
        /// <param name="pathUserImg">图片所在文件夹</param>
        /// <param name="imgNoExist">图片不存在时的替代</param>
        /// <param name="listid">数据ID</param>
        /// <param name="fwid">流程id</param>
        /// <returns></returns>
        public static byte[] ExportDocx(ref string err, DataTable dtMain, DataTable dtGrid, string pathRdlc, string pathUserImg, string imgNoExist, string listid, string fwid)
        {
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            var report = new LocalReport();

            try
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
                return docx;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="err"></param>
        /// <param name="dtMain">主表数据</param>
        /// <param name="dtGrid">子表数据</param>
        /// <param name="pathRdlc">rdl模板文件</param>
        /// <param name="pathUserImg">图片所在文件夹</param>
        /// <param name="imgNoExist">图片不存在时的替代</param>
        /// <param name="listid">数据ID</param>
        /// <param name="fwid">流程id</param>
        /// <returns></returns>
        public static byte[] ExportXlsx(ref string err, DataTable dtMain, DataTable dtGrid, string pathRdlc, string pathUserImg, string imgNoExist, string listid, string fwid)
        {
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            var report = new LocalReport();

            try
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
                return xlsx;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return null;
            }
        }
    }
}