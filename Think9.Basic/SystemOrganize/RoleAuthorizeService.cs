using System;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class RoleAuthorizeService : BaseService<RoleAuthorizeEntity>
    {
        public dynamic GetListByFilter(RoleAuthorizeEntity filter, PageInfoEntity pageInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存菜单角色权限配置
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int SavePremission(IEnumerable<RoleAuthorizeEntity> entitys, int roleId)
        {
            ComService comser = new ComService();
            int result = 0;

            comser.ExecuteSql("delete from sys_roleauthorize where RoleId=" + roleId + "");
            foreach (RoleAuthorizeEntity obj in entitys)
            {
                base.Insert(obj);
                result = 1;
            }

            return result;
        }

        /// <summary>
        /// 根据角色菜单获取列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public IEnumerable<RoleAuthorizeEntity> GetListByRoleIdModuleId(int roleId, int moduleId)
        {
            string where = "where RoleId=@RoleId and ModuleId=@ModuleId";
            return GetByWhere(where, new { RoleId = roleId, ModuleId = moduleId });
        }
    }
}