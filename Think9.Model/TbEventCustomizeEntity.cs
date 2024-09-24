using DapperExtensions;

namespace Think9.Models
{
    [Table("tbeventcustomize")]
    public class TbEventCustomizeEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string TbId { get; set; }
        public string EventId { get; set; }

        //执行类别 1sql 2存储过程
        public string ExecuteType { get; set; }

        public string ExecuteSql { get; set; }
        public string ExecuteStept { get; set; }
        public int OrderNo { get; set; }

        public string FullName { get; set; }
        public string Remarks { get; set; }

        [Computed]
        public string ProcedureName { get; set; }

        [Computed]
        public string Guid { get; set; }
    }
}