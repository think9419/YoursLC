using DapperExtensions;

namespace Think9.Models
{
    [Table("rulemultiple")]
    public class RuleMultipleEntity
    {
        //[DapperExtensions.Key(true)]
        public string RuleId { get; set; }

        // 取值数据表1固定取值2动态取值
        public string ValueScope { get; set; }

        // 取值数据表
        public string TbId { get; set; }

        // 字典子项id
        public string DictItemId { get; set; }

        // 值字段
        public string ValueFiled { get; set; }

        // 显示字段
        public string TxtFiled { get; set; }

        // 排序字段
        public string OrderFiled { get; set; }

        // 排序方式？1正序2反序
        public string OrderType { get; set; }

        // 限定字符串
        public string LimitStr { get; set; }

        /// <summary>
        /// 是否显示查看 1显示2不显示
        /// </summary>
        public string showDetails { get; set; }

        // 是否可多选？1可以2不可以
        public string isMuch { get; set; }

        /// <summary>
        /// 查询字段
        /// </summary>
        public string RuleBy { get; set; }

        //0从系统数据库读取数据 非0从已定义的外部数据库读数
        public int DbID { get; set; }

        [Computed]
        public string DbID_Exa { get; set; }

        [Computed]
        public string Name { get; set; }

        [Computed]
        public string TbId_Exa { get; set; }

        [Computed]
        public string RuleBy_Exa { get; set; }
    }
}