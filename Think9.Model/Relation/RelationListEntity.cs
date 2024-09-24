using DapperExtensions;

namespace Think9.Models
{
    [Table("relationlist")]
    public class RelationListEntity
    {
        [DapperExtensions.Key(true)]
        public int RelationId { get; set; }

        /// <summary>
        /// 11表示数据读取 21表示子表数据初始化 31表示数据回写
        /// </summary>
        public string RelationType { get; set; }

        public string RelationBy { get; set; }
        public string RelationName { get; set; }
        public string RelationFlag { get; set; }
        public string FlowStr { get; set; }
        public int ICount { get; set; }

        /// <summary>
        /// 0表示从系统数据库读取数据 非0表示从其他数据源读取
        /// </summary>
        public int DbID { get; set; }

        [Computed]
        public string DbID_Exa { get; set; }

        /// <summary>
        /// 启用禁用 1启用2禁用
        /// </summary>
        [Computed]
        public string isUse
        {
            get; set;
        }
    }
}