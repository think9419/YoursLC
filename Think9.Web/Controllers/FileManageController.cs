using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Com;

namespace Think9.Controllers.Basic
{
    [Area("Com")]
    public class FileManageController : Controller
    {
        private ComService comService = new ComService();
        private AttachmentService attachmentService = new AttachmentService();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private FlowBase flowService = new FlowBase();

        private string GetHostUrl(HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host).ToString();
        }

        public FileManageController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public IActionResult Attachment(string attid, string fwid, string listid, string prcid, string userid)
        {
            ServiceFlow flow = new ServiceFlow();

            string A1 = ""; //公共附件选项? 新建 1有权限2无
            string A2 = ""; //公共附件选项? 下载 1有权限2无
            string A3 = ""; //公共附件选项? 删除 1有权限2无

            string attSort = "";//附件分类
            string attExts = "";//允许文件类型
            FlowEntity mflow = flow.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
            if (mflow != null)
            {
                attSort = string.IsNullOrEmpty(mflow.FlowAttachment) ? "" : mflow.FlowAttachment;
                attExts = string.IsNullOrEmpty(mflow.FlowAttachment2) ? "" : mflow.FlowAttachment2;
            }
            if (string.IsNullOrEmpty(attExts))
            {
                attExts = "xls|xlsx|csv|zip|pdf|doc|docx|png|jpeg|jpg|gif|ico|txt";//允许文件类型
            }

            attachmentService.GetFileAuthority(fwid, prcid, ref A1, ref A2, ref A3);
            listid = listid == null ? "0" : listid;
            fwid = fwid == null ? "" : fwid;//流程编码
            ViewBag.ListId = listid;
            ViewBag.FwId = fwid;
            ViewBag.PrcId = prcid == null ? "" : prcid;//流程步骤id
            ViewBag.UserId = userid;
            ViewBag.A1 = A1;
            ViewBag.A2 = A2;
            ViewBag.A3 = A3;

            string attid2 = string.IsNullOrEmpty(attid) ? "" : attid;//附件id
            if (attid2 == "")
            {
                if (fwid.StartsWith("bi_"))
                {
                    if (listid != "0")
                    {
                        attid2 = comService.GetSingleField("select attachmentId  FROM " + fwid.Replace("bi_", "tb_") + " WHERE listid= " + listid);
                    }
                }
                else
                {
                    if (listid != "0")
                    {
                        attid2 = comService.GetSingleField("select attachmentId  FROM flowrunlist WHERE listid= " + listid);
                    }
                }
            }

            if (string.IsNullOrEmpty(attid2))
            {
                //限定50字符以内 附件id以流程编码作为前缀
                string str = fwid.Replace("fw_", "").Replace("bi_", "");
                if (str.Length > 17)
                {
                    attid2 = str.Substring(0, 17) + "_" + System.Guid.NewGuid().ToString("N");
                }
                else
                {
                    attid2 = str + "_" + System.Guid.NewGuid().ToString("N");
                }

                if (listid != "0")
                {
                    //这操作可能会重复，但是保险些
                    if (fwid.StartsWith("bi_"))
                    {
                        comService.ExecuteSql("update " + fwid.Replace("bi_", "tb_") + " set attachmentId='" + attid2 + "'   WHERE listid= " + listid);
                    }
                    else
                    {
                        comService.ExecuteSql("update flowrunlist set attachmentId='" + attid2 + "'   WHERE listid= " + listid);
                    }
                }
            }

            ViewBag.attachmentId = attid2;

            ViewBag.exts = attExts;//允许文件类型

            //有附件分类
            if (attSort.Trim() != "")
            {
                List<valueTextEntity> list = new List<valueTextEntity>();
                string[] arr = BaseUtil.GetStrArray(attSort, " ");
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (arr[i] != null)
                    {
                        if (arr[i].ToString().Trim() != "")
                        {
                            list.Add(new valueTextEntity { Value = arr[i].ToString().Trim(), Text = arr[i].ToString().Trim() });
                        }
                    }
                }

                ViewBag.Sort = list;//附件分类

                return View("Attachment2");
            }
            else
            {
                return View();
            }
        }

        public IActionResult AttList(string fwid, string listid, string userid, string isOriginal)
        {
            ServiceFlow flow = new ServiceFlow();
            isOriginal = string.IsNullOrEmpty(isOriginal) ? "n" : isOriginal;//id是否初始值？
            listid = ListIdService.GetOriginalListId(listid, isOriginal).ToString();

            string A2 = "1"; //公共附件选项? 下载 1有权限2无

            string attid = "";//附件id
            if (fwid.StartsWith("bi_"))
            {
                attid = comService.GetSingleField("select attachmentId  FROM " + fwid.Replace("bi_", "tb_") + " WHERE listid= " + listid);
            }
            else
            {
                attid = comService.GetSingleField("select attachmentId  FROM flowrunlist WHERE listid= " + listid);
            }

            string attSort = "";//附件分类
            FlowEntity mflow = flow.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
            if (mflow != null)
            {
                attSort = string.IsNullOrEmpty(mflow.FlowAttachment) ? "" : mflow.FlowAttachment;
            }

            ViewBag.ListId = listid;
            ViewBag.A2 = A2;
            ViewBag.attachmentId = attid;
            ViewBag.FwId = fwid;
            ViewBag.UserId = userid;

            //有附件分类
            if (attSort.Trim() != "")
            {
                List<valueTextEntity> list = new List<valueTextEntity>();
                string[] arr = BaseUtil.GetStrArray(attSort, " ");
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (arr[i] != null)
                    {
                        if (arr[i].ToString().Trim() != "")
                        {
                            list.Add(new valueTextEntity { Value = arr[i].ToString().Trim(), Text = arr[i].ToString().Trim() });
                        }
                    }
                }

                ViewBag.Sort = list;//附件分类

                return View("AttList2");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult GetFileList(PageInfoEntity pageInfo, string attid, string fwid, string prcid, string A2, string A3)
        {
            string strPath = $"{this._webHostEnvironment.WebRootPath}\\UserFile\\";

            pageInfo.field = "id";
            pageInfo.order = "desc";

            long total = 0;
            IEnumerable<dynamic> list = attachmentService.GetPageByFilter(ref total, null, pageInfo, "where attachmentId ='" + attid + "'");
            foreach (AttachmentEntity obj in list)
            {
                obj.A2 = A2;
                obj.A3 = A3;
                obj.Src = "";
                if (obj.A2 == "1")
                {
                    obj.Src = Path.Combine("/UserFile/" + attid + "/", obj.FullName);
                }
            }

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetAttList(PageInfoEntity pageInfo, string attid)
        {
            string strPath = $"{this._webHostEnvironment.WebRootPath}\\UserFile\\";

            pageInfo.field = "id";
            pageInfo.order = "desc";

            long total = 0;
            IEnumerable<dynamic> list = attachmentService.GetPageByFilter(ref total, null, pageInfo, "where attachmentId ='" + attid + "'");
            foreach (AttachmentEntity obj in list)
            {
                obj.A2 = "1";
                obj.Src = "";

                obj.Src = Path.Combine("/UserFile/" + attid + "/", obj.FullName);
            }

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        public IActionResult FileUplaod(string listid, string tbid, string indexid, string from, string fname, string prcno)
        {
            string maintbid = comService.GetSingleField("select ParentId  FROM tbbasic WHERE TbId= '" + tbid + "'").Trim();
            if (maintbid == "")
            {
                maintbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            }
            string fileType = comService.GetSingleField("select FileType  FROM tbindex WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'").Trim();
            if (fileType == "")
            {
                fileType = Think9.Services.Base.Configs.GetValue("UploadFileType");//默认的上传文件类型
                ViewBag.FileTypeShow = "文件格式未限定";
            }
            else
            {
                ViewBag.FileTypeShow = "" + fileType;
            }
            ViewBag.FileType = fileType;
            ViewBag.tbid = maintbid;
            ViewBag.listid = string.IsNullOrEmpty(listid) ? "0" : listid;
            ViewBag.indexid = indexid == null ? "" : indexid;
            ViewBag.btnDisabled = Think9.Services.Com.FlowCom.GetIndexDisabled(tbid, prcno, indexid); //判断是否可写字段
            ViewBag.from = from == null ? "" : from;
            ViewBag.fname = fname == null ? "" : fname;

            if (!string.IsNullOrEmpty(fname))
            {
                string newDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + maintbid + "_Files\\" + listid + "\\");
                string oldDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + maintbid + "_Files\\0\\");
                if (listid != "0" && Think9.Util.Helper.FileHelper.IsExistFile(oldDirectory + fname))
                {
                    Think9.Util.Helper.FileHelper.CreateSuffic(newDirectory);
                    System.IO.File.Move(oldDirectory + fname, newDirectory + fname);
                }
                string strPath = GetHostUrl(HttpContext.Request) + "/UserFile/" + maintbid + "_Files/" + listid + "/" + fname;
                ViewBag.src = strPath;
            }

            return View();
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FileUplaod(string listid, string tbid, string oldName, List<IFormFile> file)
        {
            string err = "";
            string fileName = "";
            listid = string.IsNullOrEmpty(listid) ? "0" : listid;
            tbid = string.IsNullOrEmpty(tbid) ? "err" : tbid;
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + tbid + "_Files\\" + listid + "\\");
            Boolean fileOK = false;
            DataResult<string> rtnResult = new DataResult<string>();
            UploadFileEntity uploadFile = new UploadFileEntity();

            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    string ext = new FileInfo(formFile.FileName).Extension;
                    fileOK = true;

                    if (fileOK)
                    {
                        fileName = System.Guid.NewGuid().ToString("N") + ext;

                        //创建文件夹-创建前已经做了判断有则不创建
                        Think9.Util.Helper.FileHelper.CreateSuffic(uploads);
                        var filePath = Path.Combine(uploads, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        if (!string.IsNullOrEmpty(oldName))
                        {
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(uploads, oldName));
                            uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + tbid + "_Files\\0\\");
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(uploads, oldName));
                        }
                    }
                    else
                    {
                        err = "不允许的文件格式";
                    }
                }
                else
                {
                    err = "操作失败";
                }
            }

            if (err == "")
            {
                uploadFile.code = 0;
                uploadFile.src = GetHostUrl(HttpContext.Request) + "/UserFile/" + tbid + "_Files/" + listid + "/" + fileName; ;
                uploadFile.msg = "上传成功";
                uploadFile.filename = fileName;
                return Json(uploadFile);
            }
            else
            {
                uploadFile.code = 1;
                uploadFile.msg = err;
                return Json(uploadFile);
            }
        }

        public IActionResult FileUplaodGrid(string listid, string fwid, string tbid, string indexid, string id, string strv, string from, string fname, string prcno)
        {
            string maintbid = string.IsNullOrEmpty(fwid) ? "" : fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string fileType = comService.GetSingleField("select FileType  FROM tbindex WHERE TbId= '" + tbid + "' and IndexId= '" + strv + "'").Trim();
            if (fileType == "")
            {
                fileType = Think9.Services.Base.Configs.GetValue("UploadFileType");//默认的上传文件类型
            }
            ViewBag.FileType = fileType;// 设置允许上传的格式

            ViewBag.listid = string.IsNullOrEmpty(listid) ? "0" : listid;
            ViewBag.maintbid = maintbid;
            ViewBag.from = from == null ? "" : from;

            ViewBag.PuTbId = tbid == null ? "" : tbid;
            ViewBag.PuIndexId = indexid == null ? "" : indexid;
            ViewBag.PuId = id == null ? "" : id;
            ViewBag.PuV = strv == null ? "" : strv.Replace("v", "");
            ViewBag.Pufname = fname == null ? "" : fname;

            if (!string.IsNullOrEmpty(fname))
            {
                string newDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + maintbid + "_Files\\" + listid + "\\");
                string oldDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + maintbid + "_Files\\0\\");
                if (listid != "0" && Think9.Util.Helper.FileHelper.IsExistFile(oldDirectory + fname))
                {
                    Think9.Util.Helper.FileHelper.CreateSuffic(newDirectory);
                    System.IO.File.Move(oldDirectory + fname, newDirectory + fname);
                }
                string strPath = GetHostUrl(HttpContext.Request) + "/UserFile/" + maintbid + "_Files/" + listid + "/" + fname;
                ViewBag.src = strPath;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FileUplaodGrid(string listid, string maintbid, string oldName, List<IFormFile> file)
        {
            string err = "";
            string fileName = "";
            listid = string.IsNullOrEmpty(listid) ? "0" : listid;
            maintbid = string.IsNullOrEmpty(maintbid) ? "err" : maintbid;
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + maintbid + "_Files\\" + listid + "\\");
            Boolean fileOK = false;
            DataResult<string> rtnResult = new DataResult<string>();
            UploadFileEntity uploadFile = new UploadFileEntity();

            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    string ext = new FileInfo(formFile.FileName).Extension;
                    fileOK = true;

                    if (fileOK)
                    {
                        //fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + System.Guid.NewGuid().ToString("N") + ext;
                        fileName = System.Guid.NewGuid().ToString("N") + ext;

                        //创建文件夹-创建前已经做了判断有则不创建
                        Think9.Util.Helper.FileHelper.CreateSuffic(uploads);
                        var filePath = Path.Combine(uploads, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        if (!string.IsNullOrEmpty(oldName))
                        {
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(uploads, oldName));
                            uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + maintbid + "_Files\\0\\");
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(uploads, oldName));
                        }
                    }
                    else
                    {
                        err = "不允许的文件格式";
                    }
                }
                else
                {
                    err = "操作失败";
                }
            }

            if (err == "")
            {
                uploadFile.code = 0;
                uploadFile.src = GetHostUrl(HttpContext.Request) + "/UserFile/" + maintbid + "_Files/" + listid + "/" + fileName; ;
                uploadFile.msg = "上传成功";
                uploadFile.filename = fileName;
                return Json(uploadFile);
            }
            else
            {
                uploadFile.code = 1;
                uploadFile.msg = err;
                return Json(uploadFile);
            }
        }

        public IActionResult PictureUplaod(string tbid, string indexid, string from, string fname, string prcno)
        {
            tbid = tbid == null ? "" : tbid;
            indexid = indexid == null ? "" : indexid;
            string strPath = $"{this._webHostEnvironment.WebRootPath}\\UserImg\\";
            ViewBag.tbid = tbid == null ? "" : tbid;
            ViewBag.indexid = indexid == null ? "" : indexid;
            string fileType = comService.GetSingleField("select FileType  FROM tbindex WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'").Trim();
            if (fileType == "")
            {
                fileType = Think9.Services.Base.Configs.GetValue("UploadImgType");//默认的上传文件类型
            }
            ViewBag.FileType = fileType;// 设置允许上传的格式
            ViewBag.btnDisabled = Think9.Services.Com.FlowCom.GetIndexDisabled(tbid, prcno, indexid); //判断是否可写字段
            ViewBag.from = from == null ? "" : from;
            ViewBag.fname = fname == null ? "nonexistent.gif" : fname;
            if (ViewBag.fname == "nonexistent.gif")
            {
                ViewBag.src = Path.Combine("/images/", "nonexistent.gif");
                ViewBag.src2 = Path.Combine("/images/", "nonexistent.gif");
            }
            else
            {
                if (System.IO.File.Exists(strPath + ViewBag.fname))
                {
                    ViewBag.src = Path.Combine("/UserImg/", ViewBag.fname);
                    ViewBag.src2 = Path.Combine("/UserImg/", "_" + ViewBag.fname);
                }
                else
                {
                    ViewBag.src = Path.Combine("/images/", "nonexistent.gif");
                    ViewBag.src2 = Path.Combine("/images/", "nonexistent.gif");
                }
            }

            return View();
        }

        public IActionResult PictureUplaodGrid(string tbid, string indexid, string id, string strv, string fname, string from)
        {
            tbid = tbid == null ? "" : tbid;
            indexid = indexid == null ? "" : indexid;
            string strPath = $"{this._webHostEnvironment.WebRootPath}\\UserImg\\";
            ViewBag.PuTbId = tbid;
            ViewBag.PuIndexId = indexid;
            string fileType = comService.GetSingleField("select FileType  FROM tbindex WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'").Trim();
            if (fileType == "")
            {
                fileType = Think9.Services.Base.Configs.GetValue("UploadImgType");//默认的上传文件类型
            }
            ViewBag.FileType = fileType;
            ViewBag.PuId = id == null ? "" : id;
            ViewBag.PuV = strv == null ? "" : strv.Replace("v", "");
            ViewBag.Pufname = fname == null ? "nonexistent.gif" : fname;
            if (ViewBag.Pufname == "nonexistent.gif")
            {
                ViewBag.src = Path.Combine("/images/", "nonexistent.gif");
                ViewBag.src2 = Path.Combine("/images/", "nonexistent.gif");
            }
            else
            {
                if (System.IO.File.Exists(strPath + ViewBag.Pufname))
                {
                    ViewBag.src = Path.Combine("/UserImg/", ViewBag.Pufname);
                    ViewBag.src2 = Path.Combine("/UserImg/", "_" + ViewBag.Pufname);
                }
                else
                {
                    ViewBag.src = Path.Combine("/images/", "nonexistent.gif");
                    ViewBag.src2 = Path.Combine("/images/", "nonexistent.gif");
                }
            }

            ViewBag.PuFrom = from == null ? "" : from;

            return View();
        }

        public IActionResult PictureShow(string id)
        {
            string strPath = $"{this._webHostEnvironment.WebRootPath}\\UserImg\\";
            ViewBag.id = id == null ? "nonexistent.gif" : id;
            if (ViewBag.id == "nonexistent.gif")
            {
                ViewBag.src = Path.Combine("/images/", "nonexistent.gif");
                ViewBag.src2 = Path.Combine("/images/", "nonexistent.gif");
            }
            else
            {
                if (System.IO.File.Exists(strPath + ViewBag.id))
                {
                    ViewBag.src = Path.Combine("/UserImg/", ViewBag.id);
                    ViewBag.src2 = Path.Combine("/UserImg/", "_" + ViewBag.id);
                }
                else
                {
                    ViewBag.src = Path.Combine("/images/", "nonexistent.gif");
                    ViewBag.src2 = Path.Combine("/images/", "nonexistent.gif");
                }
            }

            return View();
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImgUplaod(string tbid, string oldName, List<IFormFile> file)
        {
            string err = "";
            string imgName = "";
            tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            oldName = string.IsNullOrEmpty(oldName) ? "" : oldName;
            Boolean fileOK = false;
            //DataResult<string> rtnResult = new DataResult<string>();
            UploadFileEntity uploadFile = new UploadFileEntity();

            foreach (var formFile in file)
            {
                if (formFile.Length > 0)
                {
                    string ext = new FileInfo(formFile.FileName).Extension;
                    String[] allowedExtensions = { ".gif", ".jpg", ".png", ".bmp", ".jpeg" };
                    for (int i = 0; i < allowedExtensions.Length; i++)
                    {
                        if (ext.ToLower() == allowedExtensions[i])
                        {
                            fileOK = true;
                            break;
                        }
                    }

                    if (fileOK)
                    {
                        imgName = tbid.Replace("tb_", "") + "_" + System.Guid.NewGuid().ToString("N") + ext;

                        string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserImg");
                        var filePath = Path.Combine(uploads, "_" + imgName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        //压缩图片
                        CompressImage(uploads, imgName);

                        if (!string.IsNullOrEmpty(oldName) && oldName != "nonexistent.gif" && oldName.StartsWith(tbid + "_"))
                        {
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserImg"), oldName));
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserImg"), "_" + oldName));
                        }
                    }
                    else
                    {
                        err = "不允许的文件格式";
                    }
                }
                else
                {
                    err = "操作失败";
                }
            }

            if (err == "")
            {
                uploadFile.code = 0;
                uploadFile.src = Path.Combine("/UserImg/", imgName);
                uploadFile.msg = "上传成功";
                uploadFile.filename = imgName;
                return Json(uploadFile);
            }
            else
            {
                uploadFile.code = 1;
                uploadFile.msg = err;
                return Json(uploadFile);
            }
        }

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AttUplaod(string attid, string sort, string fwid, string prcid, string listid, string userid, List<IFormFile> file)
        {
            string err = "";
            string fileName = "";
            DataResult<string> rtnResult = new DataResult<string>();
            UploadFileEntity uploadFile = new UploadFileEntity();

            FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
            try
            {
                foreach (var formFile in file)
                {
                    if (formFile.Length > 0)
                    {
                        string ext = new FileInfo(formFile.FileName).Extension.ToLower();
                        err = JudgeFileType(mflow, ext);

                        if (string.IsNullOrEmpty(err))
                        {
                            fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + formFile.FileName;
                            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + attid);
                            var filePath = Path.Combine(uploads, fileName);

                            //创建文件夹-创建前已经做了判断有则不创建
                            Think9.Util.Helper.FileHelper.CreateSuffic(uploads);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);

                                FileInfo fInfo2 = new FileInfo(filePath);

                                string length = $"{fInfo2.Length / 1024}KB";

                                AttachmentEntity model = new AttachmentEntity();
                                model.attachmentId = attid;
                                model.FwId = fwid;
                                model.ListId = int.Parse(listid);
                                model.FwId = fwid;
                                model.PrcsId = string.IsNullOrEmpty(prcid) ? 0 : int.Parse(prcid);
                                model.FullName = fileName;
                                model.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                model.UserId = userid;

                                model.DocType = fInfo2.Extension;
                                model.AttachmentSort = string.IsNullOrEmpty(sort) ? "" : sort;
                                model.TotalSize = length;
                                model.DownloadNumber = 0;

                                if (attachmentService.Insert(model))
                                {
                                    if (model.ListId == 0)
                                    {
                                        //要将标志位加上，后面再修改
                                        Record.AddAttInfo(userid, listid, fwid, "#上传附件#" + fileName, attid);
                                    }
                                    else
                                    {
                                        Record.Add(userid, listid, fwid, "#上传附件#" + fileName);
                                    }
                                }
                                else
                                {
                                    uploadFile.code = 1;
                                    uploadFile.msg = "上传失败";
                                    return Json(uploadFile);
                                }
                            }
                        }
                    }
                    else
                    {
                        err = "上传失败";
                    }
                }

                if (err == "")
                {
                    uploadFile.code = 0;
                    uploadFile.src = attid;
                    uploadFile.msg = "上传成功";
                    uploadFile.filename = fileName;
                    return Json(uploadFile);
                }
                else
                {
                    uploadFile.code = 1;
                    uploadFile.msg = err;
                    return Json(uploadFile);
                }
            }
            catch (Exception ex)
            {
                uploadFile.code = -1;
                uploadFile.msg = ex.Message;
                return Json(uploadFile);
            }
        }

        [HttpGet]
        public IActionResult Download(string attid, string listid, string fwid, string uid, string filename)
        {
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + attid);
            var filePath = Path.Combine(uploads, filename);
            if (System.IO.File.Exists(filePath))
            {
                Record.Add(uid, listid, fwid, "#下载附件#" + filename);

                ///定义并实例化一个内存流，以存放图片的字节数组。
                MemoryStream ms = new MemoryStream();
                ///图片读入FileStream
                FileStream f = new FileStream(filePath, FileMode.Open);
                ///把FileStream写入MemoryStream
                ms.SetLength(f.Length);
                f.Read(ms.GetBuffer(), 0, (int)f.Length);
                ms.Flush();
                f.Close();

                var contentType = MimeMapping.GetMimeMapping(filename);
                return File(ms, contentType, filePath);
            }
            else
            {
                return Json("文件不存在");
            }
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteFile(int id, string attid, string listid, string fwid, string uid, string filename)
        {
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + attid);
            var fullpath = Path.Combine(uploads, filename);

            if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }

            attachmentService.DeleteByWhere("where Id=" + id + "");

            if (listid != "0")
            {
                Record.Add(uid, listid, fwid, "#删除附件#" + filename);
            }

            var result = new AjaxResult { state = ResultType.success.ToString(), message = "删除成功" };
            return Json(result);
        }

        private string JudgeFileType(FlowEntity mflow, string type)
        {
            string err = "";
            string attBan = "";//禁止附件类型
            string attAllow = "";//允许文件类型
            if (mflow != null)
            {
                attBan = string.IsNullOrEmpty(mflow.FlowAttachment3) ? "" : mflow.FlowAttachment3.Trim();
                attAllow = string.IsNullOrEmpty(mflow.FlowAttachment2) ? "" : mflow.FlowAttachment2.Trim();
            }
            string[] arrBan = BaseUtil.GetStrArray(attBan, "|");
            string[] arrAllow = BaseUtil.GetStrArray(attAllow, "|");

            for (int i = 0; i < arrBan.GetLength(0); i++)
            {
                if (arrBan[i] != null)
                {
                    if (type == arrBan[i].ToString().ToLower().Trim() || type == "." + arrBan[i].ToString().ToLower().Trim())
                    {
                        err = "禁止上传" + arrBan[i].ToString().Trim() + "类型文件";
                        break;
                    }
                }
            }

            Boolean fileOK = false;
            if (string.IsNullOrEmpty(attAllow))
            {
                fileOK = true;
            }
            else
            {
                for (int i = 0; i < arrAllow.GetLength(0); i++)
                {
                    if (arrAllow[i] != null)
                    {
                        if (type == arrAllow[i].ToString().ToLower().Trim() || type == "." + arrAllow[i].ToString().ToLower().Trim())
                        {
                            fileOK = true;
                            break;
                        }
                    }
                }
            }

            if (!fileOK)
            {
                err += " 只允许上传" + attAllow + "类型文件";
            }

            return err;
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <returns></returns>
        private static void CompressImage(string uploads, string imgName)
        {
            using var image = new MagickImage(Path.Combine(uploads, "_" + imgName));

            image.Quality = 100;//无损压缩
            double width = image.Width;
            int iPercentage = 10;
            if (image.Width < 80)
            {
                iPercentage = 100;
            }
            Percentage percentage = new Percentage(iPercentage);
            image.Resize(percentage);//调大小 按px值
                                     //image.Scale(maxWidth, maxHeight);//缩放 字不清楚较严重
            image.Write(Path.Combine(uploads, imgName));//输出到磁盘
            image.Dispose();
        }
    }
}