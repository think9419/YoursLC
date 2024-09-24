using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("sms")]
    public class SmsEntity
    {
        /// <summary>
        /// 短消息编号
        /// </summary>
        [DapperExtensions.Key(true)]
        public int SmsId { get; set; }

        /// <summary>
        /// 发送userid
        /// </summary>
        public string FromId { get; set; }

        /// <summary>
        /// 接收userid--只能一个
        /// </summary>
        public string ToId { get; set; }

        /// <summary>
        /// 类型？0表示个人短信 1手机短信 9表示邮件发送信息 11外出申请12请假申请13出差申请20工作汇报21点评31日程安排99工作提醒
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发送时间--可以是将来时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 短消息标志位
        /// </summary>
        public string SmsFlag { get; set; }

        /// <summary>
        /// 链接url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否读过？1是2否
        /// </summary>
        public string isRead { get; set; }

        /// <summary>
        /// 是否删除？1是2否
        /// </summary>
        public string isDel { get; set; }

        /// <summary>
        /// 关联表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 关联id
        /// </summary>
        public int RelateId { get; set; }

        /// <summary>
        /// 备用
        /// </summary>
        public string SmsBy1 { get; set; }

        [Computed]
        public int MsgCout { get; set; }

        [Computed]
        public string Attachment { get; set; }

        [Computed]
        public string Subject { get; set; }
    }
}