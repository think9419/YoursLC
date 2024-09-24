using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class ImportDataController : BaseController
    {
        private Think9.Services.Com.ExcelData excelData = new Think9.Services.Com.ExcelData();
        private ComService comService = new ComService();

        public ActionResult ImportDataFromExcel(string tbid, string isDebug)
        {
            if (String.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }

            if (!comService.IsDataTableExists(tbid))
            {
                return Json(ErrorTip("数据表不存在！"));
            }

            ViewBag.IsDebug = String.IsNullOrEmpty(isDebug) ? "debug" : isDebug;
            ViewBag.tbid = tbid;
            ViewBag.fwid = comService.GetSingleField("select FlowId  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
            ViewBag.attId = "ImportData_" + System.Guid.NewGuid().ToString("N");
            return View();
        }

        public JsonResult GetColumnList(string tbid, string columns)
        {
            columns = string.IsNullOrEmpty(columns) ? "" : columns;
            List<InfoListEntity> list = excelData.GetColumnList(tbid, columns);

            var result = new { code = 0, msg = "", count = 999, data = list };
            return Json(result);
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ExcelUplaod(List<IFormFile> file)
        {
            string numColumns = "0";
            var filePath = "";
            string err = "";
            string fileName = "";
            string attid = System.Guid.NewGuid().ToString("N");
            UploadFileEntity uploadFile = new UploadFileEntity();

            try
            {
                foreach (var formFile in file)
                {
                    if (formFile.Length > 0)
                    {
                        fileName = attid + formFile.FileName;
                        string ext = System.IO.Path.GetExtension(fileName);

                        string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\TempFile\\");
                        filePath = Path.Combine(uploads, fileName);

                        //创建文件夹-创建前已经做了判断有则不创建
                        Think9.Util.Helper.FileHelper.CreateSuffic(uploads);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        err = "上传失败";
                    }
                }

                if (err == "")
                {
                    List<string> listHeader = new List<string>();

                    DataTable dtExcel = excelData.ExcelToTable(filePath, listHeader, ref numColumns);
                    if (dtExcel == null)
                    {
                        var result = new AjaxResult { state = ResultType.error.ToString(), message = "不能读取数据" };
                        return Json(result);
                    }

                    DataTable dtTop = excelData.DtSelectTop(100, dtExcel);

                    string table = excelData.GetGridTableHtml(dtTop, listHeader, int.Parse(numColumns));

                    uploadFile.code = 0;
                    uploadFile.src = attid;
                    uploadFile.msg = table;
                    uploadFile.filename = fileName;
                    string strHeader = "";
                    foreach (string str in listHeader)
                    {
                        strHeader += str + ",";
                    }
                    uploadFile.fileinfo = strHeader;
                    uploadFile.file_exa = "共有" + dtExcel.Rows.Count + "数据，最多显示100行";

                    return Json(uploadFile);
                }
                else
                {
                    var result = new AjaxResult { state = ResultType.error.ToString(), message = err };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { state = ResultType.error.ToString(), message = GetErr(ex) });
            }
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="fwid"></param>
        /// <param name="filename"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportData(string fwid, string filename, List<InfoListEntity> list)
        {
            string listidsStr = "";
            string show = "";
            string err = "";

            if (CurrentUser == null)
            {
                return Json(ErrorTip("当前用户对象为空"));
            }

            try
            {
                err = excelData.ImportDataFromExcel(ref listidsStr, ref show, fwid, filename, list, CurrentUser);

                if (!string.IsNullOrEmpty(err))
                {
                    return Json(ErrorTip(err));
                }
                else
                {
                    return Json(SuccessTip(show));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}