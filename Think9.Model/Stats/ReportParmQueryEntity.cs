using DapperExtensions;

namespace Think9.Models
{
    [Table("reportparmquery")]
    public class ReportParmQueryEntity
    {
        [DapperExtensions.Key(true)]
        public int ListId { get; set; }

        public string ReportId { get; set; }

        public string ParmId { get; set; }

        public int OrderNo { get; set; }

        /// <summary>
        /// -1表示自己定义
        /// </summary>
        public string RuleId { get; set; }

        /// <summary>
        /// 1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框
        /// </summary>
        public string ControlType { get; set; }

        public string DefaultValue { get; set; }

        public string ControlLable { get; set; }

        public string ControlPlaceholder { get; set; }

        public int ControlHeight { get; set; }

        public int ControlWidth { get; set; }

        [Computed]
        public string Type { get; set; }

        [Computed]
        public string ParameteName { get; set; }

        [Computed]
        public string RuleName { get; set; }

        [Computed]
        public string ControlTypeName { get; set; }
    }
}