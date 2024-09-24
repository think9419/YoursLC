using DapperExtensions;

namespace Think9.Models
{
    [Table("sys_module")]
    public class ModuleEntity : Entity
    {
        /// <summary>
        /// 父级
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 字体类型 layui-icon|com-icon|my-icon
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string UrlAddress { get; set; }

        /// <summary>
        /// 菜单按钮复选框Html
        /// </summary>
        [Computed]
        public string ModuleButtonHtml { get; set; }

        /// <summary>
        /// 菜单是否选中 role
        /// </summary>
        [Computed]
        public bool IsChecked { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Computed]
        public string RoleStr { get; set; }
    }
}