using DapperExtensions;

namespace Think9.Models
{
    [Table("tbhiddenindex")]
    public class TbHiddenIndexEntity
    {
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        /// <summary>
        /// 录入表id
        /// </summary>
        public string TbId { get; set; }

        /// <summary>
        /// 对象类别 1组织机构 2角色 3用户 4岗位
        /// </summary>
        public string ObjType { get; set; }

        /// <summary>
        /// 对象编码
        /// </summary>
        public string ObjId { get; set; }

        [Computed]
        public string ObjId_Exa { get; set; }

        public string IndexId { get; set; }

        [Computed]
        public string IndexId_Exa { get; set; }

        public string isHidden { get; set; }//是否保密？y是
    }
}