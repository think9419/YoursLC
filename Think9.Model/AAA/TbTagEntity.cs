using DapperExtensions;

namespace Think9.Models
{
    [Table("tbtag")]
    public class TbTagEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string Type { get; set; }
        public string TbId { get; set; }
        public string TagName { get; set; }
        public string FilterStr { get; set; }
        public int? OrderNo { get; set; }
    }
}