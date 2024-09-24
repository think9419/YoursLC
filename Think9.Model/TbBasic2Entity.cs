using DapperExtensions;

namespace Think9.Models
{
    [Table("tbbasic2")]
    public class TbBasic2Entity
    {
        public string TbId { get; set; }
        public int ColumnsNumber { get; set; }

        /// <summary>
        /// 0无增加、删除按钮 1增加和删除按钮在右侧 2增加和删除按钮在顶部
        /// </summary>
        public string InType { get; set; }
    }
}