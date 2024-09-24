using DapperExtensions;

namespace Think9.Models
{
    [Table("recordrun")]
    public class RecordRunEntity
    {
        public int Id { get; set; }
        public string ListId { get; set; }
        public string ruNumber { get; set; }
        public string FlowId { get; set; }
        public string TbId { get; set; }
        public string OperateTime { get; set; }
        public string OperatePerson { get; set; }
        public string OperateType { get; set; }
        public string Info { get; set; }
        public string IP { get; set; }
        public string RecordFlag { get; set; }
    }
}