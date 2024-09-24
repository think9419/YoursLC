using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbEventController : BaseController
    {
        private TbEventService tbEvent = new TbEventService();
        private ComService comService = new ComService();

        [HttpGet]
        public ActionResult List(string tbid)
        {
            TbBasicService tbBasicService = new TbBasicService();
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        [HttpGet]
        public JsonResult GetRecordList(string id, string isSys, PageInfoEntity pageInfo)
        {
            isSys = string.IsNullOrEmpty(isSys) ? "y" : isSys;
            long total = 0;
            string where = " where EventId = " + id + " and isSys ='" + isSys + "'";
            pageInfo.field = "WriteTime";
            pageInfo.order = "desc";

            SqlAndParamService service = new SqlAndParamService();
            var list = service.GetPageByFilter(ref total, null, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetList(PageInfoEntity pageInfo, string tbid)
        {
            string where = "where TbId='" + tbid + "' ";
            var result = new { code = 0, msg = "", count = 999999, data = tbEvent.GetByWhere(where) };
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add(string tbid)
        {
            ViewBag.Guid = System.Guid.NewGuid().ToString("N");
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            ViewBag.SelectList = tbEvent.GetTbEventCustomizeList(tbid);

            return View();
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = tbEvent.GetByWhereFirst("where Id=@Id", new { Id = id });
            if (model != null)
            {
                model.ProcedureName = model.ExecuteSql;
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(string id, TbEventCustomizeEntity model)
        {
            if (model.ExecuteType == "2")
            {
                model.ExecuteSql = model.ProcedureName;
                if (string.IsNullOrEmpty(model.ExecuteSql))
                {
                    return Json(ErrorTip("请输入存储过程名称"));
                }
            }
            model.ExecuteSql = model.ExecuteSql == null ? "" : BaseUtil.ReplaceHtml(model.ExecuteSql);

            string where = "where Id=" + id + "";
            string updateFields = "ExecuteSql,OrderNo,FullName,Remarks";

            var result = tbEvent.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功，修改后需『重新生成』才能生效") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult Add(string tbid, string guid, TbEventCustomizeEntity model)
        {
            model.ExecuteSql = model.ExecuteSql == null ? "" : BaseUtil.ReplaceHtml(model.ExecuteSql);
            model.TbId = tbid;
            try
            {
                string err = tbEvent.Add(model, guid);
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
        public ActionResult Delete(string id)
        {
            string err = tbEvent.Delete(id);

            if (err == "")
            {
                return Json(SuccessTip("删除成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }
    }
}