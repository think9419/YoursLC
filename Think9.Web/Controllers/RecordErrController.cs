using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("Record")]
    public class RecordErrController : BaseController
    {
        private RecordErrService recordErrService = new RecordErrService();
        private ComService comService = new ComService();

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpPost]
        public JsonResult GetList(PageInfoEntity pageInfo, string key, string tbId, string listId)
        {
            pageInfo.field = "OperateTime";
            pageInfo.order = "desc";

            RecordErrEntity model = new RecordErrEntity();
            long total = 0;
            string where = "where 1=1 ";
            if (!string.IsNullOrEmpty(tbId))
            {
                where += " and tbId=@TbId";
                model.TbId = tbId;
            }
            if (!string.IsNullOrEmpty(listId))
            {
                where += " and listId=@ListId";
                model.ListId = listId;
            }
            if (!string.IsNullOrEmpty(key))
            {
                where += " and Info like @Info";
                model.Info = string.Format("%{0}%", key);
            }

            IEnumerable<dynamic> list = recordErrService.GetPageByFilter(ref total, model, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpPost]
        public JsonResult BatchDel(string idsStr)
        {
            string err = "";
            int numSuccess = 0;
            int numFail = 0;
            string id = "";
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    id = arr[i].ToString().Trim();
                    if (string.IsNullOrEmpty(err))
                    {
                        if (comService.ExecuteSql("delete from recorderr where Id = " + id + "") > 0)
                        {
                            numSuccess++;
                        }
                        else
                        {
                            numFail++;
                        }
                    }
                    else
                    {
                        numFail++;
                    }
                }
            }

            string str = numFail == 0 ? "删除成功" + numSuccess.ToString() + "数据" : "删除成功" + numSuccess + "数据;" + "删除失败" + numFail.ToString() + "数据;";
            return Json(ErrorTip(str));
        }
    }
}