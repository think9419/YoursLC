using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    public partial class TbBasicController : BaseController
    {
        public JsonResult DelTbButDisable(string tbid, string idsStr)
        {
            TbButDisableService tbButDisable = new TbButDisableService();
            tbButDisable.DelByIdsStr(idsStr);

            return Json(SuccessTip("删除成功，属性已更改无需『重新生成』"));
        }

        public ActionResult GetTbButDisableList(string tbid, string btnid, string pageType)
        {
            TbButDisableService tbButDisable = new TbButDisableService();
            var result = new { code = 0, count = 0, data = tbButDisable.GetList(tbid, btnid, pageType) };
            return Json(result);
        }

        public ActionResult AddTbButDisable(string tbid, string btnid, string objType, string pageType, string idsStr)
        {
            TbButDisableService tbButDisable = new TbButDisableService();
            tbButDisable.Add(tbid, btnid, objType, pageType, idsStr);

            return Json(SuccessTip("编辑成功，属性已更改无需『重新生成』"));
        }

        public ActionResult TbHiddenIndex(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        public JsonResult DelTbHiddenIndex(string tbid, string idsStr)
        {
            string id = "";
            TbHiddenIndexService tbHiddenIndexService = new TbHiddenIndexService();
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    id = arr[i].ToString().Trim();
                    string where = "where Id=" + id + "";

                    tbHiddenIndexService.DeleteByWhere(where);
                }
            }
            return Json(SuccessTip("删除成功"));
        }

        [HttpPost]
        public ActionResult AddTbHiddenIndex(string tbid, string objType, string objId, string indexId, string isHidden)
        {
            try
            {
                TbHiddenIndexService tbHiddenIndexService = new TbHiddenIndexService();
                if (tbHiddenIndexService.Insert(new TbHiddenIndexEntity { TbId = tbid, isHidden = isHidden, ObjType = objType, ObjId = ";" + objId + ";", IndexId = ";" + indexId + ";" }))
                {
                    return Json(SuccessTip("添加成功"));
                }
                else
                {
                    return Json(ErrorTip("添加失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult GetTbHiddenIndexList(PageInfoEntity pageInfo, string tbid, string keyword)
        {
            tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;

            TbHiddenIndexService tbHiddenIndexService = new TbHiddenIndexService();
            long total = 0;
            string where = "WHERE TbId='" + tbid + "'";
            var list = tbHiddenIndexService.GetByWhere(where).ToList();
            foreach (var item in list)
            {
                if (item.isHidden == "y")
                {
                    item.isHidden = "只读&保密";
                }
                else
                {
                    item.isHidden = "只读";
                }
            }
            var result = new { code = 0, count = 999999, data = list };
            return Json(result);
        }

        public ActionResult GetObjListForPage(PageInfoEntity pageInfo, string type, string keyword)
        {
            type = string.IsNullOrEmpty(type) ? "1" : type;
            TbHiddenIndexService tbHiddenIndexService = new TbHiddenIndexService();
            long total = 0;
            IEnumerable<dynamic> _list = tbHiddenIndexService.GetObjList(ref total, pageInfo, type, keyword);
            return Json(new { code = 0, msg = "", count = total, data = _list });
        }

        public ActionResult GetIndexListForPage(PageInfoEntity pageInfo, string tbid, string keyword)
        {
            tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;
            TbHiddenIndexService tbHiddenIndexService = new TbHiddenIndexService();
            long total = 0;
            IEnumerable<dynamic> _list = tbHiddenIndexService.GetIndexList(ref total, pageInfo, tbid, keyword);
            return Json(new { code = 0, msg = "", count = total, data = _list });
        }
    }
}