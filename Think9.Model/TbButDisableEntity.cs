using DapperExtensions;

namespace Think9.Models
{
    [Table("tbbutdisable")]
    public class TbButDisableEntity
    {
        /// <summary>
        /// 主键 自增长
        /// </summary>
        [DapperExtensions.Key(true)]
        public int id { get; set; }

        /// <summary>
        /// 录入表id
        /// </summary>
        public string TbId { get; set; }

        public string GridId { get; set; }

        /// <summary>
        /// 按钮id
        /// </summary>
        public string BtnId { get; set; }

        /// <summary>
        /// 页面类别 list form detail
        /// </summary>
        public string PageType { get; set; }

        /// <summary>
        /// 对象类别 1组织机构 2角色 3用户 4岗位
        /// </summary>
        public string ObjType { get; set; }

        /// <summary>
        /// 对象编码
        /// </summary>
        public string ObjId { get; set; }

        /// <summary>
        /// 对象编码
        /// </summary>
        [Computed]
        public string ObjName { get; set; }

        public string Guid { get; set; }

        /// <summary>
        /// 选择
        /// </summary>
        [Computed]
        public string SelectId { get; set; }
    }
}