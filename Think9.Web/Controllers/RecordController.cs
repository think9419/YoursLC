using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("Record")]
    public class OperationRecordController : BaseController
    {
        private RecordService recordService = new RecordService();

        private CurrentUserEntity GetUser()
        {
            return GetCurrentUser();
        }

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpPost]
        public JsonResult GetList(PageInfoEntity pageInfo, string key, string userId, string tbId, string listId)
        {
            pageInfo.field = "OperateTime";
            pageInfo.order = "desc";

            RecordRunEntity model = new RecordRunEntity();
            long total = 0;

            string where = "where 1=1 ";
            if (!string.IsNullOrEmpty(userId))
            {
                where += " and OperatePerson=@OperatePerson";
                model.OperatePerson = userId;
            }
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

            IEnumerable<dynamic> list = recordService.GetPageByFilter(ref total, model, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }
    }
}