using DapperExtensions;

namespace Think9.Models
{
    [Table("sys_datatype")]
    public class IndexDtaeTypeEntity
    {
        /// <summary>
        /// 类别编码 CAT_index：录入指标分类
        /// </summary>
        /// <returns></returns>
        public string TypeId { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        /// <returns></returns>
        public string TypeName { get; set; }

        /// <summary>
        /// 变量类型--1日期型 2字符型 3数值型 4附件 5图片
        /// </summary>
        /// <returns></returns>
        public string DataType { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public int Mlen { get; set; }

        /// <summary>
        ///小数位数
        /// </summary>
        /// <returns></returns>
        public int Digit { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        /// <returns></returns>
        public int isSys { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        /// <returns></returns>
        public int TypeOrder { get; set; }
    }
}