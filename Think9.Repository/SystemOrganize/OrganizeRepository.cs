using Dapper;
using System.Collections.Generic;
using Think9.Models;

namespace Think9.Repository
{
    public class OrganizeRepository : BaseRepository<OrganizeEntity>
    {
        public IEnumerable<OrganizeEntity> GetOrganizeList()
        {
            using (var conn = dbContext.GetConnection())
            {
                string sql = @"SELECT * FROM sys_organize ORDER BY OrderNo ";
                return conn.Query<OrganizeEntity>(sql);
            }
        }
    }
}