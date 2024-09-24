using System.Data;
using Think9.Models;

namespace Think9.Services.Base
{
    public class MainTBService : BaseService<MainTBEntity>
    {
        public static MainTBEntity GetMainTBEntity(string listid, string flowid)
        {
            ComService comService = new ComService();

            MainTBEntity model = new MainTBEntity();

            if (flowid.StartsWith("bi_"))
            {
                DataTable dt = comService.GetDataTable("select * from " + flowid.Replace("bi_", "tb_") + "    where listid=" + listid + " ");
                if (dt.Rows.Count > 0)
                {
                    model.isLock = dt.Rows[0]["isLock"].ToString();
                    model.createTime = dt.Rows[0]["CreateTime"].ToString();
                    model.createUser = dt.Rows[0]["CreateUser"].ToString();
                    model.createDept = dt.Rows[0]["CreateDept"].ToString();
                    model.createDeptStr = dt.Rows[0]["CreateDeptStr"].ToString();
                    model.runName = dt.Rows[0]["RunName"].ToString();
                    model.attachmentId = dt.Rows[0]["AttachmentId"].ToString();
                }
            }
            else
            {
                DataTable dt = comService.GetDataTable("select * from flowrunlist where listid=" + listid + " ");
                if (dt.Rows.Count > 0)
                {
                    model.isLock = dt.Rows[0]["isLock"].ToString();
                    model.createTime = dt.Rows[0]["CreateTime"].ToString();
                    model.createUser = dt.Rows[0]["CreateUser"].ToString();
                    model.createDept = dt.Rows[0]["CreateDept"].ToString();
                    model.createDeptStr = dt.Rows[0]["CreateDeptStr"].ToString();
                    model.runName = dt.Rows[0]["RunName"].ToString();
                    model.attachmentId = dt.Rows[0]["AttachmentId"].ToString();
                }
            }

            return model;
        }
    }
}