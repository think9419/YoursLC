namespace Think9.Models
{
    public class FileEntity
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 原始文件名称
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 展示地址
        /// </summary>
        public string DisplayUrl { get; set; }
    }
}