using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbCodeBuildController : BaseController
    {
        private TbBasicService tbBasicService = new TbBasicService();
        private CodeBuildServices codeBuild = new CodeBuildServices();
        private SortService sortService = new SortService();

        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public TbCodeBuildController()
        {
        }

        public override ActionResult Index(int? id)
        {
            ViewBag.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");
            ViewBag.SortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");
            return base.Index(id);
        }

        public ActionResult CodeBuild()
        {
            return View("~/Areas/SysTable/TableList/CodeBuild.cshtml");
        }

        [HttpPost]
        public ActionResult GetMsg(string idsStr)
        {
            string msg = codeBuild.GetCountInfo(idsStr);
            var result = SuccessTip(msg);
            return Json(result);
        }

        public ActionResult LookCode(string tbid)
        {
            string err = "";
            string siteRoot = directoryPath;
            if (!tbid.Contains(","))
            {
                TbBasicEntity modelTbBasic = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
                if (modelTbBasic == null)
                {
                    return Json("录入表对象为空！");
                }

                modelTbBasic.isAux = string.IsNullOrEmpty(modelTbBasic.isAux) ? "2" : modelTbBasic.isAux;
                if (modelTbBasic.isAux == "1")
                {
                    return Json("不能对辅助表进行此操作！");
                }
            }
            Think9.Models.CodeBuildEntity model = codeBuild.LookOverTableCode(ref err, siteRoot, tbid, CurrentUser);
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
            if (Debugger.IsAttached)
            {
                try
                {
                    string siteRoot = directoryPath;
                    DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                    string err = codeBuild.GetTableCode(dtRecord, idsStr, siteRoot, "", CurrentUser, "");
                    if (string.IsNullOrEmpty(err))
                    {
                        RecordCodeService.Add(dtRecord);
                        var result = SuccessTip("操作成功,生成的代码在AppCreatCode文件夹中");
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
            else
            {
                return Json(ErrorTip("请在本地开发模式时使用代码生成，请选择代码下载方式！"));
            }
        }

        [HttpPost]
        public ActionResult DownloadCode(string idsStr, string type)
        {
            if (CurrentUser == null)
            {
                return Json(ErrorTip("当前用户对象为空！"));
            }

            if (!idsStr.Contains(","))
            {
                TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + idsStr + "'");
                if (model == null)
                {
                    return Json("录入表对象为空！");
                }

                model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
                if (model.isAux == "1")
                {
                    return Json("不能对辅助表进行此操作！");
                }
            }

            try
            {
                string siteRoot = directoryPath;
                string guid = System.Guid.NewGuid().ToString("N");

                //在文件夹AppCode_Temp生成临时代码
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                string err = codeBuild.GetTableCode(dtRecord, idsStr, siteRoot, guid, CurrentUser, "", 0, type);
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
    }
}