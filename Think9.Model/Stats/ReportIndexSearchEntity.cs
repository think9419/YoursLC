using DapperExtensions;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportindexsearch")]
    public class ReportIndexSearchEntity
    {
        /// <summary>
        /// 主键 自增长
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string ReportId { get; set; }

        public string IndexId { get; set; }

        public string Lable { get; set; }

        public int? OrderNo { get; set; }
    }
}