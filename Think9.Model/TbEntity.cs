using DapperExtensions;

namespace Think9.Models
{
    [Table("TbModel")]
    public class TbEntity
    {
        public string TbId { get; set; }

        public string SearchMode { get; set; }
        public string ColumnsMode { get; set; }
        public string CalculatMode { get; set; }
        public string BalanceMode { get; set; }
        public string SumMode { get; set; }
        public string MainRelation { get; set; }
        public string XMLTypeStr { get; set; }
        public string ShowIndexId { get; set; }
        public string ReportUser { get; set; }
    }
}