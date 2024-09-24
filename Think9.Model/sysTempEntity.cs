using DapperExtensions;

namespace Think9.Models
{
    [Table("sys_temp")]
    public class SysTempEntity
    {
        [DapperExtensions.Key(true)]
        public int id { get; set; }

        public string Guid { get; set; }

        public string Info1 { get; set; }

        public string Info2 { get; set; }

        public string Info3 { get; set; }

        public string Info4 { get; set; }

        public int OrderNo { get; set; }
    }
}