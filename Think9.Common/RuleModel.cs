using System.Collections.Generic;

namespace Think9.Services.Com
{
    public class ModelRule
    {
        //加230805

        /// <summary>
        ///  完整的查询语句 查询页面
        /// </summary>
        public string SqlList { get; set; }

        /// <summary>
        ///  完整的查询语句 查询页面
        /// </summary>
        public string SqlList_PG { get; set; }

        /// <summary>
        ///  完整的查询语句 查询页面
        /// </summary>
        public string SqlList_ORACLE { get; set; }

        /// <summary>
        ///  完整的查询语句 编辑页面
        /// </summary>
        public string SqlForm { get; set; }

        /// <summary>
        ///  完整的查询语句 查询页面
        /// </summary>
        public string SqlForm_PG { get; set; }

        /// <summary>
        ///  完整的查询语句 查询页面
        /// </summary>
        public string SqlForm_ORACLE { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DbID
        {
            get; set;
        }

        /// <summary>
        /// 报表类型？1主表2子表
        /// </summary>
        public string TbType
        {
            get; set;
        }

        /// <summary>
        /// 是否可多选？1可以2不可以
        /// </summary>
        public string isSelMuch { get; set; }

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
        /// //1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片
        /// </summary>
        public string ControlType { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 指标名称
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// 数据规范ID
        /// </summary>
        public string RuleId { get; set; }

        /// <summary>
        /// 数据规范类型?1系统指标2自动编号6单列选择7多列选择8树形选择
        /// </summary>
        public string RuleType { get; set; }

        /// <summary>
        ///  取值数据表1数据字典取值2录入表取值
        /// </summary>
        public string ValueScope { get; set; }

        /// <summary>
        /// 从哪个表取值
        /// </summary>
        public string FrmTB { get; set; }

        /// <summary>
        /// 值字段
        /// </summary>
        public string ValueFiled { get; set; }

        /// <summary>
        /// 显示字段
        /// </summary>
        public string TxtFiled { get; set; }

        public string DictItemId { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderFiled { get; set; }

        /// <summary>
        /// 排序方式？1正序2反序
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 查询字段，以；分割
        /// </summary>
        public string SearchFiled { get; set; }

        /// <summary>
        ///  限定字符串
        /// </summary>
        public string LimitStr { get; set; }

        /// <summary>
        ///  查询页面
        /// </summary>
        public string WhereList { get; set; }

        /// <summary>
        ///  编辑页面
        /// </summary>
        public string WhereForm { get; set; }

        public List<IdValueModel> list { get; set; }
    }
}