using DapperExtensions;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportparm")]
    public class ReportParmEntity
    {
        /// <summary>
        /// 主键 自增长
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string ReportId { get; set; }

        public int? RowId { get; set; }

        public int? ColNum { get; set; }

        public string ParmId { get; set; }

        public string ParmValue { get; set; }

        public string Description { get; set; }

        [Computed]
        public string ParmName { get; set; }

        [Computed]
        public string Postion { get; set; }

        [Computed]
        public string Type { get; set; }

        [Computed]
        public string StrID { get; set; }
    }
}