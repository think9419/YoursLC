using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("sys_taskschedule")]
    public class TaskScheduleEntity
    {
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string FileName { get; set; }
        public string JobGroup { get; set; }
        public string JobName { get; set; }
        public string CronExpress { get; set; }
        public DateTime StarRunTime { get; set; }
        public DateTime? EndRunTime { get; set; }
        public DateTime? createTime { get; set; }
        public int State { get; set; }
    }
}