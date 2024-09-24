using DapperExtensions;

namespace Think9.Models
{
    [Table("flow")]
    public class FlowEntity
    {
        /// <summary>
        /// 报表编号
        /// </summary>
        public string TbId
        {
            get; set;
        }

        /// <summary>
        /// 流程编码   bi_基础信息 fw_一般录入表
        /// </summary>
        public string FlowId
        {
            get; set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string FlowName
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public string FlowDoc
        {
            get; set;
        }

        /// <summary>
        /// 流程类型？1固定2自由流程 0无流程
        /// </summary>
        public string flowType
        {
            get; set;
        }

        /// <summary>
        /// 对基础信息表，有修改权限的用户
        /// </summary>
        public string EditUser
        {
            get; set;
        }

        [Computed]
        public string EditUser_Exa
        {
            get; set;
        }

        /// <summary>
        /// 查看编辑模式
        /// 11查看编辑用户本人新建的数据
        ///22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
        ///21查看编辑用户所属单位(部门)新建的数据
        ///32查看用户所属下级部门新建的数据  编辑本人新建的数据
        ///31查看编辑用户所属下级部门新建的数据
        ///42查看所有数据  编辑本人新建的数据
        ///41查看编辑所有数据
        /// </summary>
        public string SearchMode
        {
            get; set;
        }

        /// <summary>
        /// 查看编辑模式
        /// 11查看编辑用户本人新建的数据
        ///22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
        ///21查看编辑用户所属单位(部门)新建的数据
        ///32查看用户所属下级部门新建的数据  编辑本人新建的数据
        ///31查看编辑用户所属下级部门新建的数据
        ///42查看所有数据  编辑本人新建的数据
        ///41查看编辑所有数据
        /// </summary>
        [Computed]
        public string SearchMode_Exa
        {
            get; set;
        }

        /// <summary>
        /// 管理权限所有数据
        /// </summary>
        public string ManageUser
        {
            get; set;
        }

        [Computed]
        public string ManageUser_Exa
        {
            get; set;
        }

        /// <summary>
        /// 管理权限下级部门数据
        /// </summary>
        public string ManageUser2
        {
            get; set;
        }

        [Computed]
        public string ManageUser2_Exa
        {
            get; set;
        }

        /// <summary>
        /// 查询权限所有数据
        /// </summary>
        public string QueryUser
        {
            get; set;
        }

        [Computed]
        public string QueryUser_Exa
        {
            get; set;
        }

        /// <summary>
        /// 查询权限下级部门数据
        /// </summary>
        public string QueryUser2
        {
            get; set;
        }

        [Computed]
        public string QueryUser2_Exa
        {
            get; set;
        }

        /// <summary>
        /// 自动名称表达式类型
        /// </summary>
        public string AutoNameType
        {
            get; set;
        }

        /// <summary>
        /// FlowAutoName包含的数量，决定是否需要回写名称
        /// </summary>
        public int AutoNum
        {
            get; set;
        }

        /// <summary>
        /// 自动编号类型
        /// </summary>
        public int AutoLen
        {
            get; set;
        }

        /// <summary>
        /// 允许手工编辑名称？1允许2不允许
        /// </summary>
        public string isEdit
        {
            get; set;
        }

        /// <summary>
        /// 标识字符串
        /// </summary>
        public string FlowFlag
        {
            get; set;
        }

        /// <summary>
        ///标识字符串 填报页面按钮页面跳转
        /// </summary>
        public string FlowFlag2
        {
            get; set;
        }

        /// <summary>
        /// 标识字符串
        /// </summary>
        public string FlowFlag3
        {
            get; set;
        }

        /// <summary>
        /// 标识字符串
        /// </summary>
        public string FlowFlag4
        {
            get; set;
        }

        /// <summary>
        /// 标识字符串
        /// </summary>
        public string FlowFlag5
        {
            get; set;
        }

        /// <summary>
        /// 附件分类
        /// </summary>
        public string FlowAttachment
        {
            get; set;
        }

        /// <summary>
        /// 允许附件类型
        /// </summary>
        public string FlowAttachment2
        {
            get; set;
        }

        /// <summary>
        /// 禁止附件类型
        /// </summary>
        public string FlowAttachment3
        {
            get; set;
        }

        /// <summary>
        /// 自动锁定 1是2否
        /// </summary>
        public string AutoLock
        {
            get; set;
        }

        /// <summary>
        /// 按钮标题
        /// </summary>
        public string ButonTtitle
        {
            get; set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string FlowDesc
        {
            get; set;
        }

        /// <summary>
        /// 是否启用？1是2否
        /// </summary>
        public string isUse
        {
            get; set;
        }
    }
}