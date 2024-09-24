using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Stats;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class IndexStatsController : BaseController
    {
        private ComService comService = new ComService();
        private TbBasicService tbServer = new TbBasicService();
        private SortService sortService = new SortService();
        private IndexStatsService indexStatsService = new IndexStatsService();
        private IndexParmService indexParmService = new IndexParmService();
        private ReportService reportService = new ReportService();

        private string _dbtype = Think9.Services.Base.Configs.GetDBProvider("DBProvider");//数据库类型

        private SelectList SortList
        { get { return new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_indexstats"), "SortID", "SortName"); } }

        [HttpGet]
        public JsonResult GetConditionFieldList(string id, string dbId)
        {
            TbRelationService tbRelationService = new TbRelationService();
            RuleServiceBasic ruleService = new RuleServiceBasic();
            dbId = string.IsNullOrEmpty(dbId) ? "0" : dbId;

            try
            {
                if (dbId == "0")
                {
                    return Json(tbRelationService.GetConditionFieldList(id));
                }
                return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                List<valueTextEntity> list = new List<valueTextEntity>();
                list.Add(new valueTextEntity { ClassID = "err", Value = "", Text = ex.Message });
                return Json(list);
            }
        }

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            ViewBag.SortList = SortList;
            return base.Index(id);
        }

        [HttpGet]
        public ActionResult Add(string frm)
        {
            ExternalDbService externalDbService = new ExternalDbService();
            TbFieldService tbFieldService = new TbFieldService();

            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.SortList = SortList;
            //可选择的数据源绑定
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(tbServer.GetMainAndGridTb(""));
            ViewBag.ViewList = tbFieldService.GetViewsList();
            ViewBag.DbList = DataTableHelp.ToEnumerable<valueTextEntity>(externalDbService.GetList());

            return View();
        }

        [HttpPost]
        public ActionResult Add(IndexStatsEntity model)
        {
            try
            {
                model.SelectField = model.SelectField == null ? "" : BaseUtil.ReplaceHtml(model.SelectField);
                model.WhereStr = model.WhereStr == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr);

                string err = indexStatsService.Add(model);
                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
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

        [HttpGet]
        public ActionResult Edit(string frm, string id)
        {
            var model = indexStatsService.GetByWhereFirst("where IndexId=@IndexId", new { IndexId = id });
            if (model != null)
            {
                ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
                ViewBag.SortList = SortList;
                model.DbID_Exa = ExternalDbService.GetName(model.DbID == null ? 0 : (int)model.DbID);
                model.FromTbId_Exa = "从{" + model.FromTbId + "}取值";
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(IndexStatsEntity model)
        {
            model.UpdateTime = DateTime.Now;
            string where = "where IndexId='" + model.IndexId + "'";
            string updateFields = "Units,IndexSort,IndexName,Digit,SelectField,WhereStr,IndexExplain,UpdateTime";
            try
            {
                model.IndexName = ReportHelp.CleanInvalidHtmlChars(model.IndexName);
                model.SelectField = model.SelectField == null ? "" : BaseUtil.ReplaceHtml(model.SelectField);
                model.WhereStr = model.WhereStr == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr);

                var result = indexStatsService.Update(where, updateFields, model) > 0 ? SuccessTip("编辑成功，修改后需『重新生成』才能生效") : ErrorTip("编辑失败");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult GetList(PageInfoEntity pageInfo, string key, string type)
        {
            IndexStatsEntity model = new IndexStatsEntity();
            key = key == null ? "" : key;
            long total = 0;

            pageInfo.returnFields = "*";
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string where = "where (1=1)";
            if (key != "")
            {
                where += " and (IndexId like @IndexId OR IndexName like @IndexName) ";
                model.IndexId = string.Format("%{0}%", key);
                model.IndexName = string.Format("%{0}%", key);
            }

            if (!String.IsNullOrEmpty(type))
            {
                where += " and (IndexSort = @IndexSort) ";
                model.IndexSort = type;
            }

            try
            {
                var list = indexStatsService.GetPageList(model, pageInfo, where, _dbtype, ref total).ToList();
                foreach (IndexStatsEntity obj in list)
                {
                    if (obj.DbID != null && obj.DbID != 0)
                    {
                        obj.IndexName += " <i class='fa fa-external-link'></i>";
                    }
                }
                var result = new { code = 0, msg = "", count = total, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult GetIndexListBySearch(PageInfoEntity pageInfo, string type, string key)
        {
            IndexStatsEntity model = new IndexStatsEntity();
            model.Type = type;
            key = key == null ? "" : key;

            pageInfo.returnFields = "IndexId,IndexName";
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string where = "where Type = @type";
            if (string.IsNullOrEmpty(type))
            {
                where = "where 1 = 1 ";
            }

            if (!string.IsNullOrEmpty(key))
            {
                where += " and (IndexId like @IndexId OR IndexName like @IndexName) ";
                model.IndexId = string.Format("%{0}%", key);
                model.IndexName = string.Format("%{0}%", key);
            }

            long total = 0;
            var list = indexStatsService.GetPageByFilter(ref total, model, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            string where = "where IndexId='" + id + "'";
            var result = indexStatsService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("操作失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult BatchDel(string idsStr)
        {
            string id = "";
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    id = arr[i].ToString().Trim();
                    string where = "where IndexId='" + id + "'";

                    indexStatsService.DeleteByWhere(where);
                }
            }
            return Json(SuccessTip("删除成功"));
        }

        [HttpPost]
        public ActionResult GetIndexORParmListBySearch(PageInfoEntity pageInfo, string sort, string key)
        {
            sort = sort == null ? "" : sort;
            key = key == null ? "" : key;
            long total = 0;

            pageInfo.returnFields = "*";
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string where = "where 1=1 ";
            if (sort.Contains("@"))
            {
                IndexParmEntity model = new IndexParmEntity();

                if (key != "")
                {
                    where += " and (ParmId like @ParmId OR ParmName like @ParmName) ";
                    model.ParmId = string.Format("%{0}%", key);
                    model.ParmName = string.Format("%{0}%", key);
                }

                var list = indexParmService.GetPageByFilter(ref total, model, pageInfo, where);
                foreach (IndexParmEntity obj in list)
                {
                    obj.Value = obj.ParmId;
                    obj.Text = "@" + obj.ParmName;
                }

                return Json(new { code = 0, msg = "", count = total, data = list });
            }
            else
            {
                IndexStatsEntity model = new IndexStatsEntity();
                if (sort != "")
                {
                    where += " and IndexSort = @IndexSort ";
                    model.IndexSort = sort;
                }
                if (key != "")
                {
                    where += " and (IndexId like @IndexId OR IndexName like @IndexName) ";
                    model.IndexId = string.Format("%{0}%", key);
                    model.IndexName = string.Format("%{0}%", key);
                }

                var list = indexStatsService.GetPageByFilter(ref total, model, pageInfo, where);
                foreach (IndexStatsEntity obj in list)
                {
                    obj.Value = obj.IndexId;
                    if (obj.DbID != 0 && obj.DbID != null)
                    {
                        obj.Text = obj.IndexName + "<i class='fa fa-external-link'></i>";
                    }
                    else
                    {
                        obj.Text = obj.IndexName;
                    }
                }
                var result = new { code = 0, msg = "", count = total, data = list };
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult ShowIndexDetails(string id)
        {
            string type = "";
            ViewBag.id = string.IsNullOrEmpty(id) ? "" : id;
            ViewBag.Details = indexStatsService.GetSqlStrById(ref type, id);
            ViewBag.Explain = "";
            if (type == "index")
            {
                ViewBag.Explain = reportService.GetIndexExplain(ViewBag.Details);
            }
            return View();
        }
    }
}