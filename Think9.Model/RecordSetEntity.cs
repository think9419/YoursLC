using DapperExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Think9.Models
{
    [Table("recordset")]
    public class RecordSetEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string ObjectId { get; set; }

        public string OperatePerson { get; set; }

        public string Info { get; set; }//

        public string IP { get; set; }//

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [Display(Name = "修改时间")]
        public DateTime OperateTime { get; set; }
    }
}