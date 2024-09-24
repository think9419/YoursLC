using System;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class SmsService : BaseService<SmsEntity>
    {
        private ComService ComService = new ComService();

        public int AddSms(string fromid, string toid, string content, int type)
        {
            int icount = 0;
            SmsEntity model = new SmsEntity();
            model.FromId = fromid;
            model.Type = type;
            model.Content = content;
            model.createTime = DateTime.Now;
            model.isRead = "2";
            model.isDel = "2";

            string[] arr = BaseUtil.GetStrArray(toid, ";");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    model.ToId = arr[i].ToString();
                    if (base.Insert(model))
                    {
                        icount++;
                    }
                }
            }

            return icount;
        }

        public int SendSmsToCreateUser(string fromid, string listid, string selectUser, int type)
        {
            int icount = 0;
            SmsEntity model = new SmsEntity();
            model.FromId = fromid;
            model.Type = type;
            model.createTime = DateTime.Now;
            model.isRead = "2";
            model.isDel = "2";

            DataTable dt = ComService.GetDataTable("select * from flowrunlist where listid=" + listid + "");
            if (dt.Rows.Count > 0)
            {
                model.ToId = dt.Rows[0]["createUser"].ToString();
                model.Content = "您发起的编号为 - " + dt.Rows[0]["ruNumber"].ToString() + " 名称为 - " + dt.Rows[0]["runName"].ToString() + "工作已由" + fromid + " 转交" + selectUser;
                if (base.Insert(model))
                {
                    icount++;
                }
            }

            return icount;
        }

        public bool ReadSms(int id)
        {
            SmsEntity model = new SmsEntity();
            model.SmsId = id;
            model.isRead = "1";

            return base.UpdateById(model, "isRead");
        }
    }
}