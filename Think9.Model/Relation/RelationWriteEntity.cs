using DapperExtensions;

namespace Think9.Models
{
    [Table("relationwd")]
    public class RelationWriteEntity
    {
        public int RelationId { get; set; }

        // 回写类别1增加2删除3修改
        public int RelationSort { get; set; }

        // 源表流程编码
        public string SourceFlowId { get; set; }

        // 源表编码
        public string SourceTbId { get; set; }

        // 适用流程
        [Computed]
        public string FlowPrcs_Exa { get; set; }

        // 适用流程
        public string FlowPrcs { get; set; }

        // 回写数据表编码
        public string WriteTbId { get; set; }

        // 条件表达式
        public string WhereStr { get; set; }

        // 回写次数1表示只回写一次，其余均回写
        public string NumberType { get; set; }

        // 说明
        public string RelationWDBy { get; set; }

        // 标志字符串
        public string RelationWDFlag { get; set; }

        // 排序
        public int? OrderNo { get; set; }

        /// <summary>
        /// 11表示数据读取 21表示子表数据初始化 31表示数据回写
        /// </summary>
        [Computed]
        public string RelationType { get; set; }

        [Computed]
        public string RelationName { get; set; }

        [Computed]
        public string RelationBy { get; set; }

        [Computed]
        public string TbID { get; set; }

        [Computed]
        public string WriteField { get; set; }

        [Computed]
        public string Expression { get; set; }

        [Computed]
        public string isValue { get; set; }

        [Computed]
        public string ICount { get; set; }

        [Computed]
        public string FromTbName { get; set; }

        [Computed]
        public string tipsWhereStr { get; set; }
    }
}