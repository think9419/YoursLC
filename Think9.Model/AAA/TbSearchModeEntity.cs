using DapperExtensions;

namespace Think9.Models
{
    [Table("tbsearchmode")]
    public class TbSearchModeEntity
    {
        public string TypeId { get; set; }
        public string TbId { get; set; }
        public string UserId { get; set; }
    }
}