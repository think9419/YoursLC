using DapperExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Think9.Models
{
    [Table("Notify")]
    public class NotifyEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int NotifyId { get; set; }

        public string attachmentId { get; set; }
        public DateTime BeginDate { get; set; }
        public string Content { get; set; }
        public DateTime? EndDate { get; set; }
        public string FromId { get; set; }
        public int Important { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime publishTime { get; set; }

        public string Readers { get; set; }
        public string Subject { get; set; }
        public string ToDep { get; set; }
        public string ToPriv { get; set; }
        public string ToUser { get; set; }
        public int Type { get; set; }
    }
}