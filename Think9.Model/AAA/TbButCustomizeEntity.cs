using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("tbbutcustomize")]
    public class TbButCustomizeEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public string PageType { get; set; }
        public string TbId { get; set; }
        public string GridId { get; set; }
        public string Icon { get; set; }
        public string BtnId { get; set; }
        public string BtnText { get; set; }
        public string BtnWarn { get; set; }
        public string BtnExa { get; set; }
        public string IsBatch { get; set; }//1单处理2批处理
        public string Remarks { get; set; }
        public DateTime UpdateTime { get; set; }

        public string IsLink { get; set; }//1打开链接 2后台处理
        public string LinkFlag { get; set; }//录入表或者统计表id，注意录入表只能是主表并且是非辅助表
        public string LinkFlag2 { get; set; }//录入表时需要设置，分别为add edit list detail 统计表只能是list
    }
}