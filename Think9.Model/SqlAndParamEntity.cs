using DapperExtensions;

namespace Think9.Models
{
    [Table("eventrecord")]
    public class SqlAndParamEntity
    {
        [Computed]
        public string Type { get; set; } = "1";

        [Computed]
        public string Sql { get; set; }

        [Computed]
        public object Param { get; set; }

        [DapperExtensions.Key(true)]
        public long ID { get; set; }

        public string Err { get; set; }

        public SqlAndParamEntity(string eventId, string btnId, string flowId, string fromTbId, string listid, string _isSys = "y")
        {
            EventId = string.IsNullOrEmpty(eventId) ? 0 : int.Parse(eventId);
            BtnId = btnId;
            FlowId = flowId;
            FromTbId = fromTbId;
            FromId = listid;
            isSys = _isSys;
        }

        public SqlAndParamEntity()
        {
        }

        /// <summary>
        /// RelationId
        /// </summary>
        public int EventId
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public string isSys { get; set; } = "y";

        /// <summary>
        ///
        /// </summary>
        public string BtnId
        {
            get; set;
        }

        /// <summary>
        /// 流程编码
        /// </summary>
        public string FlowId
        {
            get; set;
        }

        /// <summary>
        /// 流程步骤编码
        /// </summary>
        public string PrcsId
        {
            get; set;
        }

        /// <summary>
        /// 源表编码
        /// </summary>
        public string FromTbId
        {
            get; set;
        }

        /// <summary>
        /// 源表对应的listid
        /// </summary>
        public string FromId
        {
            get; set;
        }

        public string WriteTime
        {
            get; set;
        }

        public int Num
        {
            get; set;
        }

        public string strSql { get; set; }
    }
}