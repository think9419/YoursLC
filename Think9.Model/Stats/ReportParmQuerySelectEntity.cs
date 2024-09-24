using DapperExtensions;

namespace Think9.Models
{
    [Table("reportparmqueryselect")]
    public class ReportParmQuerySelectEntity
    {
        [DapperExtensions.Key(true)]
        public int SelectId { get; set; }

        public int ListId { get; set; }

        public string ReportId { get; set; }

        public int OrderNo { get; set; }

        public string ValueStr { get; set; }

        public string TextStr { get; set; }

        public string IsDefault { get; set; }

        public string ParmId { get; set; }
    }
}