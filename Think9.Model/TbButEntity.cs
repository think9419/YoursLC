using DapperExtensions;

namespace Think9.Models
{
    [Table("tbbut")]
    public class TbButEntity
    {
        public string TbId { get; set; }
        public string BtnId { get; set; }
        public string BtnText { get; set; }
        public string BtnWarn { get; set; }
        public string BtnExa { get; set; }

        [Computed]
        public string FlowId { get; set; }
    }
}