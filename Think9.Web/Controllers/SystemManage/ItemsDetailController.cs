using Microsoft.AspNetCore.Mvc;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Web.SystemOrganize.Controllers
{
    public class ItemsDetailController : BaseController
    {
        private ItemsService ItemsService = new ItemsService();
        private ItemsDetailService ItemsDetailService = new ItemsDetailService();

        public ActionResult ListDetail(string id)
        {
            ViewBag.ClassID = id;
            return View();
        }

        public JsonResult GetDetailList(ItemsDetailEntity model, PageInfoEntity pageInfo)
        {
            string classid = HttpContext.Request.Query["classid"].ToString();
            pageInfo.field = "OrderNo";
            var result = ItemsDetailService.GetPageByFilter(model, pageInfo, "where ItemCode='" + classid + "'");
            return Json(result);
        }

        public ActionResult AddDetail()
        {
            ViewBag.ClassID = HttpContext.Request.Query["classid"].ToString();

            return View();
        }

        [HttpPost]
        public ActionResult AddDetail(ItemsDetailEntity model)
        {
            string where = "where ItemCode=@ItemCode and DetailCode=@DetailCode";
            object param = new { ItemCode = model.ItemCode, DetailCode = model.DetailCode };
            if (ItemsDetailService.GetTotal(where, param) > 0)
            {
                var result = ErrorTip("添加失败!已存在相同编码");
                return Json(result);
            }
            else
            {
                var result = ItemsDetailService.Insert(model) ? SuccessTip("添加成功") : ErrorTip("添加失败");
                return Json(result);
            }
        }

        public ActionResult EditDetail(string id, string classid)
        {
            ViewBag.ClassID = classid;
            string where = "where ItemCode=@ClassID and DetailCode=@id";
            object param = new { ClassID = classid, id = id };

            var model = ItemsDetailService.GetByWhereFirst(where, param);

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Json(ErrorTip("ERR：数据不存在！！！"));
            }
        }

        [HttpPost]
        public ActionResult EditDetail(ItemsDetailEntity model)
        {
            string where = "where ItemCode='" + model.ItemCode + "' and DetailCode='" + model.DetailCode + "'";
            string updateFields = "DetailName,OrderNo";

            var result = ItemsDetailService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("修改成功") : ErrorTip("修改失败");

            return Json(result);
        }

        [HttpGet]
        public JsonResult DeleteDetail(string id, string classid)
        {
            string where = "where ItemCode='" + classid + "' and DetailCode='" + id + "'";
            var result = ItemsDetailService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("删除失败");
            return Json(result);
        }
    }
}