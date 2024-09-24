using Dapper;
using System.Collections.Generic;
using System.Text;
using Think9.Models;

namespace Think9.Repository
{
    public class ButtonRepository : BaseRepository<ButtonEntity>//, IButtonRepository
    {
        /// <summary>
        /// 根据角色菜单按钮位置获取按钮列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="moduleId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerable<ButtonEntity> GetButtonListByRoleIdModuleId(int roleId, int moduleId, PositionEnum position)
        {
            using (var conn = dbContext.GetConnection())
            {
                string sql = @"SELECT b.* FROM sys_roleauthorize a
                            INNER JOIN sys_button b ON a.ButtonId=b.Id
                            WHERE a.RoleId=@RoleId
                            and a.ModuleId=@ModuleId
                            and b.Location=@Location
                            ORDER BY b.SortCode";
                return conn.Query<ButtonEntity>(sql, new { RoleId = roleId, ModuleId = moduleId, Location = (int)position });
            }
        }

        /// <summary>
        /// 根据角色菜单获取按钮列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="moduleId"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public IEnumerable<ButtonEntity> GetButtonListByRoleIdModuleId(int roleId, int moduleId, out IEnumerable<ButtonEntity> selectList)
        {
            using (var conn = dbContext.GetConnection())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SELECT Id,FullName FROM sys_button a
                            INNER JOIN sys_roleauthorize b ON a.Id = b.ButtonId
                            WHERE b.RoleId = @RoleId and b.ModuleId = @ModuleId;");
                sb.AppendLine(@"SELECT Id, FullName FROM sys_button");
                using (var reader = conn.QueryMultiple(sb.ToString(), new { RoleId = roleId, ModuleId = moduleId }))
                {
                    selectList = reader.Read<ButtonEntity>();
                    return reader.Read<ButtonEntity>();
                }
            }
        }
    }
}