using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    public class MessageController : BaseController
    {
        private NotifyService notifyService = new NotifyService();
        private ComService comService = new ComService();

        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSmsCount()
        {
            string userId = CurrentUser == null ? "!NullEx" : CurrentUser.Account;

            SmsEntity model = new SmsEntity();
            model.MsgCout = 0;
            string strcount1 = comService.GetSingleField("SELECT COUNT(1) FROM sms where ToId = '" + userId + "' and isRead = '2'");
            if (!string.IsNullOrEmpty(strcount1))
            {
                model.MsgCout += int.Parse(strcount1);
            }
            string strcount2 = comService.GetSingleField("SELECT COUNT(1) FROM  notify");
            if (!string.IsNullOrEmpty(strcount2))
            {
                model.MsgCout += int.Parse(strcount2);
            }
            return Json(model);
        }

        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUnReadListJson()
        {
            DataTable dt = DataTableHelp.NewSmsDt();
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            userid = ";" + userid + ";";

            NotifyEntity model = new NotifyEntity();
            model.Readers = string.Format("%{0}%", userid);

            IEnumerable<dynamic> list = notifyService.GetByWhere("where Readers not like @Readers", model, null, "order by publishTime desc ");

            string content = "";
            string attachment = "";
            string sql = "select * from notify";
            foreach (NotifyEntity obj in list)
            {
                DataRow row = dt.NewRow();
                row["SmsId"] = obj.NotifyId.ToString();
                row["Type"] = 0;
                row["FromId"] = obj.FromId;
                row["Subject"] = obj.Subject;
                //row["Content"] = obj.Content + "<br>" + "fujian";
                row["createTime"] = obj.publishTime.ToString();
                row["Subject"] = obj.Subject;
                content = obj.Content;
                attachment = "";
                if (!string.IsNullOrEmpty(obj.attachmentId))
                {
                    foreach (DataRow dr in comService.GetDataTable("select * from flowattachment where AttachmentId='" + obj.attachmentId + "'").Rows)
                    {
                        attachment += "<a style='color: #FE7300;text-decoration: underline;' href='/UserFile/" + dr["AttachmentId"].ToString() + "/" + dr["FullName"].ToString() + "' target='_blank'>附件：" + dr["FullName"].ToString() + "</a>";
                    }
                }

                row["Content"] = content;
                row["Attachment"] = attachment;
                dt.Rows.Add(row);
            }

            string userId = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            sql = "select * from sms where ToId = '" + userId + "' and isRead = '2' order by SmsId desc";
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                DataRow row = dt.NewRow();
                row["SmsId"] = dr["SmsId"].ToString();
                row["Type"] = 99;
                row["FromId"] = dr["FromId"].ToString();
                row["Subject"] = "";
                row["Content"] = dr["Content"].ToString();
                row["createTime"] = dr["createTime"].ToString();
                dt.Rows.Add(row);
            }

            return Json(DataTableHelp.ToEnumerable<SmsEntity>(dt));
        }
    }
}