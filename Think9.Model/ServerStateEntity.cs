using System;

namespace Think9.Models
{
    /// <summary>
    /// ServerState Entity Model
    /// </summary>
    public class ServerStateEntity
    {
        /// <summary>
        /// ����Id
        /// </summary>
        public string F_Id { get; set; }

        /// <summary>
        /// ��վվ��
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
        /// ����
        /// </summary>
        public DateTime F_Date { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int F_Cout { get; set; }
    }
}