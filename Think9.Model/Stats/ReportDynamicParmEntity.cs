using DapperExtensions;

namespace Think9.Models
{
    [Table("reportdynparm")]
    public class ReportDYNParmEntity
    {
        [DapperExtensions.Key(true)]
        public int DynamicId { get; set; }

        public string ReportId { get; set; }

        public string ParmId { get; set; }

        /// <summary>
        /// 手写sql
        /// </summary>
        public string SqlStr { get; set; }

        /// <summary>
        /// -1表示自己定义
        /// </summary>
        public string RuleId { get; set; }

        public string ItemStr { get; set; }

        [Computed]
        public string Postion { get; set; }

        [Computed]
        public string Type { get; set; }

        [Computed]
        public string ParameteName { get; set; }

        [Computed]
        public string RuleName { get; set; }
    }
}