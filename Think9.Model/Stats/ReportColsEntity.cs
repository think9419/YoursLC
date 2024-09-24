using DapperExtensions;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportcols")]
    public class ReportColsEntity
    {
        public string ReportId { get; set; }

        public int ColNum { get; set; }

        public string ControlStr { get; set; }

        public decimal? ColWidth { get; set; }

        /// <summary>
        /// string int decimal date img
        /// </summary>
        public string ColType { get; set; }

        [Computed]
        public string ColType_Exa
        {
            get; set;
        }
    }
}