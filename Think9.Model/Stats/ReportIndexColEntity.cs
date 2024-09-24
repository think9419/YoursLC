using DapperExtensions;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportindexcol")]
    public class ReportIndexColEntity
    {
        /// <summary>
        /// 主键 自增长
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string ReportId { get; set; }

        public string TbId { get; set; }

        public string IndexId { get; set; }

        public string ColName { get; set; }

        public string DataType { get; set; }

        //统计求和模式0不统计1求和2平均3最大4最小
        public string IsSum { get; set; }

        public int? ColWidth { get; set; }

        public int? OrderNo { get; set; }

        [Computed]
        public string Parm { get; set; }
    }
}