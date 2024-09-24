namespace Think9.Models
{
    /// <summary>
    /// 用户登录使用的类
    /// </summary>
    public class CurrentUserEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadIcon { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string RoleNo { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptNo { get; set; }

        /// <summary>
        /// 上级部门编码
        /// </summary>
        public string UpDeptNo { get; set; }

        /// <summary>
        /// 是所有的上级单位(部门)编码通过;相连的字符
        /// </summary>
        public string DeptNoStr { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        public string LoginIPAddress { get; set; }

        /// <summary>
        /// 登录IP城市
        /// </summary>
        public string LoginIPAddressName { get; set; }
    }
}