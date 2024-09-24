using DapperExtensions;

namespace Think9.Models
{
    [Table("flowprcsnext")]
    public class FlowPrcsNextEntity
    {
        /// <summary>
        ///
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id
        {
            get; set;
        }

        /// <summary>
        /// 本步骤--对应flowprcs中的PrcsId
        /// </summary>
        public int PrcsId
        {
            get; set;
        }

        /// <summary>
        /// 可选择的下一步骤
        /// </summary>
        public int NextPrcsId
        {
            get; set;
        }

        /// <summary>
        /// 可选择的下一步骤
        /// </summary>
        [Computed]
        public string NextPrcsName
        {
            get; set;
        }

        /// <summary>
        /// 转入条件
        /// </summary>
        public string NextPrcsInCase
        {
            get; set;
        }

        /// <summary>
        /// 转入条件
        /// </summary>
        public string NextPrcsInCaseShow
        {
            get; set;
        }

        /// <summary>
        /// 授权范围-人员--可进一步指定此时指定的优先级高授权范围-人员--可进一步指定人员
        /// </summary>
        public string NextPrcsUser
        {
            get; set;
        }

        public int NextOrder
        {
            get; set;
        }
    }
}