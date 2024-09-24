using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbButCustomizeController : BaseController
    {
        private TbButCustomizeService tbButCustomize = new TbButCustomizeService();
        private TbButCustomizeEventService tbButCustomizeEvent = new TbButCustomizeEventService();
        private TbEventParaService tbEventParaService = new TbEventParaService();

        //
        [HttpGet]
        public JsonResult GetSelectValueField(string id)
        {
            try
            {
                return Json(tbButCustomize.GetSelectValueFieldListById(id == null ? "" : id));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetParaList(string id)
        {
            try
            {
                var result = new { code = 0, msg = "", count = 999999, data = tbEventParaService.GetByWhere(" where BtnId=" + id + " and Frm='2'", null, null, "order by id") };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult AddPara2(string tbid, string id, string type, string name, string value)
        {
            string err = "";
            try
            {
                err = tbEventParaService.AddPara2(tbid, id, type, name, value);
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (err == "")
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult DeletePara(string id)
        {
            string err = tbEventParaService.DelPara(id);

            if (err == "")
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public JsonResult GetList(string tbid)
        {
            string where = "where TbId='" + tbid + "' ";
            var result = new { code = 0, msg = "", count = 999999, data = tbButCustomize.GetByWhere("where TbId='" + tbid + "' ") };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetEventList(string guid, string id)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tbButCustomize.GetEventListById(id, guid) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add(string tbid)
        {
            ViewBag.Guid = System.Guid.NewGuid().ToString("N");
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;

            ViewBag.SelectList = tbButCustomize.GetSelectList(tbid, "list");

            return View();
        }

        [HttpPost]
        public ActionResult Add(string tbid, string guid, TbButCustomizeEntity model)
        {
            model.BtnId = "btn2_" + model.BtnId;
            if (model.TbId == tbid)
            {
                model.GridId = "";
            }
            else
            {
                model.GridId = model.TbId;
                model.TbId = tbid;
            }

            try
            {
                string err = tbButCustomize.AddBut(model, guid);
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
        public ActionResult Edit(string id, string tbid)
        {
            ViewBag.SelectList = tbButCustomize.GetSelectList(tbid, "list");
            var model = tbButCustomize.GetByWhereFirst("where Id=@Id", new { Id = id });
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
        public ActionResult Edit(TbButCustomizeEntity model, string id)
        {
            model.UpdateTime = DateTime.Now;

            string where = "where Id=" + id + "";
            string updateFields = "BtnText,BtnWarn,Remarks,UpdateTime,Icon";

            var result = tbButCustomize.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpGet]
        public ActionResult AddEvent(string id, string Guid, string tbid)
        {
            string type = string.IsNullOrEmpty(Guid) ? "1" : "0";
            ViewBag.Guid = string.IsNullOrEmpty(Guid) ? System.Guid.NewGuid().ToString("N") : Guid;
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            ViewBag.Id = string.IsNullOrEmpty(id) ? "0" : id;
            ViewBag.type = type;

            return View();
        }

        [HttpPost]
        public ActionResult AddEvent(string id, string tbid, string guid, TbButCustomizeEventEntity model)
        {
            id = string.IsNullOrEmpty(id) ? "" : id;//BtnId
            guid = string.IsNullOrEmpty(guid) ? "" : guid;
            model.ExecuteSql = model.ExecuteSql == null ? "" : BaseUtil.ReplaceHtml(model.ExecuteSql);
            try
            {
                string err = tbButCustomizeEvent.AddEvent(model, guid, tbid, id);
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
        public ActionResult EditEvent(string id)
        {
            ViewBag.Id = id;
            var model = tbButCustomizeEvent.GetByWhereFirst("where Id=@Id", new { Id = id });
            if (model != null)
            {
                ViewBag.TbId = model.TbId;
                if (model.ExecuteType == "2")
                {
                    model.ProcedureName = model.ExecuteSql;
                }
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult EditEvent(string id, TbEventCustomizeEntity model)
        {
            if (model.ExecuteType == "2")
            {
                model.ExecuteSql = model.ProcedureName;
                if (string.IsNullOrEmpty(model.ExecuteSql))
                {
                    return Json(ErrorTip("请输入存储过程"));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.ExecuteSql))
                {
                    return Json(ErrorTip("请输入sql语句"));
                }
            }
            model.ExecuteSql = model.ExecuteSql == null ? "" : BaseUtil.ReplaceHtml(model.ExecuteSql);

            string where = "where Id=" + id + "";
            string updateFields = "ExecuteSql,OrderNo,FullName,Remarks";

            var result = tbButCustomizeEvent.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("编辑失败");
            return Json(result);
        }

        public ActionResult DeleteButEvent(string id, string btnId)
        {
            tbButCustomize.DelButEvent(id, btnId);
            return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
        }

        [HttpGet]
        public JsonResult DeleteBut(string id, string tbid)
        {
            string err = tbButCustomize.DelBut(id, tbid);
            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("删除成功"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }
    }
}