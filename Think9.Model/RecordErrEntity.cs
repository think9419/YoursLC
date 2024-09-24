using DapperExtensions;

namespace Think9.Models
{
    [Table("recorderr")]
    public class RecordErrEntity
    {
        public int Id { get; set; }
        public string ListId { get; set; }
        public string TbId { get; set; }
        public string OperateTime { get; set; }
        public string Info { get; set; }
    }
}