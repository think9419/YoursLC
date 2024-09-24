using DapperExtensions;

namespace Think9.Models
{
    [Table("tbindex")]
    public class TbIndexEntity
    {
        #region Model

        /// <summary>
        /// 报表编码
        /// </summary>
        public string TbId
        {
            get; set;
        }

        /// <summary>
        /// 指标编码
        /// </summary>
        public string IndexId
        {
            get; set;
        }

        /// <summary>
        /// 指标名称
        /// </summary>
        public string IndexName
        {
            get; set;
        }

        /// <summary>
        /// Index编号 系统自动生成
        /// </summary>
        public int IndexNo
        {
            get; set;
        }

        /// <summary>
        /// 数据类型 1000:日期 2000:文字(不限定长度) 2030:文字(30字符以内) 2100:文字(100字符以内) 2500:文字(500字符以内) 3000:整数 3002:2位小数 3004:4位小数 3006:6位小数 3102:金额 3202:金额 5200:图片
        /// </summary>
        public string DataType
        {
            get; set;
        }

        /// <summary>
        /// 输入方式 0直接输入 1单选 2多选
        /// </summary>
        public string InType
        {
            get; set;
        }

        /// <summary>
        /// 列表表头名称
        /// </summary>
        public string ListHeadName
        {
            get; set;
        }

        /// <summary>
        /// 统计求和模式0不统计1求和2平均3最大4最小
        /// </summary>
        public string ListStat
        {
            get; set;
        }

        /// <summary>
        /// 列宽
        /// </summary>
        public int ColumnWith
        {
            get; set;
        }

        /// <summary>
        /// 列高--子表中使用，控件（文本框）高度
        /// </summary>
        public int ColumnHeight
        {
            get; set;
        }

        /// <summary>
        /// 主键？1是2不是
        /// </summary>
        public string isPK
        {
            get; set;
        }

        /// <summary>
        /// 唯一？1是2不是
        /// </summary>
        public string isUnique
        {
            get; set;
        }

        /// <summary>
        /// Value可否为空？1可以2不可以
        /// </summary>
        public string isEmpty
        {
            get; set;
        }

        /// <summary>
        /// 查询列表中该列显示？1显示2不显示
        /// </summary>
        public string isColumnShow
        {
            get; set;
        }

        /// <summary>
        /// 列表显示(list)？9只是移动端不显示1显示2不显示
        /// </summary>
        public string isColumnShow2
        {
            get; set;
        }

        /// <summary>
        /// 是否做为查询条件？1是2不是
        /// </summary>
        public string isSearch
        {
            get; set;
        }

        /// <summary>
        /// 快速查询？1是2不是
        /// </summary>
        public string isSearch2
        {
            get; set;
        }

        /// <summary>
        /// 是否排序？1排序2不排序
        /// </summary>
        public string isOrder
        {
            get; set;
        }

        /// <summary>
        /// value是否可编辑？保存后1可以2不可以
        /// </summary>
        public string isEdit
        {
            get; set;
        }

        /// <summary>
        /// 数据库中是否存在该字段？1存在2不存在
        /// </summary>
        public string isBb
        {
            get; set;
        }

        /// <summary>
        /// 是否锁定？1是2否3按行锁定 9编辑时锁定
        /// </summary>
        public string isLock
        {
            get; set;
        }

        /// <summary>
        /// 按行锁定
        /// </summary>
        public string isLock2
        {
            get; set;
        }

        /// <summary>
        /// 日期加时间 1是
        /// </summary>
        public string isTime
        {
            get; set;
        }

        /// <summary>
        /// Index排序号
        /// </summary>
        public int IndexOrderNo
        {
            get; set;
        }

        /// <summary>
        /// 标志字符串
        /// </summary>
        public string Flag
        {
            get; set;
        }

        /// <summary>
        /// 格式类型
        /// </summary>
        public string FormatType
        {
            get; set;
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultV
        {
            get; set;
        }

        /// <summary>
        /// 数据规范名称
        /// </summary>
        [Computed]
        public string RuleName
        {
            get; set;
        }

        /// <summary>
        /// 规范类型？1系统指标2自动编号6单列选择7多列选择8树形选择
        /// </summary>
        public string RuleType
        {
            get; set;
        }

        /// <summary>
        /// 数据规范ID
        /// </summary>
        public string RuleId
        {
            get; set;
        }

        /// <summary>
        /// 值对应的value字段
        /// </summary>
        public string ListVField
        {
            get; set;
        }

        /// <summary>
        /// 值对应的显示字段
        /// </summary>
        public string ListTField
        {
            get; set;
        }

        /// <summary>
        /// 数据集读数 select部分
        /// </summary>
        public string ListStrSelect
        {
            get; set;
        }

        /// <summary>
        /// 数据集读数 from部分
        /// </summary>
        public string ListStrFrm
        {
            get; set;
        }

        /// <summary>
        /// 数据集读数 where部分
        /// </summary>
        public string ListStrWhere
        {
            get; set;
        }

        /// <summary>
        /// 附加的条件
        /// </summary>
        public string ListAddStrWhere
        {
            get; set;
        }

        /// <summary>
        /// 数据集读数 order部分
        /// </summary>
        public string ListStrOrder
        {
            get; set;
        }

        /// <summary>
        /// 是否可多选？1是2不是
        /// </summary>
        public string isSelMuch
        {
            get; set;
        }

        /// <summary>
        /// 录入表中的控件id 如果有多个用；分割
        /// </summary>
        public string ControlId1
        {
            get; set;
        }

        /// <summary>
        /// LIST中的控件id 如果有多个用；分割
        /// </summary>
        public string ControlId2
        {
            get; set;
        }

        /// <summary>
        /// REPORT表中的控件id 如果有多个用；分割
        /// </summary>
        public string ControlId3
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
        /// 是否使用？1启用2禁用
        /// </summary>
        public string isUse
        {
            get; set;
        }

        /// <summary>
        /// 指标提示
        /// </summary>
        public string ControlBy1
        {
            get; set;
        }

        /// <summary>
        ///控件类型 1文本框 2下拉框3单选框4多选框9图片
        /// </summary>
        public string ControlBy2
        {
            get; set;
        }

        /// <summary>
        /// 规则-- Email电子邮箱 IdCart身份证号码 Phone手机号码 Url网址 Number数值
        /// </summary>
        public string ControlBy3
        {
            get; set;
        }

        /// <summary>
        /// 数据规范的查询关键字
        /// </summary>
        public string ControlBy4
        {
            get; set;
        }

        /// <summary>
        /// 备用
        /// </summary>
        public string ControlBy5
        {
            get; set;
        }

        /// <summary>
        /// 水平排列
        /// </summary>
        public string TextAlign
        {
            get; set;
        }

        /// <summary>
        /// 垂直排列
        /// </summary>
        public string VerticalAlign
        {
            get; set;
        }

        /// <summary>
        /// 链接属性 0无 1图片 2www
        /// </summary>
        public string isLink
        {
            get; set;
        }

        /// <summary>
        /// 是否扫码？0否1是
        /// </summary>
        public string Code01
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public string Code02
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public string Auto01
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public string Auto02
        {
            get; set;
        }

        /// <summary>
        ///1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片
        /// </summary>
        public string ControlType
        {
            get; set;
        }

        public string FileType
        {
            get; set;
        }

        /// <summary>
        /// 隐藏指标？1不是2是
        /// </summary>
        public string isShow
        {
            get; set;
        }

        /// <summary>
        /// 显示HtmlTag？1显示2不显示
        /// </summary>
        public string isHtmlTag
        {
            get; set;
        }

        public string LinkFlag
        {
            get; set;
        }

        public string LinkFlag2
        {
            get; set;
        }

        [Computed]
        public string LinkStr
        {
            get; set;
        }

        /// <summary>
        /// 报表名称
        /// </summary>
        [Computed]
        public string TbName
        {
            get; set;
        }

        /// <summary>
        /// 指标类型
        /// </summary>
        [Computed]
        public string DataTypeName
        {
            get; set;
        }

        /// <summary>
        /// 其他信息
        /// </summary>
        [Computed]
        public string ExInfo
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        [Computed]
        public string ControlType_Exa
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        [Computed]
        public string Rule_Exa
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        [Computed]
        public string Auto02_Exa
        {
            get; set;
        }

        #endregion Model
    }
}