using DapperExtensions;

namespace Think9.Models
{
    [Table("tbeventpara")]
    public class TbEventParaEntity
    {
        /// <summary>
        /// 主键 TbId
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public int EventId { get; set; }

        public int BtnId { get; set; }

        public string TbId { get; set; }

        public string IndexId { get; set; }

        public string ParaType { get; set; }

        public string ParaName { get; set; }
        public string ParaValue { get; set; }

        public string Guid { get; set; }

        public string Frm { get; set; }//1自定义事件 2自定义按钮处理
    }
}