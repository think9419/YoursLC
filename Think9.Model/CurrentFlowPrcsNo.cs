namespace Think9.Models
{
    public class CurrentPrcsEntity
    {
        //public int code { get; set; }//

        /// <summary>
        /// flowrunlist关联id
        /// </summary>
        public int ListId { get; set; }//

        /// <summary>
        /// flowrunlist与FlowRunPrcsList关联id
        /// </summary>
        public int relatedId { get; set; }

        /// <summary>
        /// 流程是否启用？1是2否code
        /// </summary>
        public string isUse
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
        /// 0未锁定1已锁定
        /// </summary>
        public string isLock { get; set; }

        public string TbId { get; set; }

        /// <summary>
        /// bi_基础信息 fw_一般录入表
        /// </summary>
        public string FlowId { get; set; }

        public string FlowName { get; set; }

        /// <summary>
        /// 流程类型？1固定2自由流程 0无流程
        /// </summary>
        public string flowType { get; set; }

        /// <summary>
        /// flowprcs对应的主键id
        /// </summary>
        public string PrcsId { get; set; }//

        /// <summary>
        /// flowprcs对应的步骤编码
        /// </summary>
        public string PrcsNo { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public string PrcsOrder
        {
            get; set;
        }

        /// <summary>
        /// 对应的步骤名称
        /// </summary>
        public string PrcsName { get; set; }

        /// <summary>
        /// runFlag流程步骤标志位--0待新建1待接手2办理中3已办结
        /// </summary>
        public string runFlag
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

        /// <summary>
        /// 保密字段
        /// </summary>
        public string HiddenIndex
        {
            get; set;
        }

        /// <summary>
        /// 流程当前办理人
        /// </summary>
        public string currentPrcsUser1
        {
            get; set;
        }

        /// <summary>
        /// 流程办理人
        /// </summary>
        public string transactUser1
        {
            get; set;
        }

        /// <summary>
        ///流程步骤设定的授权范围-人员
        /// </summary>
        public string PrcsUser
        {
            get; set;
        }

        /// <summary>
        /// 流程步骤设定的授权范围-部门
        /// </summary>
        public string PrcsDept
        {
            get; set;
        }

        /// <summary>
        /// 流程步骤设定的授权范围-角色
        /// </summary>
        public string PrcsPriv
        {
            get; set;
        }

        /// <summary>
        /// 错误提示
        /// </summary>
        public string ERR
        {
            get; set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        /// 所有的上级单位(部门)编码通过;相连的字符
        /// </summary>
        public string createDeptStr { get; set; }

        /// <summary>
        /// 是用户设置的单位(部门)编码
        /// </summary>
        public string createDept { get; set; }
    }
}