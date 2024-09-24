using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Data;
using System.IO;
using System.Linq;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class RpCodeBuildController : BaseController
    {
        private CodeBuildServices codeBuild = new CodeBuildServices();
        private SortService sortService = new SortService();

        private string _dbtype = Think9.Services.Base.Configs.GetDBProvider("DBProvider");//数据库类型

        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public RpCodeBuildController()
        {
        }

        public override ActionResult Index(int? id)
        {
            ViewBag.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");//不能删
            ViewBag.SortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_report"), "SortID", "SortName");
            return base.Index(id);
        }

        public ActionResult LookCode(string rpid)
        {
            string err = AppSet.CheckConnection();
            if (!string.IsNullOrEmpty(err))
            {
                return Json(ErrorTip(err));
            }

            string siteRoot = directoryPath;
            Think9.Models.CodeBuildEntity model = codeBuild.LookOverReportCode(ref err, siteRoot, rpid, CurrentUser);
            if (string.IsNullOrEmpty(err))
            {
                return View(model);
            }
            else
            {
                return Json(err);
            }
        }

        [HttpPost]
        public ActionResult CodeBuild(string idsStr)
        {
            string err = AppSet.CheckConnection();
            if (!string.IsNullOrEmpty(err))
            {
                return Json(ErrorTip(err));
            }

            string siteRoot = directoryPath;
            try
            {
                string guid = System.Guid.NewGuid().ToString("N");

                //在文件夹AppCode_Temp生成临时代码
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                err = codeBuild.GetReportCode(dtRecord, idsStr, siteRoot, guid, CurrentUser, "");
                if (string.IsNullOrEmpty(err))
                {
                    RecordCodeService.Add(dtRecord);
                    //将文件夹AppCode_Temp中的文件压缩，形成下载链接
                    string url = codeBuild.CompressFile(guid);
                    var result = SuccessTip("操作成功", url);
                    return Json(result);
                }
                else
                {
                    Think9.Util.Helper.FileHelper.DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory(), "AppCode_Temp\\" + guid + "\\"));
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult DownloadCode(string idsStr)
        {
            string err = AppSet.CheckConnection();
            if (!string.IsNullOrEmpty(err))
            {
                return Json(ErrorTip(err));
            }

            if (string.IsNullOrEmpty(idsStr))
            {
                return Json("参数idsStr为空！");
            }
            string siteRoot = directoryPath;
            string guid = System.Guid.NewGuid().ToString("N");
            try
            {
                //在文件夹AppCode_Temp生成临时代码
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                err = codeBuild.GetReportCode(dtRecord, idsStr, siteRoot, guid, CurrentUser, "");
                if (string.IsNullOrEmpty(err))
                {
                    RecordCodeService.Add(dtRecord);
                    string url = codeBuild.CompressFile(guid);

                    var result = SuccessTip("操作成功", url);
                    return Json(result);
                }
                else
                {
                    Think9.Util.Helper.FileHelper.DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory(), "AppCode_Temp\\" + guid + "\\"));
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }
    }
}