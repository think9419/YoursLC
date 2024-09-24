using DapperExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Think9.Models
{
    [Table("sys_organize")]
    public class OrganizeEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string EnCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; }

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
        /// 机构分类
        /// </summary>
        [Computed]
        public string CategoryName { get; set; }
    }
}