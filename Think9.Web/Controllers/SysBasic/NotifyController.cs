using Microsoft.AspNetCore.Mvc;
using System;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class NotifyController : BaseController
    {
        private NotifyService notifyService = new NotifyService();
        private ComService comService = new ComService();

        private CurrentUserEntity GetUser()
        {
            return GetCurrentUser();
        }

        // GET: Permissions/Role
        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public JsonResult GetList(NotifyEntity model, PageInfoEntity pageInfo, string key)
        {
            pageInfo.field = "publishTime desc";
            var result = notifyService.GetPageByFilter(model, pageInfo, "where 1=1 ");
            return Json(result);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.UserId = GetUser() == null ? "!NullEx" : GetUser().Account;
            return View();
        }

        [HttpPost]
        public ActionResult Add(NotifyEntity model, string attid)
        {
            try
            {
                model.FromId = CurrentUser == null ? "" : CurrentUser.Account;
                model.BeginDate = DateTime.Now;
                model.EndDate = null;
                model.Important = 0;
                model.publishTime = DateTime.Now;
                model.ToUser = "#all#";
                model.Type = 0;
                model.Readers = "";
                model.attachmentId = attid;
                model.Content = model.Content.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("\r\n", "<br>");

                var result = notifyService.Insert(model) ? SuccessTip("操作成功") : ErrorTip("操作失败");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            string where = "where NotifyId=@NotifyId";
            object param = new { NotifyId = id };
            var model = notifyService.GetByWhereFirst(where, param);
            if (model != null)
            {
                model.Content = model.Content.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("\r\n", "<br>");
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(NotifyEntity model, string attid)
        {
            model.attachmentId = attid;
            string where = "where NotifyId=@NotifyId";
            string updateFields = "Content,Subject,attachmentId";
            model.Content = model.Content.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("\r\n", "<br>");

            var result = notifyService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");

            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            string where = "where NotifyId=" + id + " ";
            var result = notifyService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("操作失败");
            return Json(result);
        }
    }
}