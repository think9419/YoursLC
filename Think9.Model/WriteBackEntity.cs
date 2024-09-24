using DapperExtensions;

namespace Think9.Models
{
    [Table("relationwriterecord")]
    public class WriteBackEntity
    {
        public WriteBackEntity(int relationId, string flowId, string rcsId, string fromTbId, string writeTbId, int writeNum, string listid)
        {
            RelationId = relationId;
            FlowId = flowId;
            PrcsId = rcsId;
            FromTbId = fromTbId;
            WriteTbId = writeTbId;
            WriteNum = writeNum;
            FromId = listid;
            WriteId = "some";
        }

        [DapperExtensions.Key(true)]
        public long ID { get; set; }

        public WriteBackEntity()
        {
        }

        public string strSql { get; set; }

        [Computed]
        public string Type { get; set; } = "1";

        [Computed]
        public string Sql { get; set; }

        [Computed]
        public object Param { get; set; }

        public string Err { get; set; }

        /// <summary>
        /// RelationId
        /// </summary>
        public int RelationId
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
        /// 目标表编码
        /// </summary>
        public string WriteTbId
        {
            get; set;
        }

        /// <summary>
        /// 源表对应的listid+id
        /// </summary>
        public string FromId
        {
            get; set;
        }

        /// <summary>
        /// 目标表对应的listid+id
        /// </summary>
        public string WriteId
        {
            get; set;
        }

        /// <summary>
        /// 目标表编码
        /// </summary>
        public string WriteTime
        {
            get; set;
        }

        public int Num
        {
            get; set;
        }

        /// <summary>
        /// 目标表对应的listid+id
        /// </summary>
        [Computed]
        public int WriteNum
        {
            get; set;
        }
    }
}