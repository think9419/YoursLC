using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Repository;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class ModuleService : BaseService<ModuleEntity>
    {
        private ModuleRepository moduleRepository = new ModuleRepository();
        private RoleAuthorizeService roleAuthorizeService = new RoleAuthorizeService();

        //<summary>
        //得到id组成的字符
        //</summary>
        public string GetHigherUpsIDStr(DataTable dtAll, string sID)
        {
            string sReturn = ".";
            int iCount = 1;

            DataTable dtReturn = new DataTable("");
            dtReturn.Columns.Add("icount", typeof(int));
            dtReturn.Columns.Add("id", typeof(String));
            dtReturn.Columns.Add("name", typeof(String));
            dtReturn.DefaultView.Sort = "icount DESC";

            this.AddHigherNewRow(dtAll, dtReturn, sID, iCount);

            foreach (DataRow dr in dtReturn.DefaultView.Table.Rows)
            {
                sReturn += dr["id"].ToString() + ".";
            }

            return sReturn;
        }

        //<summary>
        //得到id组成的字符
        //</summary>
        public string GetLowerLevelIDStr(DataTable dtAll, string sID)
        {
            string sReturn = ".";
            int iCount = 1;

            DataTable dtReturn = new DataTable("");
            dtReturn.Columns.Add("icount", typeof(int));
            dtReturn.Columns.Add("id", typeof(String));
            dtReturn.Columns.Add("name", typeof(String));
            dtReturn.DefaultView.Sort = "icount DESC";

            this.AddHigherNewRow(dtAll, dtReturn, sID, iCount);

            foreach (DataRow dr in dtReturn.DefaultView.Table.Rows)
            {
                sReturn += dr["id"].ToString() + ".";
            }

            return sReturn;
        }

        /// <summary>
        /// 添加一个row
        /// </summary>
        private void AddHigherNewRow(DataTable dtAll, DataTable dtReturn, string id, int iCount)
        {
            foreach (DataRow dr in dtAll.Rows)
            {
                if (id == dr["id"].ToString().Trim())
                {
                    DataRow row = dtReturn.NewRow();
                    row["icount"] = iCount;
                    row["id"] = dr["id"].ToString().Trim();
                    row["name"] = dr["name"].ToString().Trim();
                    dtReturn.Rows.Add(row);
                    iCount++;

                    //未到最顶级
                    if (dr["ParentID"].ToString().Trim() != "0")
                    {
                        this.AddHigherNewRow(dtAll, dtReturn, dr["ParentID"].ToString().Trim(), iCount);
                    }
                }
            }
        }

        /// <summary>
        /// 得到下级字符
        /// </summary>
        public void GetLowerLevel(DataTable dtAll, string id, ref string lowerLevel)
        {
            foreach (DataRow dr in dtAll.Rows)
            {
                if (id == dr["ParentID"].ToString().Trim())
                {
                    lowerLevel += dr["id"].ToString() + ".";

                    GetLowerLevel(dtAll, dr["id"].ToString(), ref lowerLevel);
                }
            }
        }

        public IEnumerable<valueTextEntity> GetSelectTreeList()
        {
            ComService ComService = new ComService();
            DataTable dtAll = ComService.GetDataTable("select id,FullName as name,ParentId from sys_module order by OrderNo");
            DataTable list = this.GetModuleTree(dtAll);

            return DataTableHelp.ToEnumerable<valueTextEntity>(list);
        }

        /// <summary>
        ///
        /// </summary>
        public DataTable GetModuleTree(DataTable dtAll)
        {
            DataTable dtReturn = new DataTable("valueText");
            dtReturn.Columns.Add("ClassID", typeof(String));
            dtReturn.Columns.Add("Value", typeof(String));
            dtReturn.Columns.Add("Text", typeof(String));
            dtReturn.Columns.Add("Exa", typeof(String));

            DataRow _row = dtReturn.NewRow();
            _row["Value"] = "0";
            _row["Text"] = "根目录";
            _row["Exa"] = "";
            dtReturn.Rows.Add(_row);

            DataRow[] drs = dtAll.Select("ParentId=0 ");
            foreach (DataRow dr in drs)
            {
                DataRow row = dtReturn.NewRow();
                row["Value"] = dr["id"].ToString().Trim();
                row["Text"] = "╋" + "" + dr["name"].ToString().Trim();
                row["Exa"] = dr["ParentId"].ToString().Trim();
                dtReturn.Rows.Add(row);

                this.CreateModuleRow(dr["id"].ToString().Trim(), dtAll, dtReturn, "├『");
            }

            return dtReturn;
        }

        /// <summary>
        /// 添加一个row
        /// </summary>
        public void CreateModuleRow(string ID, DataTable dtAll, DataTable dtReturn, string strAdd)
        {
            strAdd = "---" + strAdd;
            DataRow[] drs = dtAll.Select("ParentId=" + ID + "");
            foreach (DataRow dr in drs)
            {
                DataRow row = dtReturn.NewRow();

                row["Value"] = dr["id"].ToString().Trim();
                row["Text"] = strAdd + dr["name"].ToString().Trim() + "』";
                row["Exa"] = dr["ParentId"].ToString().Trim();

                dtReturn.Rows.Add(row);

                this.CreateModuleRow(dr["id"].ToString(), dtAll, dtReturn, strAdd);
            }
        }

        public dynamic GetListByFilter(ModuleEntity filter, PageInfoEntity pageInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public dynamic GetModuleList(int roleId, string mobile)
        {
            bool isMobile = mobile == "y" ? true : false;
            string icon = "";
            string href = "";
            InitEntity init = new InitEntity();
            init.homeInfo = new HomeInfoEntity();
            init.logoInfo = new LogoInfoEntity();
            List<MenuInfoEntity> treeList = new List<MenuInfoEntity>();

            IEnumerable<ModuleEntity> allMenus = GetModuleListByRoleId(roleId);

            var rootMenus = allMenus.Where(x => x.ParentId == 0).OrderBy(x => x.OrderNo);
            foreach (var item in rootMenus)
            {
                icon = string.IsNullOrEmpty(item.Icon) ? "fa fa-circle-thin" : "fa " + item.Icon;
                if (string.IsNullOrEmpty(item.UrlAddress))
                {
                    href = "";
                }
                else
                {
                    int _id = MenuIdService.ChangeMenuId(item.Id);
                    //href = item.UrlAddress.Trim().Contains("?") ? item.UrlAddress + "&id=" + _id : item.UrlAddress + "?id=" + _id;

                    href = item.UrlAddress.Trim().Contains("?") ? item.UrlAddress + "&id=" + _id + "&mobile=" + isMobile.ToString() : item.UrlAddress + "?id=" + _id + "&mobile=" + isMobile.ToString();
                }

                var _tree = new MenuInfoEntity { id = item.Id, title = item.FullName, href = href, fontFamily = item.FontFamily, icon = icon, target = "_self" };
                GetModuleListByModuleId(treeList, allMenus, _tree, item.Id, mobile);
                treeList.Add(_tree);
            }

            init.menuInfo = treeList;

            return init;
        }

        /// <summary>
        /// 根据一级菜单加载子菜单列表
        /// </summary>
        /// <param name="treeList"></param>
        /// <param name="allMenus"></param>
        /// <param name="tree"></param>
        /// <param name="moduleId"></param>
        private void GetModuleListByModuleId(List<MenuInfoEntity> treeList, IEnumerable<ModuleEntity> allMenus, MenuInfoEntity tree, int moduleId, string mobile)
        {
            string icon = "";
            string href = "";
            var childMenus = allMenus.Where(x => x.ParentId == moduleId).OrderBy(x => x.SortCode);
            if (childMenus != null && childMenus.Count() > 0)
            {
                List<MenuInfoEntity> _children = new List<MenuInfoEntity>();
                foreach (var item in childMenus)
                {
                    icon = string.IsNullOrEmpty(item.Icon) ? "fa fa-genderless" : "fa " + item.Icon;
                    if (string.IsNullOrEmpty(item.UrlAddress))
                    {
                        href = "";
                    }
                    else
                    {
                        int _id = MenuIdService.ChangeMenuId(item.Id);
                        //href = item.UrlAddress.Trim().Contains("?") ? item.UrlAddress + "&id=" + _id : item.UrlAddress + "?id=" + _id;
                        href = item.UrlAddress.Trim().Contains("?") ? item.UrlAddress + "&id=" + _id + "&mobile=" + mobile : item.UrlAddress + "?id=" + _id + "&mobile=" + mobile;
                    }

                    var _tree = new MenuInfoEntity { id = item.Id, title = item.FullName, href = href, fontFamily = item.FontFamily, icon = icon, target = "_self" };
                    _children.Add(_tree);
                    tree.child = _children;

                    GetModuleListByModuleId(treeList, allMenus, _tree, item.Id, mobile);
                }
            }
        }

        /// <summary>
        /// 根据角色ID获取菜单列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private IEnumerable<ModuleEntity> GetModuleListByRoleId(int roleId)
        {
            string sql = @"SELECT DISTINCT b.* FROM sys_roleauthorize a INNER JOIN sys_module b ON a.ModuleId = b.Id WHERE a.RoleId = @RoleId ORDER BY b.OrderNo";
            var list = moduleRepository.GetModuleListByRoleId(sql, roleId);
            return list;
        }

        /// <summary>
        /// 递归遍历treeSelectList
        /// </summary>
        private void GetModuleChildren(List<TreeSelectEntity> treeSelectList, IEnumerable<ModuleEntity> moduleList, TreeSelectEntity tree, int id)
        {
            var childModuleList = moduleList.Where(x => x.ParentId == id).OrderBy(x => x.SortCode);
            if (childModuleList != null && childModuleList.Count() > 0)
            {
                List<TreeSelectEntity> _children = new List<TreeSelectEntity>();
                foreach (var item in childModuleList)
                {
                    TreeSelectEntity _tree = new TreeSelectEntity
                    {
                        id = item.Id.ToString(),
                        name = item.FullName,
                        open = false
                    };
                    _children.Add(_tree);
                    tree.children = _children;
                    GetModuleChildren(treeSelectList, moduleList, _tree, item.Id);
                }
            }
        }

        /// <summary>
        /// 获取所有菜单列表及可用按钮权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public IEnumerable<ModuleEntity> GetModuleButtonList(int roleId)
        {
            string returnFields = "Id,ParentId,FullName,Icon,SortCode";
            string orderby = "ORDER BY SortCode ASC";
            IEnumerable<ModuleEntity> list = GetAll(returnFields, orderby);//sys_module数据
            foreach (var item in list)
            {
                //item.ModuleButtonHtml = ButtonService.GetButtonListHtmlByRoleIdModuleId(roleId, item.Id);//sys_button数据转化为html
                item.IsChecked = roleAuthorizeService.GetListByRoleIdModuleId(roleId, item.Id).Count() > 0 ? true : false;//sys_roleauthorize是否存在 RoleId=@RoleId and ModuleId=@ModuleId
            }
            return list;
        }
    }
}