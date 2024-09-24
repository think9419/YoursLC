using DapperExtensions;

namespace Think9.Models
{
    [Table("flowattachment")]
    public class AttachmentEntity
    {
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        public int ListId { get; set; }

        public string FwId { get; set; }

        public int PrcsId { get; set; }

        public string attachmentId { get; set; }

        public string FullName { get; set; }

        public string UserId { get; set; }

        public string createTime { get; set; }

        public string DocType { get; set; }

        public string DocState { get; set; }

        public string TotalSize { get; set; }

        public string AttachmentSort { get; set; }//

        public int DownloadNumber { get; set; }

        /// <summary>
        /// 公共附件选项?新建 1有权限2无
        /// </summary>
        [Computed]
        public string A1
        {
            get; set;
        }

        /// <summary>
        /// 公共附件选项?下载 1有权限2无
        /// </summary>
        [Computed]
        public string A2
        {
            get; set;
        }

        /// <summary>
        /// 公共附件选项?删除 1有权限2无 src
        /// </summary>
        [Computed]
        public string A3
        {
            get; set;
        }

        [Computed]
        public string Src
        {
            get; set;
        }
    }
}