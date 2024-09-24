using DapperExtensions;
using System;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("rulelist")]
    public class RuleListEntity
    {
        public string RuleId { get; set; }

        /// <summary>
        /// //数据规范类型?1系统指标2自动编号6单列选择7多列选择8树形选择
        /// </summary>
        public string RuleType { get; set; }

        public string RuleFlag { get; set; }
        public string RuleName { get; set; }
        public string RuleBy { get; set; }
        public DateTime UpdateTime { get; set; }

        [Computed]
        public string Used { get; set; }
    }
}