using System;

namespace Think9.Models
{
    /// <summary>
    /// ServerState Entity Model
    /// </summary>
    public class ServerStateEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string F_Id { get; set; }

        /// <summary>
        /// 网站站点
        /// </summary>
        public string F_WebSite { get; set; }

        /// <summary>
        /// ARM
        /// </summary>
        public string F_ARM { get; set; }

        /// <summary>
        /// CPU
        /// </summary>
        public string F_CPU { get; set; }

        /// <summary>
        /// IIS
        /// </summary>
        public string F_IIS { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime F_Date { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int F_Cout { get; set; }
    }
}