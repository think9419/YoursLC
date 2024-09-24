using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class ItemsController : BaseController
    {
        private ItemsService itemsService = new ItemsService();
        private ItemsDetailService itemsDetailService = new ItemsDetailService();
        private SortService sortService = new SortService();
        private ComService comService = new ComService();
        private RuleListService ruleListService = new RuleListService();

        //分类
        private SelectList SortList
        { get { return new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_dict"), "SortID", "SortName"); } }

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            ViewBag.SortList = sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_dict");

            return base.Index(id);
        }

        [HttpPost]
        public JsonResult GetPageListBySearch(ItemsEntity model, PageInfoEntity pageInfo, string key, string sort)
        {
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            sort = string.IsNullOrEmpty(sort) ? "" : sort;
            key = string.IsNullOrEmpty(key) ? "" : key;

            string str = "";
            long count;
            string where = "where 1=1 ";
            if (sort != "")
            {
                where += " and ItemSort=@ItemSort ";
                model.ItemSort = sort;
            }
            if (key != "")
            {
                where += " and (EnCode like @EnCode OR FullName like @EnCode OR Remarks like @EnCode) ";
                model.EnCode = string.Format("%{0}%", key);
            }

            long total = 0;
            IEnumerable<dynamic> list = itemsService.GetPageByFilter(ref total, model, pageInfo, where);

            string sql = "select * from sys_sort where ClassID='CAT_dict' ORDER BY SortOrder";
            DataTable dtSort = comService.GetDataTable(sql);

            sql = @"SELECT a.RuleName,a.RuleId,b.TbId,b.DictItemId FROM rulelist a
                           INNER JOIN rulesingle b ON a.RuleId=b.RuleId where b.TbId = 'sys_itemsdetail' ";
            DataTable dtRuleSingle = comService.GetDataTable(sql);

            sql = @"SELECT a.RuleName,a.RuleId,b.TbId,b.DictItemId FROM rulelist a
                           INNER JOIN rulemultiple b ON a.RuleId=b.RuleId where b.TbId = 'sys_itemsdetail' ";
            DataTable dtRuleMultiple = comService.GetDataTable(sql);

            sql = @"SELECT a.RuleName,a.RuleId,b.TbId,b.DictItemId FROM rulelist a
                           INNER JOIN ruletree b ON a.RuleId=b.RuleId where b.TbId = 'sys_itemsdetail' ";
            DataTable dtRuleTree = comService.GetDataTable(sql);

            foreach (Object obj in list)
            {
                if (obj is ItemsEntity)
                {
                    foreach (DataRow dr in dtSort.Rows)
                    {
                        if (dr["SortID"].ToString() == ((ItemsEntity)obj).ItemSort)
                        {
                            ((ItemsEntity)obj).ItemSort = dr["SortName"].ToString();
                        }
                    }

                    where = " where ItemCode=@ItemCode";
                    count = comService.GetTotal("sys_itemsdetail", where, new { ItemCode = ((ItemsEntity)obj).EnCode });
                    switch (count)
                    {
                        case long n when (n >= 1 && n <= 10):
                            str = "{ " + count.ToString() + " }";
                            break;

                        case long n when (n > 10 && n <= 99):
                            str = "{" + count.ToString() + "}";
                            break;

                        case long n when (n >= 99):
                            str = "{99+}";
                            break;

                        default:
                            str = "{" + count.ToString() + "}";
                            break;
                    }

                    ((ItemsEntity)obj).Amount = str;

                    ((ItemsEntity)obj).Used = ruleListService.GetDictUsedStr(dtRuleSingle, dtRuleMultiple, dtRuleTree, ((ItemsEntity)obj).EnCode);
                }
            }
            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        public ActionResult Add()
        {
            ViewBag.SortList = SortList;
            return View();
        }

        [HttpPost]
        public ActionResult Add(ItemsEntity model, string isAddRule)
        {
            if (comService.GetTotal("sys_items", " where EnCode=@EnCode", new { EnCode = model.EnCode }) > 0)
            {
                return Json(ErrorTip("已存在相同字典编码"));
            }

            try
            {
                string err = itemsService.AddItems(model);
                if (!string.IsNullOrEmpty(err))
                {
                    return Json(ErrorTip(err));
                }
                if (isAddRule == "y")
                {
                    RuleSingleService ruleSingleService = new RuleSingleService();
                    RuleSingleEntity entity = new RuleSingleEntity();
                    entity.DbID = 0;
                    entity.Name = model.FullName;
                    entity.TbId = "#dict#" + model.EnCode;
                    entity.TxtFiled = "DetailName";
                    entity.ValueFiled = "DetailCode";
                    entity.OrderFiled = "OrderNo";
                    entity.OrderType = "1";

                    err = ruleSingleService.Add(entity);
                    if (!string.IsNullOrEmpty(err))
                    {
                        return Json(ErrorTip("添加单列选择出现错误：" + err));
                    }
                }

                return Json(SuccessTip("操作成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.SortList = SortList;

            var model = itemsService.GetByWhereFirst("where EnCode=@EnCode", new { EnCode = id });
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
        public ActionResult Edit(ItemsEntity model)
        {
            string where = "where EnCode='" + model.EnCode + "'";
            string updateFields = "ItemSort,FullName,OrderNo,UpdateTime,Remarks";
            model.UpdateTime = DateTime.Now;
            model.FullName = TableHelp.CleanInvalidHtmlChars(model.FullName);

            try
            {
                var result = itemsService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            string err = "";

            string where = "where EnCode='" + id + "'";
            if (itemsService.DeleteByWhere(where))
            {
                where = "where ItemCode='" + id + "'";
                itemsDetailService.DeleteByWhere(where);
            }
            else
            {
                err = "删除失败";
            }

            if (err == "")
            {
                return Json(SuccessTip("删除成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
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
                    if (arr[i].ToString().Trim() != "")
                    {
                        id = arr[i].ToString().Trim();
                        string where = "where EnCode='" + id + "'";
                        if (itemsService.DeleteByWhere(where))
                        {
                            where = "where ItemCode='" + id + "'";
                            itemsDetailService.DeleteByWhere(where);
                        }
                    }
                }
            }

            return Json(SuccessTip("删除成功"));
        }

        public ActionResult ListDetail(string id)
        {
            ViewBag.ClassID = id;
            return View();
        }

        [HttpPost]
        public JsonResult GetDetailList(ItemsDetailEntity model, PageInfoEntity pageInfo, string classid)
        {
            pageInfo.field = "OrderNo";
            var result = itemsDetailService.GetPageByFilter(model, pageInfo, "where ItemCode='" + classid + "'");
            return Json(result);
        }

        public ActionResult AddDetail(string classid)
        {
            ViewBag.ClassID = classid;

            return View();
        }

        [HttpPost]
        public ActionResult AddDetail(ItemsDetailEntity model)
        {
            string where = "where ItemCode=@ItemCode and DetailCode=@DetailCode";
            object param = new { ItemCode = model.ItemCode, DetailCode = model.DetailCode };
            if (itemsDetailService.GetTotal(where, param) > 0)
            {
                return Json(ErrorTip("添加失败!已存在相同子项编码"));
            }

            try
            {
                string err = itemsDetailService.AddItemsDetail(model);

                var result = string.IsNullOrEmpty(err) ? SuccessTip("操作成功") : ErrorTip(err);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult EditDetail(string id, string classid)
        {
            ViewBag.ClassID = classid;
            string where = "where ItemCode=@ClassID and DetailCode=@id";
            object param = new { ClassID = classid, id = id };

            var model = itemsDetailService.GetByWhereFirst(where, param);
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
        public ActionResult EditDetail(ItemsDetailEntity model)
        {
            string where = "where ItemCode='" + model.ItemCode + "' and DetailCode='" + model.DetailCode + "'";
            string updateFields = "DetailName,OrderNo";

            try
            {
                var result = itemsDetailService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult DelDetail(string id, string classid)
        {
            string where = "where ItemCode='" + classid + "' and DetailCode='" + id + "'";
            var result = itemsDetailService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("删除失败");
            return Json(result);
        }
    }
}