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
    public class RpReleaseController : BaseController
    {
        private CodeBuildServices codeBuild = new CodeBuildServices();
        private SortService sortService = new SortService();
        private ChangeModel changeModel = new ChangeModel();
        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public RpReleaseController()
        {
        }

        public override ActionResult Index(int? id)
        {
            ViewBag.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");//不能删
            ViewBag.SortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_report"), "SortID", "SortName");
            return base.Index(id);
            //return View();
        }

        //重新生成
        [HttpPost]
        public ActionResult SingleRelease(string idsStr)
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

            try
            {
                string siteRoot = directoryPath;
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                err = codeBuild.GetReportCode(dtRecord, idsStr, siteRoot, "", CurrentUser, "release");
                if (string.IsNullOrEmpty(err))
                {
                    changeModel.ChangeReportModel(dtRecord, "release", idsStr);

                    RecordCodeService.Add(dtRecord);
                    var result = SuccessTip("生成成功！将显示可能存在的问题");
                    return Json(result);
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        //重新生成
        [HttpPost]
        public ActionResult Release(string idsStr)
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
            try
            {
                string siteRoot = directoryPath;
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                err = codeBuild.GetReportCode(dtRecord, idsStr, siteRoot, "", CurrentUser, "release");
                if (string.IsNullOrEmpty(err))
                {
                    changeModel.ChangeReportModel(dtRecord, "release", idsStr);

                    RecordCodeService.Add(dtRecord);

                    var result = SuccessTip("生成成功！将显示可能存在的问题");
                    return Json(result);
                }
                else
                {
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