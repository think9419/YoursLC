using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("flowrunlist")]
    public class FlowRunListEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public long listid { get; set; }

        /// <summary>
        /// 工作编号
        /// </summary>
        public string ruNumber { get; set; }

        /// <summary>
        /// 工作名称
        /// </summary>
        public string runName { get; set; }

        /// <summary>
        /// 流程编码
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// 流程类型？1固定2自由流程 0无流程
        /// </summary>
        public string flowType { get; set; }

        /// <summary>
        /// 录入表编码
        /// </summary>
        public string TbId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        ///是所有的上级单位(部门)编码通过;相连的字符
        /// </summary>
        public string createDeptStr { get; set; }

        /// <summary>
        /// 是用户设置的单位(部门)编码
        /// </summary>
        public string createDept { get; set; }

        /// <summary>
        /// 上级单位(部门)编码
        /// </summary>
        public string CreateUpDept { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 正进行的流程步骤id
        /// </summary>
        public int? currentPrcsId { get; set; }

        /// <summary>
        /// 正进行的流程步骤名称
        /// </summary>
        public string currentPrcsName { get; set; }

        /// <summary>
        /// 上一步流程步骤id 如果是自由流程step+数字 固定流程纯数字（即PrcsId）
        /// </summary>
        public string prevPrcsId { get; set; }

        /// <summary>
        /// 上一步流程步骤的转交人 用户id
        /// </summary>
        public string prevUserId { get; set; }

        /// <summary>
        /// 流程步骤标志位--1待接手2办理中3已办结
        /// </summary>
        public string runFlag { get; set; }

        /// <summary>
        /// 关联id--FlowRunPrcsList表中的
        /// </summary>
        public int? relatedId { get; set; }

        /// <summary>
        /// currentPrcsUser  正进行的流程步骤授权范围-人员
        /// </summary>
        public string currentPrcsUser { get; set; }

        /// <summary>
        ///  正进行的流程步骤授权范围-部门
        /// </summary>
        public string currentPrcsDept { get; set; }

        /// <summary>
        ///  正进行的流程步骤授权范围-角色
        /// </summary>
        public string currentPrcsPriv { get; set; }

        /// <summary>
        ///  流程当前办理人
        /// </summary>
        public string currentPrcsUser1 { get; set; }

        /// <summary>
        ///  正进行的流程经办人
        /// </summary>
        public string currentPrcsUser2 { get; set; }

        /// <summary>
        /// currentPrcsTransact
        /// </summary>
        public string currentPrcsTransact { get; set; }

        /// <summary>
        /// transactUser1   本流程所有的办理人即经手人
        /// </summary>
        public string transactUser1 { get; set; }

        /// <summary>
        /// transactUser2   本步骤所有的经办人
        /// </summary>
        public string transactUser2 { get; set; }

        /// <summary>
        /// 公共附件id
        /// </summary>
        public string attachmentId { get; set; }

        /// <summary>
        /// 流程是否完成？1是2否
        /// </summary>
        public string isFinish { get; set; }

        /// <summary>
        /// 0未锁定1已锁定
        /// </summary>
        public string isLock { get; set; }

        /// <summary>
        /// isSetUp 是否报送？0待报送1已报送
        /// </summary>
        public string isSetUp { get; set; }

        /// <summary>
        /// isInspect 是否审核通过？0待审核1审核通过2审核未通过zzz
        /// </summary>
        public string isInspect { get; set; }

        /// <summary>
        /// 流程类型？1固定2自由流程 0无流程
        /// </summary>
        public string FlowFlag { get; set; }

        /// <summary>
        /// 审核模式？0不确定1流程模式2统一审核
        /// </summary>
        public int? reportMode { get; set; }

        /// <summary>
        /// 是否为空
        /// </summary>
        [Computed]
        public string isNull { get; set; }
    }
}