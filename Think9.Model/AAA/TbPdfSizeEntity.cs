using DapperExtensions;

namespace Think9.Models
{
    [Table("tbpdfsize")]
    public class TbPdfSizeEntity
    {
        public string TbId { get; set; }
        public string Type { get; set; }

        public decimal? Width { get; set; }
        public decimal? Heigh { get; set; }
        public decimal? Top { get; set; }
        public decimal? Left { get; set; }
        public decimal? Right { get; set; }
        public decimal? Bottom { get; set; }
    }
}