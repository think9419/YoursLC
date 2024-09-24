using DapperExtensions;

namespace Think9.Models
{
    [Table("recordcode")]
    public class RecordCodeEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public long Id { get; set; }

        public string ObjectId { get; set; }
        public string OperatePerson { get; set; }
        public string OperateTime { get; set; }
        public string FilePath { get; set; }
        public string Info { get; set; }
    }
}