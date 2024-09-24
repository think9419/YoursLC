using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class ExternalDbController : BaseController
    {
        private Think9.Services.Basic.ExternalDbService dbService = new Think9.Services.Basic.ExternalDbService();
        private ComService comService = new ComService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public JsonResult GetList(PageInfoEntity pageInfo)
        {
            pageInfo.field = "DbID";
            var list = dbService.GetAll("*", "order by DbID");
            var result = new { code = 0, msg = "", count = 9999999, data = list };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.TypeList = DbTypeList.GetList();
            return View();
        }

        [HttpPost]
        public ActionResult Add(ExtraDbEntity model)
        {
            if (comService.GetTotal("externaldb", "where DbName='" + model.DbName + "'") > 0)
            {
                return Json(ErrorTip("已存在相同名称"));
            }
            var result = dbService.Insert(model) ? SuccessTip("添加成功") : ErrorTip("添加失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult Connection(ExtraDbEntity model, string con, string type)
        {
            try
            {
                var connection = dbService.GetConnection(con, type);
                connection.Open();
                connection.Close();

                return Json(SuccessTip("连接成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            string where = "where DbID=" + id + " ";
            object param = new { DbID = id };

            var model = dbService.GetByWhereFirst(where, param);
            if (model != null)
            {
                ViewBag.TypeList = DbTypeList.GetList();
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(ExtraDbEntity model, string id)
        {
            if (comService.GetTotal("externaldb", "where DbName='" + model.DbName + "' AND DbID <> " + id + " ") > 0)
            {
                return Json(ErrorTip("已存在相同名称"));
            }

            return Json(dbService.UpdateByWhere("where DbID=" + id + "", "DbName,DbCon,Remarks", model) > 0 ? SuccessTip("编辑成功") : ErrorTip("编辑失败"));
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (comService.GetTotal("rulesingle", "where  DbID = " + id + " ") > 0)
            {
                return Json(ErrorTip("不能删除，还存在使用该连接的单列选择"));
            }
            if (comService.GetTotal("rulemultiple", "where  DbID = " + id + " ") > 0)
            {
                return Json(ErrorTip("不能删除，还存在使用该连接的多列选择"));
            }
            if (comService.GetTotal("ruletree", "where  DbID = " + id + " ") > 0)
            {
                return Json(ErrorTip("不能删除，还存在使用该连接的树形选择"));
            }
            if (comService.GetTotal("relationlist", "where  DbID = " + id + " ") > 0)
            {
                return Json(ErrorTip("不能删除，还存在使用该连接的数据读取"));
            }
            if (comService.GetTotal("reportbasic", "where  DbID = " + id + " ") > 0)
            {
                return Json(ErrorTip("不能删除，还存在使用该连接的统计报表"));
            }
            if (comService.GetTotal("indexstats", "where  DbID = " + id + " ") > 0)
            {
                return Json(ErrorTip("不能删除，还存在使用该连接的统计指标"));
            }

            var result = dbService.DeleteByWhere("where DbID=" + id + " ") ? SuccessTip("删除成功") : ErrorTip("删除失败");

            return Json(result);
        }
    }
}