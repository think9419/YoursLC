﻿using DapperExtensions;

/// <summary>
/// 基础信息表Models即ListId自增长的 都继承此类
/// </summary>
namespace Think9.Models
{
    public class BasicReportEntity
    {
        /// <summary>
        /// 主键 来源于flowrunlist
        /// </summary>
        //[DapperExtensions.Key(true)]
        public long listid { get; set; }

        /// <summary>
        /// 子表id
        /// </summary>
        [Computed]
        public long id { get; set; }

        /// <summary>
        /// 1有效0无效-1已删除
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 0未锁定1已锁定
        /// </summary>
        [Computed]
        public string isLock { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Computed]
        public string createTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        [Computed]
        public string createUser { get; set; }

        /// <summary>
        /// 创建用户所在部门
        /// </summary>
        [Computed]
        public string createDept { get; set; }

        /// <summary>
        /// 所有的上级单位(部门)编码通过;相连的字符
        /// </summary>
        [Computed]
        public string createDeptStr { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Computed]
        public string runName { get; set; }

        /// <summary>
        /// 流程步骤
        /// </summary>
        [Computed]
        public string currentPrcsName { get; set; }

        /// <summary>
        /// runFlag流程步骤标志位--0待新建1待接手2办理中3已办结
        /// </summary>
        [Computed]
        public string runFlag { get; set; }

        /// <summary>
        /// 公共附件id
        /// </summary>
        [Computed]
        public string attachmentId { get; set; }

    }
}