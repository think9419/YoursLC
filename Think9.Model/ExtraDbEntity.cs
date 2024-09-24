using DapperExtensions;

namespace Think9.Models
{
    [Table("externaldb")]
    public class ExtraDbEntity
    {
        [DapperExtensions.Key(true)]
        public int DbID { get; set; }

        public string DbType { get; set; }
        public string DbName { get; set; }

        public string DbCon { get; set; }

        public string Remarks { get; set; }
    }
}