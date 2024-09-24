namespace Think9.Models
{
    public class PageListEntity
    {
        public int listid { get; set; }

        public string ruNumber { get; set; }

        public string FlowId { get; set; }

        public string isLock { get; set; }
        public string runName { get; set; }
        public string createUser { get; set; }
        public string isFinish { get; set; }
        public string relatedId { get; set; }
        public string flowType { get; set; }
        public string currentPrcsId { get; set; }
        public string createTime { get; set; }
        public string currentPrcsName { get; set; }

        /// <summary>
        /// 流程步骤标志位--1待接手2办理中3已办结
        /// </summary>
        public string runFlag { get; set; }

        public string createDept { get; set; }
        public string createDeptStr { get; set; }
        public string attachmentId { get; set; }
    }
}