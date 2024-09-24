using DapperExtensions;

namespace Think9.Models
{
    [Table("sys_sort")]
    public class SortEntity
    {
        /// <summary>
        /// 类别编码 A01：录入指标分类
        /// </summary>
        /// <returns></returns>
        public string ClassID { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        /// <returns></returns>
        public string SortID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        /// <returns></returns>
        public string SortName { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        /// <returns></returns>
        public int SortOrder { get; set; }
    }
}