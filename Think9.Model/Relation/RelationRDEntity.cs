using DapperExtensions;

namespace Think9.Models
{
    [Table("relationrd")]
    public class RelationRDEntity
    {
        public int RelationId { get; set; }

        /// <summary>
        /// 回写类别：1增加2删除3修改 读取类别：1读取单值2读取列表值9子表写入
        /// </summary>
        public int RelationSort { get; set; }

        /// <summary>
        /// 选择报表对应的流程编码
        /// </summary>
        public string FromFlowId { get; set; }

        /// <summary>
        /// 应用的流程步骤？all所有流程步骤，流程步骤之间以；分割
        /// </summary>
        public string FlowPrcs { get; set; }

        /// <summary>
        /// 读取数据的报表编码 可能会是包含.的子表
        /// </summary>
        public string FromTbId { get; set; }

        /// <summary>
        /// 填充数据报表id 可能会是包含.的子表
        /// </summary>
        public string FillTbId { get; set; }

        /// <summary>
        /// --sql中select部分
        /// </summary>
        public string SelectField { get; set; }

        /// <summary>
        /// 选择字符串-sql中select和from部分
        /// </summary>
        public string SelectExpress { get; set; }

        /// <summary>
        /// sql语句中Where部分
        /// </summary>
        public string WhereStr1 { get; set; }

        /// <summary>
        /// sql语句中Where部分
        /// </summary>
        public string WhereStr2 { get; set; }

        public string OrderIndexId { get; set; }

        /// <summary>
        /// 排序方式？1正序2反序
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        ///  sql语句中Order部分
        /// </summary>
        public string OrderStr { get; set; }

        /// <summary>
        /// 完整的sql语句
        /// </summary>
        public string SqlStr { get; set; }

        /// <summary>
        /// 是否累加？2累加1不累加
        /// </summary>
        public string UseType { get; set; }

        /// <summary>
        /// 标志字符串--第一位2表示读数据集1表示读单数
        /// </summary>
        public string RelationRDFlag { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string RelationRDBy { get; set; }

        /// <summary>
        /// 11表示数据读取 21表示子表数据初始化 31表示数据回写
        /// </summary>
        [Computed]
        public string RelationType { get; set; }

        [Computed]
        public string RelationName { get; set; }

        [Computed]
        public string TbID { get; set; }

        [Computed]
        public string FillIndexId { get; set; }

        [Computed]
        public string SelectIndexId { get; set; }

        [Computed]
        public string isValue { get; set; }

        [Computed]
        public string ICount { get; set; }

        [Computed]
        public string FromTbName { get; set; }//

        [Computed]
        public string RelationBy { get; set; }
    }
}