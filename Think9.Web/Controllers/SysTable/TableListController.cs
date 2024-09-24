using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    public partial class TableListController : BaseController
    {
        private TbBasicService tbBasicService = new TbBasicService();
        private ComService comService = new ComService();
        private string _dbtype = HelpCode.GetDBProvider("DBProvider");//数据库类型
        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public override ActionResult Index(int? id)
        {
            ViewBag.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");
            return base.Index(id);
        }

        [HttpPost]
        public JsonResult BatchDelRecord(string idsStr, string tbid)
        {
            string id = "";
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    id = arr[i].ToString().Trim();
                    comService.ExecuteSql("delete from " + tbid + " where ID=" + id + "");
                }
            }
            return Json(SuccessTip("删除成功"));
        }

        [HttpGet]
        public ActionResult RecordList(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }

        public JsonResult GetRecordList(string tbid)
        {
            RecordService recordService = new RecordService();
            if (string.IsNullOrEmpty(tbid))
            {
                return Json("参数idsStr为空！");
            }
            try
            {
                var list = recordService.GetTbRecord(tbid);
                var result = new { code = 0, msg = "", count = 999999, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult ListWriteRecord(string id)
        {
            ViewBag.name = "数据回写：" + comService.GetSingleField("select RelationName from relationlist where  RelationId=" + id) + " - 执行记录";
            ViewBag.id = id;

            return View();
        }

        [HttpGet]
        public ActionResult EventRecord(string id, string isSys)
        {
            ViewBag.id = id;
            if (isSys == "y")
            {
                ViewBag.name = "自定义事件：" + comService.GetSingleField("select EventId from tbeventcustomize where  Id=" + id + "") + " - 事件执行记录";
            }
            else
            {
                ViewBag.name = "自定义按钮：" + comService.GetSingleField("select BtnId from tbbutcustomize where  Id=" + id + "") + " - 事件执行记录";
            }

            ViewBag.isSys = string.IsNullOrEmpty(isSys) ? "y" : isSys;
            return View();
        }

        [HttpPost]
        public ActionResult GetTableTree()
        {
            List<TreeGridEntity> list = tbBasicService.GetTableTreeList();

            var menuJson = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            return Json(SuccessTip("操作成功", menuJson));
        }

        [HttpGet]
        public JsonResult GetMainTb(PageInfoEntity pageInfo, string sortid, string key)
        {
            TbBasicEntity model = new TbBasicEntity();
            pageInfo.field = "OrderNo";
            string where = "where TbType = '1' and isAux = '2' ";
            if (!string.IsNullOrEmpty(sortid))
            {
                where = "where TbType='1' and TbSort='" + sortid + "' and isAux = '2' ";
            }
            string _keywords = key == null ? "" : key;
            if (_keywords != "")
            {
                where += " and (TbId like @TbName OR TbName like @TbName) ";
                model.TbName = string.Format("%{0}%", _keywords);
            }

            try
            {
                var list = tbBasicService.GetMainTbList(model, true, where);
                var result = new { code = 0, msg = "", count = 999999, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetMainTbListOnlyModel(PageInfoEntity pageInfo, string sortid, string key)
        {
            TbBasicEntity model = new TbBasicEntity();
            pageInfo.field = "OrderNo";
            string where = "where TbType='1' and isAux = '2' ";
            if (!string.IsNullOrEmpty(sortid))
            {
                where = "where TbType='1' and TbSort='" + sortid + "' and isAux = '2' ";
            }
            string _keywords = key == null ? "" : key;
            if (_keywords != "")
            {
                where += " and (TbId like @TbName OR TbName like @TbName) ";
                model.TbName = string.Format("%{0}%", _keywords);
            }

            try
            {
                var list = tbBasicService.GetMainTbList(model, false, where).ToList();
                foreach (TbBasicEntity obj in list)
                {
                    if (obj.Model == "调试模式")
                    {
                        obj.Model = "<sapn style='color: #FFAB00;'>调试模式</sapn>";
                    }
                }
                var result = new { code = 0, msg = "", count = 999999, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult TbGrid(string tbid)
        {
            ViewBag.tbid = tbid;
            return View("~/Areas/SysTable/TableList/TbGrid.cshtml");
        }

        public ActionResult TbDCI(string tbid)
        {
            ViewBag.tbid = tbid;
            return View("~/Areas/SysTable/TableList/TbDCI.cshtml");
        }

        [HttpGet]
        public JsonResult GetGridTbList(PageInfoEntity pageInfo, string tbid)
        {
            pageInfo.field = "OrderNo";
            string where = "where ParentId='" + tbid + "'";

            var list = tbBasicService.GetByWhere(where, null, null).ToList();
            foreach (TbBasicEntity obj in list)
            {
                obj.indexCount = "{" + comService.GetTotal("tbindex", "where TbId='" + obj.TbId + "'").ToString() + "}";
                obj.TbRelationCount = "{" + comService.GetTotal("tbrelation", "where TbID='" + obj.TbId + "'").ToString() + "}";
                obj.TbRelationCount11 = "{" + comService.GetSingleField("SELECT COUNT(1) FROM tbrelation INNER JOIN relationlist ON tbrelation.RelationId = relationlist.RelationId where tbrelation.TbID='" + obj.TbId + "'  and relationlist.RelationType = '11' ") + "}";
                obj.TbRelationCount21 = "{" + comService.GetSingleField("SELECT COUNT(1) FROM tbrelation INNER JOIN relationlist ON tbrelation.RelationId = relationlist.RelationId where tbrelation.TbID='" + obj.TbId + "'  and relationlist.RelationType = '21' ") + "}";
                obj.TbRelationCount31 = "{" + comService.GetSingleField("SELECT COUNT(1) FROM tbrelation INNER JOIN relationlist ON tbrelation.RelationId = relationlist.RelationId where tbrelation.TbID='" + obj.TbId + "'  and relationlist.RelationType = '31' ") + "}";
                obj.ValueCheckCount = "{" + comService.GetTotal("tbvaluecheck", "where TbId='" + obj.TbId + "'").ToString() + "}";

                obj.EventCount = "{" + comService.GetTotal("tbeventcustomize", "where TbId='" + obj.TbId + "'").ToString() + "}";

                obj.BtnCustomizeCount = "{" + comService.GetTotal("tbbutcustomize", "where TbId='" + obj.TbId + "' OR GridId='" + obj.TbId + "'").ToString() + "}";

                obj.HiddenIndexCount = "{" + comService.GetTotal("tbhiddenindex", "where TbId='" + obj.TbId + "'").ToString() + "}";
            }

            return Json(new { code = 0, msg = "", count = list.Count(), data = list });
        }

        public ActionResult Warn(string idsStr, string from)
        {
            if (string.IsNullOrEmpty(idsStr))
            {
                return Json("参数idsStr为空！");
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

            ViewBag.tbid = idsStr;
            ViewBag.from = string.IsNullOrEmpty(from) ? "" : from;
            return View();
        }

        [HttpPost]
        public ActionResult GetWarnList(string idsStr, string from)
        {
            if (string.IsNullOrEmpty(idsStr))
            {
                return Json("参数idsStr为空！");
            }
            try
            {
                CheckCodeBuild _check = new CheckCodeBuild();
                List<WarnEntity> list = _check.GetTablesWarning(idsStr, CurrentUser);

                var result = new { code = 0, msg = "", count = list.Count, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult GetDCIList(string tbid)
        {
            var list = tbBasicService.GetTbDCIList(tbid);

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetDCIList2(string tbid)
        {
            var list = tbBasicService.GetTbDCIList2(tbid);

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }

        public ActionResult WarnDetails(string tbid)
        {
            string file = directoryPath + "\\wwwroot\\AutoCode\\" + tbid + "_tempcode.txt";
            string details = "";
            if (System.IO.File.Exists(file))
            {
                details = FileHelper.FileToString(file).Trim();
            }

            ViewBag.Details = details;
            return View();
        }

        public ActionResult CodeDetail(string tbid)
        {
            string file = directoryPath + "\\wwwroot\\AutoCode\\" + tbid + "_tempcode.txt";
            string details = "";
            if (System.IO.File.Exists(file))
            {
                details = FileHelper.FileToString(file).Trim();
            }

            ViewBag.Details = details;
            ViewBag.Path = "\\wwwroot\\AutoCode\\" + tbid + "_tempcode.txt";
            return View();
        }

        public ActionResult CodeList(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }
    }
}