using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("tbbasic")]
    public class TbBasicEntity
    {
        /// <summary>
        /// 报表编号
        /// </summary>
        public string TbId
        {
            get; set;
        }

        /// <summary>
        /// 流程编码 bi_基础信息 fw_一般录入表
        /// </summary>
        public string FlowId
        {
            get; set;
        }

        /// <summary>
        /// 是否基础信息表？0不确定1基础信息表2一般录入表
        /// </summary>
        public string isInfo
        {
            get; set;
        }

        /// <summary>
        /// 报表类型--1表示主表2表示子表
        /// </summary>
        public string TbType
        {
            get; set;
        }

        /// <summary>
        /// 分类id
        /// </summary>
        public string TbSort
        {
            get; set;
        }

        /// <summary>
        /// 报表名称
        /// </summary>
        public string TbName
        {
            get; set;
        }

        /// <summary>
        /// 对应的主表编码--对子表 空值为主表
        /// </summary>
        public string ParentId
        {
            get; set;
        }

        /// <summary>
        /// 对象类别--数据字典
        /// </summary>
        public string ObjectType
        {
            get; set;
        }

        /// <summary>
        /// 填报周期--见数据字典
        /// </summary>
        public string TbCyc
        {
            get; set;
        }

        /// <summary>
        /// 报表填报的前推周期
        /// </summary>
        public int NTime
        {
            get; set;
        }

        /// <summary>
        /// 填报说明
        /// </summary>
        public string TbExplain
        {
            get; set;
        }

        /// <summary>
        /// 样式字符串？自画 定制 系统默认
        /// </summary>
        public string TbStyle
        {
            get; set;
        }

        /// <summary>
        /// 标志字符串
        /// </summary>
        public string TbFlag
        {
            get; set;
        }

        /// <summary>
        /// 数据库中是否存在该表？1存在2不存在
        /// </summary>
        public string isBb
        {
            get; set;
        }

        /// <summary>
        /// 是否锁定？1是2否
        /// </summary>
        public string isLock
        {
            get; set;
        }

        /// <summary>
        /// 是否允许其余报表读取该表信息?1允许2不允许
        /// </summary>
        public string isReadValue
        {
            get; set;
        }

        /// <summary>
        /// 是否允许其余报表修改该表信息?1允许2不允许
        /// </summary>
        public string isUpValue
        {
            get; set;
        }

        /// <summary>
        /// 否可编辑？表示是否可编辑属性1允许2不允许
        /// </summary>
        public string isEdit
        {
            get; set;
        }

        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderNo
        {
            get; set;
        }

        /// <summary>
        /// 启用禁用
        /// </summary>
        public string isUse
        {
            get; set;
        }

        /// <summary>
        /// 最后更新记录ID
        /// </summary>
        public int UpRecordID
        {
            get; set;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        public string createUser
        {
            get; set;
        }

        /// <summary>
        /// 新增时间
        /// </summary>
        public DateTime createTime
        {
            get; set;
        }

        /// <summary>
        /// 左侧是否num，对子表有效 ？1是2不是
        /// </summary>
        public string isLeftNum
        {
            get; set;
        }

        /// <summary>
        /// 是否软删除，对主表有效 ？1是2不是
        /// </summary>
        public string isSoftDel
        {
            get; set;
        }

        /// <summary>
        /// 是否辅助表？1是2不是
        /// </summary>
        public string isAux
        {
            get; set;
        }

        /// <summary>
        /// 锁定期限
        /// </summary>
        public int LockUpTime
        {
            get; set;
        }

        /// <summary>
        /// 子表数量
        /// </summary>
        [Computed]
        public string GridCount { get; set; }

        /// <summary>
        /// 模式？0不确定1发布模式2调试模式
        /// </summary>
        [Computed]
        public string Model { get; set; }

        /// <summary>
        /// 0无增加、删除按钮 1增加和删除按钮在右侧 2增加和删除按钮在顶部
        /// </summary>
        [Computed]
        public string InType { get; set; }

        /// <summary>
        /// 流程类型？1固定2自由流程 0无流程
        /// </summary>
        [Computed]
        public string flowType { get; set; }

        [Computed]
        public string EditUser_Exa { get; set; }

        [Computed]
        public string EditUser { get; set; }

        [Computed]
        public string indexCount { get; set; }

        [Computed]
        public string TbRelationCount { get; set; }//

        [Computed]
        public string TbRelationCount11 { get; set; }//

        [Computed]
        public string TbRelationCount21 { get; set; }//

        [Computed]
        public string TbRelationCount31 { get; set; }//

        [Computed]
        public string ValueCheckCount { get; set; }

        [Computed]
        public string EventCount { get; set; }

        [Computed]
        public string ButCount { get; set; }

        [Computed]
        public string ButEdit { get; set; }

        [Computed]
        public string ButEditTxt { get; set; }

        [Computed]
        public string ButEditWarn { get; set; }

        [Computed]
        public string ButNext { get; set; }

        [Computed]
        public string ButNextTxt { get; set; }

        [Computed]
        public string ButNextWarn { get; set; }

        [Computed]
        public string ButFinish { get; set; }

        [Computed]
        public string ButFinishTxt { get; set; }

        [Computed]
        public string ButFinishWarn { get; set; }

        [Computed]
        public string ButAtt { get; set; }

        [Computed]
        public string ButAttTxt { get; set; }

        [Computed]
        public string ButAttWarn { get; set; }

        [Computed]
        public string ButPrint { get; set; }

        [Computed]
        public string ButPrintTxt { get; set; }

        [Computed]
        public string ButPrintWarn { get; set; }

        [Computed]
        public string ButPDFDetails { get; set; }

        [Computed]
        public string ButPDFDetailsTxt { get; set; }

        [Computed]
        public string ButExcelDetails { get; set; }

        [Computed]
        public string ButExcelDetailsTxt { get; set; }

        [Computed]
        public string ButDOCDetails { get; set; }

        [Computed]
        public string ButDOCDetailsTxt { get; set; }

        [Computed]
        public string ButAttDetails { get; set; }

        [Computed]
        public string ButAttDetailsTxt { get; set; }

        [Computed]
        public string TempletCount { get; set; }

        [Computed]
        public string TempDirectoryCount { get; set; }

        [Computed]
        public string BtnCustomizeCount { get; set; }//create

        [Computed]
        public string CreateCount { get; set; }

        [Computed]
        public string HiddenIndexCount { get; set; }

        #region list列表页面按钮

        [Computed]
        public string ButListImportExcel { get; set; }//数据导入

        [Computed]
        public string ButListBatchDel { get; set; }//批量删除

        [Computed]
        public string ButListMergeExport { get; set; }//合并导出

        [Computed]
        public string ButListPrcs { get; set; }//流程查看

        [Computed]
        public string ButListDetails { get; set; }//详细

        [Computed]
        public string ButListDetailsTxt { get; set; }

        [Computed]
        public string ButListDetailsExa { get; set; }

        [Computed]
        public string ButListRecord { get; set; }//记录

        [Computed]
        public string ButListRecordTxt { get; set; }

        [Computed]
        public string ButListEdit { get; set; }//编辑

        [Computed]
        public string ButListEditTxt { get; set; }

        [Computed]
        public string ButListDel { get; set; }//删除

        [Computed]
        public string ButListDelTxt { get; set; }

        [Computed]
        public string ButListADD { get; set; }//新增按钮

        [Computed]
        public string ButListADDTxt { get; set; }

        [Computed]
        public string IsModuleExists { get; set; }

        [Computed]
        public string IsDataTableExists { get; set; }

        [Computed]
        public string IsFileExists { get; set; }

        [Computed]
        public string IsTbJson { get; set; }

        [Computed]
        public string isCreatBtn { get; set; }

        #endregion list列表页面按钮
    }
}