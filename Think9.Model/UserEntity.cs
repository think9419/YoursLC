using DapperExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Think9.Models
{
    [Table("sys_users")]
    public class UserEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DapperExtensions.Key(true)]
        public int Id { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadIcon { get; set; }

        /// <summary>
        /// 性别 1:男 0:女
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptNo { get; set; }

        /// <summary>
        /// 角色主键
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 有效标识 0：有效 1：无效
        /// 可空是用于查询
        /// </summary>
        public int? EnabledMark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Computed]
        public int SortCode { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [Computed]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Computed]
        public string RoleName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [Display(Name = "修改时间")]
        public DateTime UpdateTime { get; set; }
    }
}