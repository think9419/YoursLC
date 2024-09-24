using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Stats;

namespace Think9.Controllers.Basic
{
    [Area("SysStats")]
    public class IndexParmController : BaseController
    {
        private ComService comService = new ComService();
        private IndexParmService indexParmService = new IndexParmService();

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public ActionResult Add(string frm)
        {
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            return View();
        }

        [HttpPost]
        public ActionResult Add(IndexParmEntity model)
        {
            string err = indexParmService.Add(model);
            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = indexParmService.GetByWhereFirst("where ParmId=@ParmId", new { ParmId = id });
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(IndexParmEntity model)
        {
            model.UpdateTime = DateTime.Now;
            model.ParmName = ReportHelp.CleanInvalidHtmlChars(model.ParmName);

            string where = "where ParmId='" + model.ParmId + "'";
            string updateFields = "ParmName,UpdateTime,ParmExplain";
            var result = indexParmService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetListBySearch(IndexParmEntity model, PageInfoEntity pageInfo, string key)
        {
            key = key == null ? "" : key;
            pageInfo.returnFields = "*";
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string where = "where 1=1 ";
            if (key != "")
            {
                where += " and (ParmId like @ParmId OR ParmName like @ParmName) ";
                model.ParmId = string.Format("%{0}%", key);
                model.ParmName = string.Format("%{0}%", key);
            }

            long total = 0;
            DataTable dt = comService.GetDataTable("select * from indexstats ");
            var list = indexParmService.GetPageByFilter(ref total, model, pageInfo, where);
            foreach (IndexParmEntity obj in list)
            {
                obj.Used = indexParmService.GetUsedStr(dt, obj.ParmId);
            }

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        public ActionResult GetList(string key)
        {
            key = key == null ? "" : key;
            IndexParmEntity model = new IndexParmEntity();

            string where = "where 1=1 ";
            if (key != "")
            {
                where += " and (ParmId like @ParmId OR ParmName like @ParmName) ";
                model.ParmId = string.Format("%{0}%", key);
                model.ParmName = string.Format("%{0}%", key);
            }

            var result = new { code = 0, msg = "", count = 999999, data = indexParmService.GetByWhere(where) };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetParmType(string id)
        {
            string type = "string";
            var model = indexParmService.GetByWhereFirst("where ParmId=@ParmId", new { ParmId = id });
            if (model != null)
            {
                type = model.DataType;
            }

            return Json(SuccessTip("", type));
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            var result = indexParmService.DeleteByWhere("where ParmId='" + id + "'") ? SuccessTip("删除成功") : ErrorTip("操作失败");
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
                    string where = "where ParmId='" + id + "'";

                    indexParmService.DeleteByWhere(where);
                }
            }
            return Json(SuccessTip("删除成功"));
        }
    }
}