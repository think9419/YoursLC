using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Data;
using System.IO;
using System.Linq;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbReleaseController : BaseController
    {
        private TbBasicService tbBasicService = new TbBasicService();
        private CodeBuildServices codeBuild = new CodeBuildServices();
        private SortService sortService = new SortService();
        private ChangeModel changeModel = new ChangeModel();
        private string _dbtype = HelpCode.GetDBProvider("DBProvider");//数据库类型

        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public TbReleaseController()
        {
        }

        public override ActionResult Index(int? id)
        {
            ViewBag.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");
            ViewBag.SortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");
            return base.Index(id);
            //return View();
        }

        //重新生成
        [HttpPost]
        public ActionResult SingleRelease(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr))
            {
                return Json(ErrorTip("ID为空"));
            }
            if (CurrentUser == null)
            {
                return Json(ErrorTip("当前用户对象为空！"));
            }
            if (!idsStr.Contains(","))
            {
                TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + idsStr + "'");
                if (model == null)
                {
                    return Json(ErrorTip("录入表对象为空！"));
                }

                model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
                if (model.isAux == "1")
                {
                    return Json(ErrorTip("不能对辅助表进行此操作！"));
                }
            }

            try
            {
                string siteRoot = directoryPath;
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                string err = codeBuild.GetTableCode(dtRecord, idsStr, siteRoot, "", CurrentUser, "release");

                if (string.IsNullOrEmpty(err))
                {
                    //生成单独录入表js文件，在wwwroot文件夹中生成js文件时会引起页面刷新，需单独提出来处理
                    codeBuild.GetTableJSCode(dtRecord, idsStr, siteRoot, _dbtype);
                    changeModel.SetTableModel(dtRecord, "release", idsStr);

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

            try
            {
                string siteRoot = directoryPath;
                DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                err = codeBuild.GetTableCode(dtRecord, idsStr, siteRoot, "", CurrentUser, "release");
                if (string.IsNullOrEmpty(err))
                {
                    //生成单独录入表js文件，在wwwroot文件夹中生成js文件时会引起页面刷新，需单独提出来处理
                    codeBuild.GetTableJSCode(dtRecord, idsStr, siteRoot, _dbtype);
                    changeModel.SetTableModel(dtRecord, "release", idsStr);

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
                return Json(ErrorTip(ex.ToString()));
            }
        }
    }
}