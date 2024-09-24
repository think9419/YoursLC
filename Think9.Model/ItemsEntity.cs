using DapperExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Think9.Models
{
    [Table("sys_Items")]
    public class ItemsEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string ItemSort { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string EnCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; }//

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [Display(Name = "修改时间")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 子项数量
        /// </summary>
        [Computed]
        public string Amount { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Computed]
        public string Used { get; set; }
    }
}