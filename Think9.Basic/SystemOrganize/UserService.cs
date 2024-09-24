using Think9.Models;
using Think9.Repository;
using Think9.Services.Base;
using Think9.Util.Helper;

namespace Think9.Services.Basic
{
    public class UserService : BaseService<UserEntity>
    {
        public UserRepository UserRepository = new UserRepository();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserEntity LoginOn(string username, string password)
        {
            return UserRepository.LoginOn(username, password);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int ModifyUserPwd(ModifyPwdEntity model, int userId)
        {
            model.OldPassword = Md5.md5(model.OldPassword, 32);
            model.Password = Md5.md5(model.Password, 32);
            return UserRepository.ModifyUserPwd(model, userId);
        }
    }
}