using System.Data;
using Think9.Models;

namespace Think9.Services.Base
{
    public class AuthorityService
    {
        /// <summary>
        /// 根据当前用户返回已禁用页面按钮的字符，用于按钮的权限控制
        /// </summary>
        /// <param name="pageType">页面类别，list或form</param>
        /// <param name="user">当前用户</param>
        /// <returns>[禁用的按钮id1][禁用的按钮id2]</returns>
        public static string GetDisableButStr(string tbid, string pageType, CurrentUserEntity user)
        {
            if (user == null)
            {
                return "all";
            }

            string disableButStr = "";
            ComService comService = new ComService();
            DataTable dt = comService.GetDataTable("select * from tbbutdisable where TbId='" + tbid + "' and PageType='" + pageType + "'");

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ObjType"].ToString() == "1" && dr["ObjId"].ToString() == user.DeptNo && !disableButStr.Contains("[" + dr["BtnId"].ToString() + "]"))
                {
                    disableButStr += "[" + dr["BtnId"].ToString() + "]";
                }

                if (dr["ObjType"].ToString() == "2" && dr["ObjId"].ToString() == user.RoleNo && !disableButStr.Contains("[" + dr["BtnId"].ToString() + "]"))
                {
                    disableButStr += "[" + dr["BtnId"].ToString() + "]";
                }

                if (dr["ObjType"].ToString() == "3" && dr["ObjId"].ToString() == user.Account && !disableButStr.Contains("[" + dr["BtnId"].ToString() + "]"))
                {
                    disableButStr += "[" + dr["BtnId"].ToString() + "]";
                }
            }

            return disableButStr;
        }
    }
}