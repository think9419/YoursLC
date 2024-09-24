using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    public class HomeController : BaseController
    {
        private UserService userService = new UserService();
        private NotifyService notifyService = new NotifyService();
        private SmsService smsService = new SmsService();
        private ComService comService = new ComService();
        public IWebHostEnvironment webHostEnvironment { get; set; }

        public override ActionResult Index(int? id)
        {
            string mobile = HttpContext.Request.Query["M"].ToString();
            string isMobile = "n";
            if (!string.IsNullOrEmpty(mobile))
            {
                isMobile = mobile.ToLower();
            }
            ViewBag.Account = CurrentUser == null ? "" : CurrentUser.Account;
            ViewBag.HeadIcon = CurrentUser == null ? "" : CurrentUser.HeadIcon;

            if (isMobile == "y")
            {
                return View("~/Views/Home/MobileIndex.cshtml", new WebEntity().GetWebInfo());
            }
            else
            {
                return View(new WebEntity().GetWebInfo());
            }
        }

        public ActionResult Mobile()
        {
            ViewBag.Account = CurrentUser == null ? "" : CurrentUser.Account;
            ViewBag.HeadIcon = CurrentUser == null ? "" : CurrentUser.HeadIcon;

            return View("~/Views/Home/MobileIndex.cshtml", new WebEntity().GetWebInfo());
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Message()
        {
            return View();
        }

        public ActionResult UserInfo()
        {
            if (CurrentUser == null)
            {
                return Json("<span style='color: #FE7300;'>超时 请重新登录</span>");
            }
            else
            {
                int userId = CurrentUser.UserId;
                var model = userService.GetById(userId);
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    return Json("数据不存在");
                }
            }
        }

        [HttpPost]
        public ActionResult EditUser(UserEntity model, int id)
        {
            model.UpdateTime = DateTime.Now;
            model.Id = id;
            string updateFields = "RealName,Gender,Birthday,MobilePhone,Email";
            var result = userService.UpdateById(model, updateFields) ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult ReadSms(int id)
        {
            var result = smsService.ReadSms(id) ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult ReadNotice(int id)
        {
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            userid = ";" + userid + ";";
            comService.ExecuteSql("update notify set Readers= Readers + '" + userid + "'   WHERE NotifyId= " + id);

            return Json("");
        }

        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNoticeList()
        {
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            userid = ";" + userid + ";";
            string content = "";
            string attachment = "";
            DataTable dt = DataTableHelp.NewSmsDt();

            NotifyEntity model = new NotifyEntity();
            model.Readers = string.Format("%{0}%", userid);

            IEnumerable<dynamic> list = notifyService.GetByWhere("where Readers not like @Readers", model, null, "order by publishTime desc ");

            foreach (NotifyEntity obj in list)
            {
                DataRow row = dt.NewRow();
                row["SmsId"] = obj.NotifyId.ToString();
                row["Type"] = 0;
                row["FromId"] = obj.FromId;
                row["Subject"] = obj.Subject;
                row["createTime"] = obj.publishTime.ToString();
                row["Subject"] = obj.Subject;
                content = obj.Content;
                attachment = "";
                if (!string.IsNullOrEmpty(obj.attachmentId))
                {
                    foreach (DataRow dr in comService.GetDataTable("select * from flowattachment where AttachmentId='" + obj.attachmentId + "'").Rows)
                    {
                        attachment += "<a style='color: #FE7300;text-decoration: underline;' href='/UserFile/" + dr["AttachmentId"].ToString() + "/" + dr["FullName"].ToString() + "' target='_blank'>附件：" + dr["FullName"].ToString() + "</a>";
                    }
                }

                row["Content"] = content;
                row["Attachment"] = attachment;
                dt.Rows.Add(row);
            }
            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(DataTableHelp.ToEnumerable<SmsEntity>(dt));
        }

        public ActionResult UserPwd()
        {
            ViewBag.UserName = CurrentUser.Account;
            return View();
        }

        public JsonResult ExportFile()
        {
            UploadFileEntity uploadFile = new UploadFileEntity();
            try
            {
                var file = Request.Form.Files[0];    //获取选中文件
                var filecombin = file.FileName.Split('.');
                if (file == null || string.IsNullOrEmpty(file.FileName) || file.Length == 0 || filecombin.Length < 2)
                {
                    uploadFile.code = -1;
                    uploadFile.src = "";
                    uploadFile.msg = "上传出错!请检查文件名或文件内容";
                    return Json(uploadFile);
                }
                //定义本地路径位置
                string localPath = webHostEnvironment.WebRootPath + @"/Upload";
                string filePathName = string.Empty; //最终文件名
                filePathName = Think9.Util.Helper.Common.CreateNo() + "." + filecombin[1];
                //Upload不存在则创建文件夹
                if (!System.IO.Directory.Exists(localPath))
                {
                    System.IO.Directory.CreateDirectory(localPath);
                }
                using (FileStream fs = System.IO.File.Create(Path.Combine(localPath, filePathName)))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                uploadFile.code = 0;
                uploadFile.src = Path.Combine("/Upload/", filePathName);
                uploadFile.msg = "上传成功";
                return Json(uploadFile);
            }
            catch (Exception)
            {
                uploadFile.code = -1;
                uploadFile.src = "";
                uploadFile.msg = "上传出错!程序异常";
                return Json(uploadFile);
            }
        }

        public ActionResult ExportFile2(List<IFormFile> file)
        {
            DataResult<string> rtnResult = new DataResult<string>();

            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    FileInfo fi = new FileInfo(formFile.FileName);
                    string ext = fi.Extension;
                    var orgFileName = fi.Name;
                    var newFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + formFile.FileName;

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