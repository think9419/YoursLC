using DapperExtensions;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportindexorder")]
    public class ReportIndexOrderEntity
    {
        /// <summary>
        /// 主键 自增长
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string ReportId { get; set; }

        public string IndexId { get; set; }

        //1正序2倒序
        public string OrderType { get; set; }

        public int? OrderNo { get; set; }
    }
}