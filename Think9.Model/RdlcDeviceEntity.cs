using System.Data;

namespace Think9.Models
{
    public class RdlcDeviceEntity
    {
        public decimal? Width { get; set; }
        public decimal? Heigh { get; set; }
        public decimal? Top { get; set; }
        public decimal? Left { get; set; }
        public decimal? Right { get; set; }
        public decimal? Bottom { get; set; }

        /// <summary>
        ///统计表或录入表的编码
        /// </summary>
        public string TbId { get; set; }

        /// <summary>
        ///统计表或录入表的名称
        /// </summary>
        public string TbName { get; set; }

        /// <summary>
        ///录入表对应的流程编码
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        ///录入表对应的LISTID
        /// </summary>
        public string ListId { get; set; }

        /// <summary>
        ///rdlc模板文件
        /// </summary>
        public string PathRdlc { get; set; }

        /// <summary>
        ///用户图片所在文件夹
        /// </summary>
        public string PathUserImg { get; set; }

        /// <summary>
        ///图片不存在时的替代 RdlcDeviceEntity
        /// </summary>
        public string ImgNoExist { get; set; }

        public DataTable MainDt { get; set; }

        public DataTable GridDt { get; set; }

        public string Err { get; set; }

        public string HostUrl { get; set; }
    }
}