using Dapper;
using System.Collections.Generic;
using Think9.Models;

namespace Think9.Repository
{
    public class ModuleRepository : BaseRepository<ModuleEntity>
    {
        /// <summary>
        /// 根据角色ID获取菜单列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<ModuleEntity> GetModuleListByRoleId(string sql, int roleId)
        {
            using (var conn = dbContext.GetConnection())
            {
                return conn.Query<ModuleEntity>(sql, new { RoleId = roleId });
            }
        }
    }
}