using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class NotifyService : BaseService<NotifyEntity>
    {
        //private ComService ComService = new ComService();
        //public bool ReadSms(int id,string userid)
        //{
        //    NotifyEntity model = new NotifyEntity();
        //    model.NotifyId = id;
        //    model.Readers = "1";

        //    ComService.ExecuteSql("update notify set Readers= Readers + '" + userid + "'   WHERE NotifyId= " + id);

        //    return base.UpdateById(model, "isRead");
        //}
    }
}