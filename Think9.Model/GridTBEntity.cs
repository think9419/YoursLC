using DapperExtensions;

namespace Think9.Models
{
    public class GridTBEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public long Id { get; set; }

        public long ListId { get; set; }

        [Computed]
        public string TbId { get; set; }

        /// <summary>
        /// 标志#tbid#+#id#+#行号#
        /// </summary>
        [Computed]
        public string flag { get; set; }
    }
}