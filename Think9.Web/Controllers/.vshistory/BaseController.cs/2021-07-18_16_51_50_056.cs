using Microsoft.AspNetCore.Mvc;
using Think9.Common;
using Think9.Model;
using Think9.Service.SystemOrganize;

namespace Think9.Web.Controllers
{
    [HandlerLogin]
    public class BaseController : Controller
    {
        //zzz
        public ButtonService ButtonService = new ButtonService();

        protected const string SuccessText = "操作成功！";
        protected const string ErrorText = "操作失败！";

        public OperatorModel Operator
        {
            get { return new OperatorProvider(HttpContext).GetCurrent(); }
        }

        // GET: Base
        public virtual ActionResult Index(int? id)
        {
            var _menuId = id == null ? 0 : id.Value;
            var _roleId = Operator.RoleId;
            if (id != null)
            {
                ViewData["RightButtonList"] = ButtonService.GetButtonListByRoleIdModuleId(_roleId, _menuId, PositionEnum.FormInside);
                ViewData["TopButtonList"] = ButtonService.GetButtonListByRoleIdModuleId(_roleId, _menuId, PositionEnum.FormRightTop);
            }
            return View();
        }

        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult SuccessTip(string message = SuccessText)
        {
            return new AjaxResult { state = ResultType.success.ToString(), message = message };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual AjaxResult ErrorTip(string message = ErrorText)
        {
            return new AjaxResult { state = ResultType.error.ToString(), message = message };
        }
    }
}