using Dapper;
using System.Linq;
using Think9.Models;

namespace Think9.Repository
{
    public class UserRepository : BaseRepository<UserEntity>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserEntity LoginOn(string username, string password)
        {
            using (var conn = dbContext.GetConnection())
            {
                var sql = "Select * from sys_users where 1=1";
                if (!string.IsNullOrEmpty(username))
                {
                    sql += " and Account=@Account";
                }
                if (!string.IsNullOrEmpty(password))
                {
                    sql += " and UserPassWord=@UserPassWord";
                }
                return conn.Query<UserEntity>(sql, new { Account = username, UserPassWord = password }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int ModifyUserPwd(ModifyPwdEntity model, int userId)
        {
            using (var conn = dbContext.GetConnection())
            {
                var sql = "UPDATE sys_users SET UserPassword=@UserPassword WHERE Id=@Id AND Account=@Account AND UserPassword=@OldPassword";
                return conn.Execute(sql, new { UserPassword = model.Password, Id = userId, Account = model.UserName, OldPassword = model.OldPassword });
            }
        }
    }
}