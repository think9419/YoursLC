using DapperExtensions;

namespace Think9.Models
{
    [Table("tbvaluecheck")]
    public class TbValueCheckEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int ID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string TbId { get; set; }

        [Computed]
        public string TbNmae { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LeftValue { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Compare { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string RightValue { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Explain { get; set; }

        /// <summary>
        ///空值处理？ 0输入为空的数字视为0||输入为空的日期视为9999-12-31 1计算式存在输入为空的数字或日期，则不做校验
        /// </summary>
        public string NullCase { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int IOrder { get; set; }

        /// <summary>
        /// 1启用 0禁用
        /// </summary>
        public string isUse { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FlowStr { get; set; }

        [Computed]
        public string FlowStr_Exa { get; set; }
    }
}