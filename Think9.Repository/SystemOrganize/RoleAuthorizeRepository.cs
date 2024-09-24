using System;
using System.Collections.Generic;
using Think9.Models;
using System.Data;


namespace Think9.Repository
{
    public class RoleAuthorizeRepository : BaseRepository<RoleAuthorizeEntity>
    {
        ///// <summary>
        ///// 保存菜单角色权限配置
        ///// </summary>
        ///// <param name="entitys"></param>
        ///// <param name="roleId"></param>
        ///// <returns></returns>
        //public int SavePremission(IEnumerable<RoleAuthorizeEntity> entitys, int roleId)
        //{
        //    int result = 0;
        //    using (var conn = dbContext.GetConnection())
        //    {
        //        IDbTransaction transaction = conn.BeginTransaction();
        //        try
        //        {
        //            //先删除当前角色所有权限
        //          int icont =  conn.DeleteByWhere<RoleAuthorizeEntity>("where RoleId=@RoleId", new { RoleId = roleId }, transaction);
        //            if (entitys != null)
        //            {
        //                //批量插入权限
        //                conn.InsertBatch<RoleAuthorizeEntity>(entitys, transaction);
        //            }
        //            result = 1;
        //            transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            result = -1;
        //            transaction.Rollback();
        //        }
        //    }
        //    return result;
        //}
    }
}