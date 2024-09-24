using DapperExtensions;
using System;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("reportbasic")]
    public class ReportBasicEntity
    {
        [Computed]
        public string TempletCount { get; set; }

        [Computed]
        public string TempDirectoryCount { get; set; }

        [Computed]
        public string IsModuleExists { get; set; }

        [Computed]
        public string IsFileExists { get; set; }

        [Computed]
        public string CreateCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int DbID { get; set; }

        /// <summary>
        /// 报表分类
        /// </summary>
        public string ReportSort { get; set; }

        /// <summary>
        /// 类别 默认0，基于录入表创建为1
        /// </summary>
        public int ReporType { get; set; }

        /// <summary>
        /// 报表编码
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        public string ReportName { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int Digits { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 行高
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 查看权限-用户
        /// </summary>
        public string StrUser { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 关联的录入表id
        /// </summary>
        public string TbId { get; set; }

        /// <summary>
        /// 标志位
        /// </summary>
        public string ReportFlag { get; set; }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontStyle { get; set; }

        /// <summary>
        /// 启用？1是2否
        /// </summary>
        public string IsUse { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ReportRemarks { get; set; }

        /// <summary>
        /// 新建用户
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 新建时间 UpdateTime
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 最后修改
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        [Computed]
        public long ColsNum { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        [Computed]
        public long RowsNum { get; set; }

        /// <summary>
        /// 查询参数的数量
        /// </summary>
        [Computed]
        public long QueryParmNum { get; set; }

        /// <summary>
        /// 查询参数的数量
        /// </summary>
        [Computed]
        public long OrderParmNum { get; set; }

        /// <summary>
        /// 动态行的数量
        /// </summary>
        [Computed]
        public long DYNParmNum { get; set; }

        /// <summary>
        /// 模式？0不确定1发布模式2调试模式
        /// </summary>
        [Computed]
        public string Model { get; set; }

        [Computed]
        public string FromTbId { get; set; }

        [Computed]
        public string DbID_Exa { get; set; }
    }
}