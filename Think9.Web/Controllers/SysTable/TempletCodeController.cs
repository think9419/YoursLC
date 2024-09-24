using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public partial class TempletCodeController : BaseController
    {
        private string _dbtype = HelpCode.GetDBProvider("DBProvider");//数据库类型
        private string _directory = Path.Combine(Directory.GetCurrentDirectory(), "");
        private ComService comService = new ComService();
        private TempletCode templetCode = new TempletCode();

        public ActionResult CodeRecord(string filePath)
        {
            ViewBag.FilePath = filePath;
            return View();
        }

        public JsonResult GetCodeRecordList(PageInfoEntity pageInfo, string filePath)
        {
            //string where = "where filePath='" + filePath.Replace("\\", "\\\\") + "'";
            string where = "where filePath='" + filePath + "'";
            if (_dbtype == "mysql")
            {
                where = "where filePath='" + filePath.Replace("\\", "\\\\") + "'";
            }

            RecordCodeService recordCode = new RecordCodeService();
            var list = recordCode.GetByWhere(where, null, null, "ORDER BY OperateTime DESC").ToList();

            return Json(new { code = 0, msg = "", count = list.Count(), data = list });
        }

        public ActionResult List(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.tbid = tbid;
                return View();
            }
        }

        public ActionResult List2(string rpId)
        {
            if (string.IsNullOrEmpty(rpId))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.rpid = rpId;
                return View();
            }
        }

        public ActionResult TempCode(string tbid, string date)
        {
            if (string.IsNullOrEmpty(tbid) || string.IsNullOrEmpty(date))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.tbid = tbid;
                ViewBag.date = date;
                return View();
            }
        }

        public ActionResult TempCode2(string rpid, string date)
        {
            if (string.IsNullOrEmpty(rpid) || string.IsNullOrEmpty(date))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.rpid = rpid;
                ViewBag.date = date;
                return View();
            }
        }

        public ActionResult TempDirectory(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.tbid = tbid;
                return View();
            }
        }

        public ActionResult TempDirectory2(string rpId)
        {
            if (string.IsNullOrEmpty(rpId))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.rpid = rpId;
                return View();
            }
        }

        public ActionResult CreateList(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.tbid = tbid;
                return View();
            }
        }

        public ActionResult CreateList2(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.rpid = rpid;
                return View();
            }
        }

        public ActionResult TbCodeList(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.tbid = tbid;
                return View();
            }
        }

        public ActionResult RpCodeList(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.rpid = rpid;
                return View();
            }
        }

        public ActionResult RpCodeList2(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.rpid = rpid;
                return View();
            }
        }

        public ActionResult CodeDetail(string tbid, string path, string type, string but)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                type = string.IsNullOrEmpty(type) ? "html" : type;
                but = string.IsNullOrEmpty(but) ? "" : but;
                string code = "文件不存在";
                string creatTime = "";
                if (Think9.Util.Helper.FileHelper.IsNotEmpty(_directory + path))
                {
                    code = Think9.Util.Helper.FileHelper.FileToString(_directory + path, Encoding.UTF8);
                    creatTime = System.IO.File.GetLastWriteTime(_directory + path).ToString();
                }
                ViewBag.CreatTime = creatTime;
                ViewBag.Code = code;
                ViewBag.path = path;
                ViewBag.button = string.IsNullOrEmpty(but) ? "" : but;
                ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
                ViewBag.type = type;

                switch (type)
                {
                    case "html":
                        return View("CodeDetail_Html");

                    case "js":
                        return View("CodeDetail_JS");

                    case "rdlc":
                        return View("CodeDetail_XML");

                    case "cs":
                        return View("CodeDetail_CS");

                    default:
                        return View("CodeDetail_Html");
                }
            }
        }

        [HttpGet]
        public JsonResult GetList(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetTempletList(tbid) };
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult GetList2(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetTempletList2(rpid) };
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult GetTempCode(string tbid, string date)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetTempCodeList(tbid, date) };
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult GetTempCode2(string rpid, string date)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetTempCodeList2(rpid, date) };
                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult EditCode(string tbid, string path, string code)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                if (!Think9.Util.Helper.FileHelper.IsNotEmpty(_directory + path))
                {
                    return Json(ErrorTip(path + "文件不存在"));
                }

                Think9.Util.Helper.FileHelper.CreateFile(_directory + path, code);

                RecordCodeEntity model = new RecordCodeEntity();
                model.ObjectId = tbid;
                model.OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                model.FilePath = path;
                model.Info = "编辑代码 - 通过管理页面操作";

                RecordCodeService recordCode = new RecordCodeService();
                recordCode.Insert(model);

                return Json(SuccessTip("操作成功"));
            }
        }

        [HttpPost]
        public ActionResult TempletCode(string tbid, string type, string path, string code)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                string copyFilePath = _directory + "\\AppTempletFile\\Views\\" + tbid.Replace("tb_", "").Replace("rp_", "") + "\\";
                string name = System.IO.Path.GetFileNameWithoutExtension(_directory + path) + ".txt";
                if (type == "js")
                {
                    copyFilePath = _directory + "\\AppTempletFile\\Self_js\\";
                }
                if (type == "rdlc")
                {
                    copyFilePath = _directory + "\\AppTempletFile\\Reports\\";
                }
                if (type == "cs")
                {
                    copyFilePath = _directory + "\\AppTempletFile\\Controllers\\";
                    if (path.EndsWith("Service.txt"))
                    {
                        copyFilePath = _directory + "\\AppTempletFile\\Services\\";
                    }
                    if (path.EndsWith("Model.txt"))
                    {
                        copyFilePath = _directory + "\\AppTempletFile\\Models\\";
                    }
                }

                if (!Directory.Exists(copyFilePath))
                {
                    Directory.CreateDirectory(copyFilePath);
                }

                Think9.Util.Helper.FileHelper.CreateFile(copyFilePath + name, code);
                return Json(SuccessTip("操作成功"));
            }
        }

        [HttpPost]
        public ActionResult CopyCode(string tbid, string path, string code)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                if (!Think9.Util.Helper.FileHelper.IsNotEmpty(_directory + path))
                {
                    return Json(ErrorTip(path + "文件不存在"));
                }

                string copyFilePath = _directory + "\\wwwroot\\TempCode\\" + tbid.Replace("tb_", "").Replace("rp_", "") + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";//
                if (!Directory.Exists(copyFilePath))
                {
                    Directory.CreateDirectory(copyFilePath);
                }

                string name = System.IO.Path.GetFileName(_directory + path);
                string nameNew = DateTime.Now.ToString("HH时mm分ss秒") + "_手动备份_" + name;

                Think9.Util.Helper.FileHelper.CreateFile(copyFilePath + nameNew, code);
                return Json(SuccessTip("操作成功"));
            }
        }

        [HttpGet]
        public JsonResult GetTempDirectory(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetTempDirectoryList(tbid) };
                return Json(result);
            }
        }

        [HttpGet]
        public JsonResult GetTempDirectory2(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetTempDirectoryList2(rpid) };
                return Json(result);
            }
        }

        public JsonResult GetCreateList(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetCreatList(tbid) };
                return Json(result);
            }
        }

        public JsonResult GetCreateList2(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetCreatList2(rpid) };
                return Json(result);
            }
        }

        public JsonResult GetCodeList(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetCodeListByTbId(tbid) };
                return Json(result);
            }
        }

        public JsonResult GetCodeList2(string rpid)
        {
            if (string.IsNullOrEmpty(rpid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                var result = new { code = 0, count = 999999, data = templetCode.GetCodeListByRpId(rpid) };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult Creat(string tbid, string idsStr)
        {
            templetCode.CreatTemplet(tbid, idsStr);
            return Json(SuccessTip("操作成功"));
        }

        [HttpPost]
        public JsonResult Creat2(string rpid, string idsStr)
        {
            templetCode.CreatTemplet2(rpid, idsStr);
            return Json(SuccessTip("操作成功"));
        }

        [HttpPost]
        public JsonResult BatchDel(string idsStr)
        {
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    if (arr[i].ToString().Trim() != "")
                    {
                        string fullpath = arr[i].ToString().Trim();
                        if (System.IO.File.Exists(_directory + fullpath))
                        {
                            System.IO.File.Delete(_directory + fullpath);
                        }
                    }
                }
            }
            return Json(SuccessTip("操作成功"));
        }

        [HttpPost]
        public JsonResult BatchDelDirectory(string idsStr)
        {
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    if (arr[i].ToString().Trim() != "")
                    {
                        string fullpath = arr[i].ToString().Trim();
                        if (System.IO.Directory.Exists(_directory + fullpath))
                        {
                            System.IO.Directory.Delete(_directory + fullpath, true);
                        }
                    }
                }
            }
            return Json(SuccessTip("操作成功"));
        }
    }
}