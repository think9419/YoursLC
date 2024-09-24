using DapperExtensions;
using System;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("indexstats")]
    public class IndexStatsEntity
    {
        /// <summary>
        /// 外部数据库ID
        /// </summary>
        public int? DbID { get; set; }

        /// <summary>
        /// 是否复合指标？2表示是
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string IndexSort { get; set; }

        /// <summary>
        /// 指标编码
        /// </summary>
        public string IndexId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string DescName { get; set; }

        /// <summary>
        /// string int decimal datetime
        /// </summary>
        public string IndexType { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int? Digit { get; set; }

        /// <summary>
        /// 有用----流程编码
        /// </summary>
        public string FromFlowId { get; set; }

        /// <summary>
        /// 有用 SelectType
        /// </summary>
        public string SelectType { get; set; }

        /// <summary>
        ///  有用 SelectType
        /// </summary>
        public string SelectField { get; set; }

        /// <summary>
        /// 有用----显示时使用
        /// </summary>
        public string SelectExpress { get; set; }

        /// <summary>
        /// 有用----报表编码 可能会是包含.的子表
        /// </summary>
        public string FromTbId { get; set; }

        /// <summary>
        /// 有用  sql语句中From部分
        /// </summary>
        public string FromStr { get; set; }

        /// <summary>
        /// （有用）sql语句中From部分
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        ///有用---- sql语句中Where部分
        /// </summary>
        public string WhereStr { get; set; }

        /// <summary>
        /// 有用----type=1才有--- 计算时SqlStr+WhereStr1
        /// </summary>
        public string SqlStr { get; set; }

        /// <summary>
        /// 有用----计算公式1--type=2才有
        /// </summary>
        public string FormulaStr { get; set; }

        /// <summary>
        /// 标志位
        /// </summary>
        public string IndexFlag { get; set; }

        /// <summary>
        /// 解释
        /// </summary>
        public string IndexExplain { get; set; }

        /// <summary>
        /// 启用？1是2否
        /// </summary>
        public string IsUse { get; set; }

        /// <summary>
        /// 添加方式？0一般方式添加 1纯语句方式添加
        /// </summary>
        public int? AddBy { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime UpdateTime { get; set; }

        [Computed]
        public string Value { get; set; }

        [Computed]
        public string Text { get; set; }

        [Computed]
        public string ParmStr { get; set; }

        [Computed]
        public string ReportStr { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Computed]
        public string FromTbId_Exa { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Computed]
        public string DbID_Exa { get; set; }
    }
}