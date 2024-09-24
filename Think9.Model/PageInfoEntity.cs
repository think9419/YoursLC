namespace Think9.Models
{
    public class PageInfoEntity
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 每页数据量
        /// </summary>
        public int limit { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string order { get; set; }

        /// <summary>
        /// 返回字段逗号分隔
        /// </summary>
        public string returnFields { get; set; }

        /// <summary>
        /// 前缀 多表联合时使用
        /// </summary>
        public string prefix { get; set; }
    }
}