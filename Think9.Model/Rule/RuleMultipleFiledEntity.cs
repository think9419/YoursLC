using DapperExtensions;

namespace Think9.Models
{
    [Table("rulemultiplefiled")]
    public class RuleMultipleFiledEntity
    {
        public string RuleId { get; set; }

        public string FiledValue { get; set; }

        public string FiledText { get; set; }

        public int FiledOrder { get; set; }
    }
}