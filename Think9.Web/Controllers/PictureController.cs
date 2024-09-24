using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Think9.Models;

namespace Think9.Controllers.Basic
{
    public class PictureController : Controller
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Uplaod(List<IFormFile> file)
        {
            DataResult<string> rtnResult = new DataResult<string>();

            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    FileInfo fi = new FileInfo(formFile.FileName);
                    string ext = fi.Extension;
                    var orgFileName = fi.Name;
                    var newFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid() + ext;
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Resource");
                    var filePath = Path.Combine(uploads, newFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //await formFile.CopyToAsync(stream);
                    }
                    rtnResult.IsSuccess = true;
                }
                else
                {
                    rtnResult.ErrorMessage = "上传文件出错!";
                }
            }
            return Json(rtnResult);
        }
    }
}