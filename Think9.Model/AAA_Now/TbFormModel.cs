/*******************************************************************************
 * Create:admin 2024-05-21 11:55:00
 * Description: YoursLc有源低代码 http://yourslc.top Model类
*********************************************************************************/

using DapperExtensions;
using System;

namespace Think9.Models
{
    /// <summary>
    /// 实体类【主表】  生成时间：2024-05-21 11:55:00
    /// </summary>
    [Table("tbform")]
    public class TbFormModel
    {
        #region Model

        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public long Id { get; set; }

        public string TbId { get; set; }

        public string IndexId { get; set; }
        public string IndexName { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public int? NLabel { get; set; }

        /// 行
        /// </summary>
        public int? NLine { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public int? NColumn { get; set; }

        public DateTime AddTime { get; set; }

        public string Exa { get; set; }

        #endregion Model
    }
}