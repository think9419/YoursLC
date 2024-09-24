using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class TaskScheduleService : BaseService<TaskScheduleEntity>
    {
        //public dynamic GetListByFilter(TaskScheduleEntity filter, PageInfoEntity pageInfo)
        //{
        //    pageInfo.prefix = "a.";
        //    string _where = " sys_taskschedule a where a.State in (0,1)";
        //    pageInfo.returnFields = "*";
        //    return GetPageUnite(filter, pageInfo, _where);
        //}

        //public bool ResumeScheduleJob(TaskScheduleEntity sm)
        //{
        //    return UpdateById(sm);
        //}

        //public bool StopScheduleJob(TaskScheduleEntity sm)
        //{
        //    return UpdateById(sm);
        //}
    }
}