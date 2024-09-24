using DapperExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Think9.Models
{
    [Table("indexbasic")]
    public class IndexBasicEntity
    {
        /// <summary>
        /// 指标分类
        /// </summary>
        /// <returns></returns>
        public string IndexSort { get; set; }

        /// <summary>
        /// 指标编码
        /// </summary>
        /// <returns></returns>
        public string IndexId { get; set; }

        /// <summary>
        /// 指标名称
        /// </summary>
        /// <returns></returns>
        public string IndexName { get; set; }

        /// <summary>
        /// 指标类型---来源表sys_DataType 1000:日期 2000:文字(不限定长度) 2030:文字(30字符以内) 2100:文字(100字符以内) 2500:文字(500字符以内) 3000:整数 3002:2位小数 3004:4位小数 3006:6位小数 3102:金额 3202:金额 5200:图片
        /// </summary>
        /// <returns></returns>
        public string IndexDataType { get; set; }

        [Computed]
        public string DataTypeName { get; set; }

        /// <summary>
        ///长度--输入0表示不限定长度
        /// </summary>
        /// <returns></returns>
        public int? Mlen { get; set; }

        /// <summary>
        ///小数位数
        /// </summary>
        /// <returns></returns>
        public int? Digit { get; set; }

        /// <summary>
        /// 标志位
        /// </summary>
        /// <returns></returns>
        public string IndexFlag { get; set; }

        /// <summary>
        /// 是否系统定义？1是2不是
        /// </summary>
        /// <returns></returns>
        public string IndexDefineType { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        /// <returns></returns>
        public string IndexExplain { get; set; }

        /// <summary>
        ///否可编辑？1可以2不可以
        /// </summary>
        /// <returns></returns>
        public string isEdit { get; set; }

        /// <summary>
        ///是否使用？1启用2禁用 UpdateTime
        /// </summary>
        /// <returns></returns>
        public string isUse { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [Display(Name = "修改时间")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 已使用该指标的录入表
        /// </summary>
        [Computed]
        public string TableName { get; set; }
    }
}