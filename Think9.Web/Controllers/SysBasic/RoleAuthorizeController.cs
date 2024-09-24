using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class RoleAuthorizeController : BaseController
    {
        private RoleAuthorizeService roleAuthorizeService = new RoleAuthorizeService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpPost]
        public ActionResult InsertBatch(IEnumerable<RoleAuthorizeEntity> list, int roleId)
        {
            var result = roleAuthorizeService.SavePremission(list, roleId) > 0 ? SuccessTip("保存成功") : ErrorTip("操作失败");
            return Json(result);
        }
    }
}