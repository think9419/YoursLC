using DapperExtensions;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportindexcolparm")]
    public class ReportIndexColParmEntity
    {
        /// <summary>
        /// 主键 自增长
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public int ColId { get; set; }

        public string ReportId { get; set; }

        public string ParmId { get; set; }

        public string ParmValue { get; set; }

        //0当前行字段 1固定值
        public string Type { get; set; }
    }
}