using DapperExtensions;

namespace Think9.Models
{
    [Table("ruleauto")]
    public class RuleAutoEntity
    {
        [DapperExtensions.Key(true)]
        public int AutoOrder { get; set; }

        public string RuleId { get; set; }
        public string AutoType { get; set; }
        public string AutoSome1 { get; set; }
        public string AutoSome2 { get; set; }

        [Computed]
        public string AutoShow { get; set; }
    }
}