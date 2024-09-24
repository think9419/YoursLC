using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("flowprcs")]
    public class FlowPrcsEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int PrcsId
        {
            get; set;
        }

        [Computed]
        public string FlowName
        {
            get; set;
        }

        /// <summary>
        /// 流程编号对应Flow表
        /// </summary>
        public string FlowId
        {
            get; set;
        }

        /// <summary>
        /// 流程步骤第1步？1是2否
        /// </summary>
        public int isFirst
        {
            get; set;
        }

        /// <summary>
        /// 步骤编码
        /// </summary>
        public string PrcsNo
        {
            get; set;
        }

        /// <summary>
        /// 下一步
        /// </summary>
        [Computed]
        public string strNext
        {
            get; set;
        }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string PrcsName
        {
            get; set;
        }

        /// <summary>
        /// 可写字段 #all#表示全部
        /// </summary>
        public string PrcsIndex
        {
            get; set;
        }

        [Computed]
        public string PrcsIndex_Exa
        {
            get; set;
        }

        /// <summary>
        /// 保密字段
        /// </summary>
        public string HiddenIndex
        {
            get; set;
        }

        [Computed]
        public string HiddenIndex_Exa
        {
            get; set;
        }

        /// <summary>
        /// 授权范围-人员
        /// </summary>
        public string PrcsUser
        {
            get; set;
        }

        [Computed]
        public string PrcsUser_Exa
        {
            get; set;
        }

        /// <summary>
        /// 授权范围-部门
        /// </summary>
        public string PrcsDept
        {
            get; set;
        }

        [Computed]
        public string PrcsDept_Exa
        {
            get; set;
        }

        /// <summary>
        /// 授权范围-角色
        /// </summary>
        public string PrcsPriv
        {
            get; set;
        }

        [Computed]
        public string PrcsPriv_Exa
        {
            get; set;
        }

        /// <summary>
        /// 公共附件选项?新建编辑删除下载打印 1有权限2无
        /// </summary>
        public string BAttachment
        {
            get; set;
        }

        /// <summary>
        /// 公共附件选项?新建 1有权限2无
        /// </summary>
        [Computed]
        public string A1
        {
            get; set;
        }

        /// <summary>
        /// 公共附件选项?下载 1有权限2无
        /// </summary>
        [Computed]
        public string A2
        {
            get; set;
        }

        /// <summary>
        /// 公共附件选项?删除 1有权限2无
        /// </summary>
        [Computed]
        public string A3
        {
            get; set;
        }

        /// <summary>
        /// 图片化显示的坐标
        /// </summary>
        public int SetLeft
        {
            get; set;
        }

        /// <summary>
        /// 图片化显示的坐标
        /// </summary>
        public int SetTop
        {
            get; set;
        }

        /// <summary>
        /// 插件程序名称--插件程序将在本步骤执行完毕后被自动调用执行
        /// </summary>
        public string Plugin
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public string AutoItem
        {
            get; set;
        }

        /// <summary>
        /// 转入条件列表
        /// </summary>
        public string PrcsIn
        {
            get; set;
        }

        /// <summary>
        /// 转出条件列表
        /// </summary>
        public string PrcsOut
        {
            get; set;
        }

        /// <summary>
        /// 转入条件--条件列表加逻辑运算
        /// </summary>
        public string PrcsInSet
        {
            get; set;
        }

        /// <summary>
        /// 转出条件--条件列表加逻辑运算
        /// </summary>
        public string PrcsOutSet
        {
            get; set;
        }

        /// <summary>
        /// 是否允许会签？1允许会签2禁止会签3强制会签
        /// </summary>
        public string Feedback
        {
            get; set;
        }

        /// <summary>
        /// 是否允许查看其他人的会签意见？1允许2禁止
        /// </summary>
        public string SignLook
        {
            get; set;
        }

        /// <summary>
        /// 强制转交--经办人未办理完毕时是否允许主办人强制转交？1允许2
        /// </summary>
        public string TurnPriv
        {
            get; set;
        }

        /// <summary>
        /// 转交时邮件自动通知以下人员
        /// </summary>
        public string MailTo
        {
            get; set;
        }

        /// <summary>
        /// 选人过滤规则？1允许选择全部指定的经办人2只允许本部门经办人3只允许本角色经办人
        /// </summary>
        public string UserFilter
        {
            get; set;
        }

        /// <summary>
        /// 自动选人规则？1不进行自动选择2自动选择流程发起人3自动选择本部门主管4指定自动选人默认人员5自动选择上级单位(部门)主管6自动选择一级部门主管
        /// </summary>
        public string AutoType
        {
            get; set;
        }

        /// <summary>
        /// 是否允许更改指定经办人及相关选项？1允许2不允许
        /// </summary>
        public string UserLock
        {
            get; set;
        }

        /// <summary>
        /// 是否默认由最先接收的人办理？1是2否
        /// </summary>
        public string TopDefault
        {
            get; set;
        }

        /// <summary>
        /// 未知
        /// </summary>
        public string AutoUserOp
        {
            get; set;
        }

        /// <summary>
        /// 未知
        /// </summary>
        public string AutoUser
        {
            get; set;
        }

        /// <summary>
        /// 办理时限--为空表示不限时
        /// </summary>
        public int TimeOut
        {
            get; set;
        }

        /// <summary>
        /// 回退选项？1不允许2允许回退上一步骤3允许回退之前步骤
        /// </summary>
        public string AllowBack
        {
            get; set;
        }

        /// <summary>
        /// 并发相关选项？--不用
        /// </summary>
        public int SyncDeal
        {
            get; set;
        }

        /// <summary>
        /// 并发合并选项--不用
        /// </summary>
        public int GatherNode
        {
            get; set;
        }

        /// <summary>
        /// 子流程类型--不用
        /// </summary>
        public int ChildFlow
        {
            get; set;
        }

        /// <summary>
        /// 返回步骤
        /// </summary>
        public int PrcsBack
        {
            get; set;
        }

        /// <summary>
        /// 未知
        /// </summary>
        public string AttachPriv
        {
            get; set;
        }

        /// <summary>
        /// 未知
        /// </summary>
        public string ConditionDesc
        {
            get; set;
        }

        /// <summary>
        /// 办理时限-起始时间
        /// </summary>
        public DateTime? TimeStart
        {
            get; set;
        }

        /// <summary>
        /// 办理时限-截至时间
        /// </summary>
        public DateTime? TimeEnd
        {
            get; set;
        }

        /// <summary>
        /// 转交前签署意见类型
        /// </summary>
        public string OpinionType
        {
            get; set;
        }

        /// <summary>
        /// 排序号
        /// </summary>
        public int? PrcsOrder
        {
            get; set;
        }
    }
}