namespace Think9.Models
{
    public class ReportCellsEntity
    {
        public string ReportId { get; set; }

        public int ColNum { get; set; }

        public int RowId { get; set; }

        /// <summary>
        /// 1字符 2指标或者参数
        /// </summary>
        public string IndexType { get; set; }

        public string IndexId { get; set; }

        public string IndexName { get; set; }

        /// <summary>
        /// 左边框 1显示 2不显示
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// 右边框 1显示 2不显示
        /// </summary>
        public string Right { get; set; }

        /// <summary>
        /// 上边框 1显示 2不显示
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// 下边框 1显示 2不显示
        /// </summary>
        public string Bottom { get; set; }

        /// <summary>
        /// 水平排列 1中2左3右
        /// </summary>
        public string HorizontalAlign { get; set; }

        /// <summary>
        /// 垂直排列 1中2上3下
        /// </summary>
        public string VerticalAlign { get; set; }

        /// <summary>
        /// 文本方向 1水平2垂直
        /// </summary>
        public string WriteDirection { get; set; }
    }
}