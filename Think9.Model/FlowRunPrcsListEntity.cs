using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("flowrunprcslist")]
    public class FlowRunPrcsListEntity
    {
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [DapperExtensions.Key(true)]
        public int id { get; set; }

        /// <summary>
        /// 对应flowrunlist中的listid
        /// </summary>
        /// <returns></returns>
        public long listid { get; set; }

        /// <summary>
        /// 流程步骤id
        /// </summary>
        /// <returns></returns>
        public int PrcsId { get; set; }

        /// <summary>
        ///流程编码
        /// </summary>
        /// <returns></returns>
        public string FlowId { get; set; }

        /// <summary>
        ///流程步骤名称
        /// </summary>
        /// <returns></returns>
        public string FlowPrcs { get; set; }

        /// <summary>
        /// 接收用户的id
        /// </summary>
        /// <returns></returns>
        public string beginUserId { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        /// <returns></returns>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 转交用户id
        /// </summary>
        /// <returns></returns>
        public string deliverUserId { get; set; }

        /// <summary>
        /// 转交时间
        /// </summary>
        /// <returns></returns>
        public DateTime? deliverTime { get; set; }

        /// <summary>
        /// 办理时长 小时 转交是计算
        /// </summary>
        /// <returns></returns>
        public int timeLong { get; set; }

        /// <summary>
        /// 状态码 步骤标志位--1待接手2办理中3已办结
        /// </summary>
        /// <returns></returns>
        public string runFlag { get; set; }

        /// <summary>
        /// 办理的过程信息
        /// </summary>
        /// <returns></returns>
        public string stepInfo { get; set; }

        /// <summary>
        /// 意见或者建议
        /// </summary>
        /// <returns></returns>
        public string deliverComments { get; set; }

        /// <summary>
        /// 意见或者建议 是否公开 1是2否
        /// </summary>
        /// <returns></returns>
        public string Ispublic { get; set; }

        /// <summary>
        /// 转交类型 0新建 1选择下一步转交 2选择回退转交 9通过数据管理进行转交
        /// </summary>
        /// <returns></returns>
        public string deliverType { get; set; }
    }
}