using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class SqlTempletController : BaseController
    {
        private TbEventService tbEvent = new TbEventService();
        private TbBasicService tbServer = new TbBasicService();
        private TbFieldService tbFieldService = new TbFieldService();
        private ComService comService = new ComService();
        private SysTempService tempService = new SysTempService();
        private TbEventParaService tbEventParaService = new TbEventParaService();

        [HttpGet]
        public ActionResult GetIndexList(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tbFieldService.GetFieldistByIdStr(string.IsNullOrEmpty(tbid) ? "" : tbid) };
            return Json(result);
        }

        public ActionResult SelectIndex(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }

        [HttpGet]
        public ActionResult DeleteParaForTemp(string id)
        {
            string err = tempService.DelParaFrmTemp(id);

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

        public ActionResult AddParaForTemp(string id, string type, string name, string value)
        {
            string err = "";
            try
            {
                err = tempService.AddParaTemp(id, type, name, value);
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

        public ActionResult AddPara(string tbid, string id, string type, string name, string value)
        {
            string err = "";
            try
            {
                err = tbEventParaService.AddPara(tbid, id, type, name, value);
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
        public ActionResult GetParaListFrmTemp(string id)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tempService.GetListParaTemp(id) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetParaList(string id, string frm)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tbEventParaService.GetListPara(id, frm) };
            return Json(result);
        }

        public ActionResult GetTableFieldsList(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = tbFieldService.GetTableColumList(tbid) };

            return Json(result);
        }

        [HttpGet]
        public ActionResult TempletInsert(string tbid)
        {
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            //可选择的数据源绑定
            DataTable dt = tbServer.GetMainAndGridTb();
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            ViewBag.FieldList = tbFieldService.GetFieldistByIdStr2(string.IsNullOrEmpty(tbid) ? "" : tbid);

            return View();
        }

        [HttpGet]
        public ActionResult TempletUpdate(string tbid)
        {
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            //可选择的数据源绑定
            DataTable dt = tbServer.GetMainAndGridTb();
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            ViewBag.FieldList = tbFieldService.GetFieldistByIdStr2(string.IsNullOrEmpty(tbid) ? "" : tbid);

            return View();
        }

        [HttpGet]
        public ActionResult TempletDelete(string tbid)
        {
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            //可选择的数据源绑定
            DataTable dt = tbServer.GetMainAndGridTb();
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            ViewBag.FieldList = tbFieldService.GetFieldistByIdStr2(string.IsNullOrEmpty(tbid) ? "" : tbid);

            return View();
        }
    }
}