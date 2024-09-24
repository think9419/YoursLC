using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Think9.Services.Base;

namespace Think9.Controllers.Web
{
    public class HandlerLoginAttribute : Attribute, IActionFilter
    {
        public bool Ignore = true;

        public HandlerLoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (Ignore == false)
            {
                return;
            }
            if (new OperatorProvider(context.HttpContext).GetCurrent() == null)
            {
                context.HttpContext.Response.Redirect("/Login/LoginAgain");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //if (context.Result?.GetType() == typeof(JsonResult))
            //{
            //    JsonResult result = context.Result as JsonResult;
            //    JObject valueObject = (JObject)(result.Value);
            //    valueObject["xb"] = "xxxx";
            //    result.Value = valueObject;
            //    context.Result = result;
            //    string yy = "";
            //}
        }
    }
}