using System;

namespace Think9.Models
{
    public class UserParmEntity
    {
        /// <summary>
        /// 当前用户真实姓名
        /// </summary>
        public string currentUserName { get; set; }

        /// <summary>
        /// 当前用户登录名
        /// </summary>
        public string currentUserId { get; set; }

        /// <summary>
        /// 当前用户单位(部门)编码
        /// </summary>
        public string currentDeptNo { get; set; }

        /// <summary>
        /// 当前用户单位(部门)名称
        /// </summary>
        public string currentDeptName { get; set; }

        /// <summary>
        /// 当前用户角色编码
        /// </summary>
        public string currentRoleNo { get; set; }

        /// <summary>
        /// 当前用户角色名称
        /// </summary>
        public string currentRoleName { get; set; }

        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime timeToday { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime timeNow { get; set; }
    }
}