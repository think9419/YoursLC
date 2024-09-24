using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Com;
using Think9.Services.Flow;

namespace Think9.Controllers.Basic
{
    [Area("SysFlow")]
    public class FlowPrcsController : BaseController
    {
        private FlowService flowervice = new FlowService();
        private FlowPrcsService flowPrcsService = new FlowPrcsService();
        private FlowPrcsNextService flowPrcsNextService = new FlowPrcsNextService();
        private ComService comService = new ComService();

        public ActionResult List(string id, string tbid)
        {
            string err = "";
            ViewBag.flowId = id;
            ViewBag.tbid = tbid;
            try
            {
                FlowEntity flow = flowervice.GetByWhereFirst("where FlowId=@flid", new { flid = id });
                if (flow == null)
                {
                    err = "流程对象为空！";
                }
                else
                {
                    if (flow.flowType == "0")
                    {
                        err = "未定义流程，不能定义流程步骤！";
                    }
                    if (flow.flowType == "2")
                    {
                        err = "自由流程，无需定义流程步骤！";
                    }
                }

                if (err == "")
                {
                    return View();
                }
                else
                {
                    return Json(err);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        [HttpGet]
        public JsonResult GetListByFId(FlowPrcsEntity model, PageInfoEntity pageInfo, string id)
        {
            try
            {
                var result = new { code = 0, msg = "", count = 999999, data = flowPrcsService.GetListByFId(model, pageInfo, id) };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        public JsonResult GetNextListById(string id, string fid)
        {
            try
            {
                var result = new { code = 0, msg = "", count = 999999, data = flowPrcsNextService.GetNextListById(id, fid) };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        public ActionResult AddPrcs(string id, string tbid)
        {
            string name = "";
            FlowEntity flow = flowervice.GetByWhereFirst("where FlowId=@flid", new { flid = id });
            if (flow != null)
            {
                name = flow.FlowName;
            }
            ViewBag.flowId = id;
            ViewBag.tbid = tbid;
            ViewBag.name = name;

            return View();
        }

        public ActionResult DelPrcs(string id)
        {
            string err = "";
            try
            {
                if (comService.GetTotal("flowrunlist", "where currentPrcsId=" + id + "") > 0)
                {
                    err = "不能删除，还存在当前流程步骤的数据";
                }
                else
                {
                    comService.ExecuteSql("delete from flowprcsnext where PrcsId=" + id + "");//先删除
                    comService.ExecuteSql("delete from flowprcs where PrcsId=" + id + "");
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
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        [HttpPost]
        public ActionResult AddPrcs(FlowPrcsEntity model)
        {
            string err = "";
            try
            {
                if (comService.GetTotal("flowprcs", "where FlowId=@FlowId and PrcsNo=@PrcsNo", model) > 0)
                {
                    err = "已存在相同步骤编码";
                }
                else
                {
                    flowPrcsService.GetDefaultModel(ref model);
                    model.BAttachment = model.A1 + model.A2 + model.A3;
                    model.PrcsName = FlowHelp.CleanInvalidHtmlChars(model.PrcsName);

                    err = flowPrcsService.Insert(model) ? "" : "操作失败";
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
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        public ActionResult EditPrcs(string id, string fid, string tbid)
        {
            CommonSelectService comSelect = new CommonSelectService();
            string name = "";
            FlowEntity flow = flowervice.GetByWhereFirst("where FlowId=@flid", new { flid = fid });
            if (flow != null)
            {
                name = flow.FlowName;
            }

            ViewBag.flowId = fid;
            ViewBag.tbid = tbid;
            ViewBag.name = name;

            FlowPrcsEntity model = flowPrcsService.GetByWhereFirst("where PrcsId=@id", new { id = id });
            if (model != null)
            {
                model.PrcsIndex_Exa = CommonSelectService.GetIndexAndSonTbListByStr(tbid, model.PrcsIndex);
                model.HiddenIndex_Exa = CommonSelectService.GetIndexAndSonTbListByStr(tbid, model.HiddenIndex);
                model.PrcsUser_Exa = CommonSelectService.GetNameStrByIdStr(model.PrcsUser, "1");
                model.PrcsDept_Exa = CommonSelectService.GetNameStrByIdStr(model.PrcsDept, "2");
                model.PrcsPriv_Exa = CommonSelectService.GetNameStrByIdStr(model.PrcsPriv, "3");

                model.BAttachment = model.BAttachment == null ? "" : model.BAttachment;
                if (model.BAttachment.Length >= 3)
                {
                    model.A1 = model.BAttachment.Substring(0, 1);
                    model.A2 = model.BAttachment.Substring(1, 1);
                    model.A3 = model.BAttachment.Substring(2, 1);
                }
                else
                {
                    model.A1 = "2";
                    model.A2 = "2";
                    model.A3 = "2";
                }
                return View(model);
            }
            else
            {
                return Json(ErrorTip("数据不存在！！！"));
            }
        }

        public ActionResult PrcsTransactor(string id)
        {
            if (id == "0")
            {
                FlowPrcsEntity model = new FlowPrcsEntity();
                model.PrcsUser_Exa = "";
                model.PrcsDept_Exa = "";
                model.PrcsPriv_Exa = "";
                return View(model);
            }
            else
            {
                FlowPrcsEntity model = flowPrcsService.GetByWhereFirst("where PrcsId=@id", new { id = id });
                if (model != null)
                {
                    model.PrcsUser_Exa = CommonSelectService.GetNameStrByIdStr(model.PrcsUser, "1");
                    model.PrcsDept_Exa = CommonSelectService.GetNameStrByIdStr(model.PrcsDept, "2");
                    model.PrcsPriv_Exa = CommonSelectService.GetNameStrByIdStr(model.PrcsPriv, "3");
                    return View(model);
                }
                else
                {
                    return Json(ErrorTip("数据不存在！！！"));
                }
            }
        }

        [HttpPost]
        public ActionResult EditPrcs(FlowPrcsEntity model)
        {
            model.BAttachment = model.A1 + model.A2 + model.A3;
            var result = flowPrcsService.UpdateById(model) ? SuccessTip("保存成功") : ErrorTip("操作失败");
            return Json(result);
        }

        public ActionResult AddNext(string id, FlowPrcsNextEntity model)
        {
            string err = "";
            try
            {
                model.PrcsId = int.Parse(id);
                if (comService.GetTotal("flowprcsnext", "where PrcsId = @PrcsId and NextPrcsId = @NextPrcsId", model) > 0)
                {
                    err = "重复添加";
                }
                else
                {
                    err = flowPrcsNextService.Insert(model) ? "" : "操作失败";
                }
                if (err == "")
                {
                    return Json(SuccessTip("保存成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }

        [HttpPost]
        public ActionResult DelNext(string id)
        {
            string err = flowPrcsNextService.DeleteById(int.Parse(id)) ? "" : "操作失败";
            if (err == "")
            {
                return Json(SuccessTip("删除成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        public ActionResult NextList(string id, string fId)
        {
            ViewBag.FId = fId;
            ViewBag.Id = id;
            ViewBag.SelectList = flowPrcsNextService.GetSelectNextList(id, fId);

            return View();
        }

        [HttpPost]
        public ActionResult UpSomeByID(string flowid, string id, string name, bool isState)
        {
            try
            {
                string where = "where PrcsId = " + id + "";
                FlowPrcsEntity model = new FlowPrcsEntity();

                if (name == "isFirst")
                {
                    model.isFirst = 2;
                    if (isState)
                    {
                        model.isFirst = 1;
                    }
                }
                var result = flowPrcsService.UpdateByWhere(where, name, model) > 0 ? SuccessTip("保存成功") : ErrorTip("操作失败");
                if (name == "isFirst" && isState)
                {
                    where = "where PrcsId <> " + id + " and flowid='" + flowid + "'";
                    model.isFirst = 2;
                    flowPrcsService.UpdateByWhere(where, name, model);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, count = -1 });
            }
        }
    }
}