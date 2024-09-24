using Microsoft.AspNetCore.Mvc;
using Think9.Services.Base;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class AutoCodeController : BaseController
    {
        [HttpPost]
        public JsonResult GetCode(string name, string type)
        {
            string appId = Think9.Services.Base.Configs.GetValue("BaiduTransAppId");//百度翻译appId
            string passWord = Think9.Services.Base.Configs.GetValue("BaiduTransPassWord");//百度翻译appId

            string code = Think9.Services.Basic.CreatCode.Creat(appId, passWord, type, name);

            var result = SuccessTip("操作成功", code);

            return Json(result);
        }

        [HttpPost]
        public JsonResult NewGuid()
        {
            var result = SuccessTip("操作成功", Think9.Services.Basic.CreatCode.NewGuid());

            return Json(result);
        }
    }
}