using DapperExtensions;
using System;

/// <summary>
/// 基础信息表Models即ListId自增长的 都继承此类
/// </summary>
namespace Think9.Models
{
    public class MainTB2Entity
    {
        /// <summary>
        /// 主键 来源于flowrunlist
        /// </summary>
        //[DapperExtensions.Key(true)]
        public long ListId { get; set; }

        /// <summary>
        /// [Computed]
        /// </summary>
        [Computed]
        public string ListId_Exa { get; set; }

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

        /// <summary>
        /// 禁用的控件
        /// </summary>
        [Computed]
        public string DisableButStr { get; set; }

        /// <summary>
        /// 排序字符
        /// </summary>
        [Computed]
        public string OrderStr { get; set; }

        [Computed]
        public string DetailLink { get; set; }

        [Computed]
        public string currentDeptName { get; set; }

        [Computed]
        public string currentDeptNo { get; set; }

        [Computed]
        public string currentUserId { get; set; }

        [Computed]
        public string currentRoleNo { get; set; }

        [Computed]
        public string currentRoleName { get; set; }

        [Computed]
        public string currentUserName { get; set; }

        [Computed]
        public DateTime timeNow { get; set; }

        [Computed]
        public DateTime timeToday { get; set; }
    }
}