using System.Data;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class CommonSelectService
    {
        public static string GetNameStrByIdStr(string sID, string sort)
        {
            string name = "";
            string sql;
            string _ID = sID == null ? "" : sID;
            ComService comService = new ComService();
            //用户
            if (sort == "1")
            {
                if (_ID == "#all#")
                {
                    name = "所有用户";
                }
                else
                {
                    sql = "Select Account AS id ,RealName AS name  from sys_users";
                    foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                    {
                        if (_ID.Contains(";" + dr["id"].ToString() + ";"))
                        {
                            name += dr["name"].ToString() + " ";
                        }
                    }
                }
            }

            //部门
            if (sort == "2")
            {
                if (_ID == "#all#")
                {
                    name = "所有部门";
                }
                else
                {
                    sql = "Select EnCode AS id ,FullName AS name  from sys_organize";
                    foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                    {
                        if (_ID.Contains(";" + dr["id"].ToString() + ";"))
                        {
                            name += dr["name"].ToString() + " ";
                        }
                    }
                }
            }

            //角色
            if (sort == "3")
            {
                if (_ID == "#all#")
                {
                    name = "所有角色";
                }
                else
                {
                    sql = "Select EnCode AS id ,FullName AS name  from sys_role";
                    foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                    {
                        if (_ID.Contains(";" + dr["id"].ToString() + ";"))
                        {
                            name += dr["name"].ToString() + " ";
                        }
                    }
                }
            }

            //角色
            if (sort == "4")
            {
                if (_ID == "#all#")
                {
                    name = "所有";
                }
                else
                {
                    sql = "Select EnCode AS id ,FullName AS name  from sys_role";
                    foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                    {
                        if (_ID.Contains(";" + dr["id"].ToString() + ";"))
                        {
                            name += dr["name"].ToString() + " ";
                        }
                    }
                }
            }

            return name;
        }

        public static string GetIndexAndSonTbListByStr(string tbid, string sID)
        {
            string name = "";
            string sql;
            string _ID = sID == null ? "" : sID;
            string _tbid = tbid == null ? "" : tbid;
            ComService ComService = new ComService();

            if (_ID == "#all#")
            {
                name = "所有";
            }
            else
            {
                sql = "Select IndexId AS id ,IndexName AS name  from tbindex where TbId='" + _tbid + "' order by IndexNo";
                foreach (DataRow dr in ComService.GetDataTable(sql).Rows)
                {
                    if (_ID.Contains(";" + dr["id"].ToString() + ";"))
                    {
                        name += dr["name"].ToString() + " ";
                    }
                }

                sql = "Select TbId AS id ,TbName AS name  from tbbasic where ParentId='" + _tbid + "' and TbType = '2'";
                foreach (DataRow dr in ComService.GetDataTable(sql).Rows)
                {
                    if (_ID.Contains(";" + dr["id"].ToString() + ";"))
                    {
                        name += dr["name"].ToString() + " ";
                    }
                }
            }

            return name;
        }
    }
}