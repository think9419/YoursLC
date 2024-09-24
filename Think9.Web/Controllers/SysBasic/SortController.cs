using Microsoft.AspNetCore.Mvc;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class SortController : BaseController
    {
        private SortService sortService = new SortService();
        private ComService comService = new ComService();

        public ActionResult List(string classid)
        {
            if (string.IsNullOrEmpty(classid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.ClassID = classid;
                return View();
            }
        }

        [HttpGet]
        public JsonResult GetList(SortEntity model, PageInfoEntity pageInfo, string classid)
        {
            if (string.IsNullOrEmpty(classid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                pageInfo.field = "SortOrder";
                var result = sortService.GetPageByFilter(model, pageInfo, "where classid='" + classid + "'");
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult Add(string classid)
        {
            if (string.IsNullOrEmpty(classid))
            {
                return Json(ErrorTip("参数错误"));
            }
            else
            {
                ViewBag.ClassID = classid;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Add(SortEntity model, string classid)
        {
            model.ClassID = classid;

            string where = "where ClassID=@ClassID and SortID=@SortID";
            object param = new { ClassID = model.ClassID, SortID = model.SortID };
            if (sortService.GetTotal(where, param) > 0)
            {
                var result = ErrorTip("添加失败!已存在相同分类编码");
                return Json(result);
            }
            else
            {
                var result = sortService.Insert(model) ? SuccessTip("添加成功") : ErrorTip("添加失败");
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult Edit(string classid, string sortid)
        {
            ViewBag.ClassID = classid;
            string where = "where ClassID=@ClassID and SortID=@SortID";
            object param = new { ClassID = classid, SortID = sortid };

            var model = sortService.GetByWhereFirst(where, param);

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
        public ActionResult Edit(SortEntity model, string classid)
        {
            string where = "where ClassID='" + classid + "' and SortID='" + model.SortID + "'";
            string updateFields = "SortName,SortOrder";

            var result = sortService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("编辑成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(string classid, string sortid)
        {
            string where = "";
            string err = "";
            bool isExit = false;
            if (classid == "CAT_index")
            {
                where = "where IndexSort='" + sortid + "'";
                if (comService.GetTotal("indexbasic", where) > 0)
                {
                    isExit = true;
                    err = "不能删除，还包含该分类的指标";
                }
            }
            if (classid == "CAT_table")
            {
                where = "where TbSort='" + sortid + "'";
                if (comService.GetTotal("tbbasic", where) > 0)
                {
                    isExit = true;
                    err = "不能删除，还包含该分类的录入表";
                }
            }
            if (classid == "CAT_dict")
            {
                where = "where ItemSort='" + sortid + "'";
                if (comService.GetTotal("sys_items", where) > 0)
                {
                    isExit = true;
                    err = "不能删除，还包含该分类的数据字典";
                }
            }
            if (classid == "CAT_indexstats")
            {
                where = "where IndexSort='" + sortid + "'";
                if (comService.GetTotal("indexstats", where) > 0)
                {
                    isExit = true;
                    err = "不能删除，还包含该分类的统计指标";
                }
            }
            if (classid == "CAT_report")
            {
                where = "where ReportSort='" + sortid + "'";
                if (comService.GetTotal("reportbasic", where) > 0)
                {
                    isExit = true;
                    err = "不能删除，还包含该分类的录入表";
                }
            }
            if (!isExit)
            {
                where = "where ClassID='" + classid + "' and SortID='" + sortid + "'";
                var result = sortService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("删除失败");
                return Json(result);
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }
    }
}