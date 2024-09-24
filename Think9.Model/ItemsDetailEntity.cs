using DapperExtensions;

namespace Think9.Models
{
    [Table("sys_itemsdetail")]
    public class ItemsDetailEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string DetailName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string DetailCode { get; set; }

        /// <summary>
        /// 字典分类
        /// </summary>
        public int OrderNo { get; set; }
    }
}