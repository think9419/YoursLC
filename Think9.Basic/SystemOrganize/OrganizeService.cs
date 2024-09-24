using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Repository;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class OrganizeService : BaseService<OrganizeEntity>
    {
        private OrganizeRepository organizeRepository = new OrganizeRepository();
        private ComService comService = new ComService();

        public IEnumerable<valueTextEntity> GetSelectTreeList()
        {
            DataTable dtAll = comService.GetDataTable("select EnCode as id,FullName as name,ParentId from sys_organize order by OrderNo");
            DataTable list = this.GetDepTree(dtAll);

            return DataTableHelp.ToEnumerable<valueTextEntity>(list);
        }

        //<summary>
        //得到DeptNo组成的字符
        //</summary>
        public string GetDeptUpNOStr(string deptno)
        {
            string strUpNO = ";";
            List<OrganizeEntity> list = GetPreLevelList(deptno);
            foreach (var item in list)
            {
                strUpNO += item.EnCode + ";";
            }

            //最上级的部分编码已经设定为top且不能修改
            if (strUpNO == ";")
            {
                strUpNO = ";top;";
            }

            return strUpNO;
        }

        /// <summary>
        ///
        /// </summary>
        public DataTable GetDepTree(DataTable dtAll)
        {
            DataTable dtReturn = new DataTable("valueText");
            dtReturn.Columns.Add("ClassID", typeof(String));
            dtReturn.Columns.Add("Value", typeof(String));
            dtReturn.Columns.Add("Text", typeof(String));
            dtReturn.Columns.Add("Exa", typeof(String));

            DataRow[] drs = dtAll.Select("ParentId='' ");
            foreach (DataRow dr in drs)
            {
                DataRow row = dtReturn.NewRow();
                row["Value"] = dr["id"].ToString().Trim();
                row["Text"] = "╋" + dr["name"].ToString().Trim();
                //row["Exa"] = dr["ParentId"].ToString().Trim();
                dtReturn.Rows.Add(row);

                this.CreateDepartmentRow(dr["id"].ToString().Trim(), dtAll, dtReturn, "├『");
            }

            return dtReturn;
        }

        /// <summary>
        /// 添加一个row
        /// </summary>
        public void CreateDepartmentRow(string DeptNo, DataTable dtAll, DataTable dtReturn, string strAdd)
        {
            strAdd = "---" + strAdd;
            DataRow[] drs = dtAll.Select("ParentId='" + DeptNo + "' ");
            foreach (DataRow dr in drs)
            {
                DataRow row = dtReturn.NewRow();

                row["Value"] = dr["id"].ToString().Trim();
                row["Text"] = strAdd + dr["name"].ToString().Trim() + "』";
                //row["Exa"] = dr["ParentId"].ToString().Trim();

                dtReturn.Rows.Add(row);

                this.CreateDepartmentRow(dr["id"].ToString(), dtAll, dtReturn, strAdd);
            }
        }

        /// <summary>
        /// 得到上级列表
        /// </summary>
        public List<OrganizeEntity> GetPreLevelList(string sDeptNo)
        {
            int i = 0;
            IEnumerable<OrganizeEntity> allist = BaseRepository.GetAll("FullName,ParentId,EnCode", "ORDER BY OrderNo ASC");

            List<OrganizeEntity> Lower = new List<OrganizeEntity>();

            AddNewModel(allist, Lower, sDeptNo, i);

            return Lower;
        }

        private void AddNewModel(IEnumerable<OrganizeEntity> allist, List<OrganizeEntity> Lower, string strDeptNo, int iCount)
        {
            foreach (var item in allist.OrderBy(x => x.OrderNo))
            {
                if (strDeptNo == item.EnCode)
                {
                    OrganizeEntity model = new OrganizeEntity();
                    model.EnCode = item.EnCode;
                    model.FullName = item.FullName;

                    Lower.Add(model);

                    iCount++;

                    //未到最顶级
                    if (item.ParentId != "")
                    {
                        this.AddNewModel(allist, Lower, item.ParentId, iCount);
                    }
                }
            }
        }

        public dynamic GetListByFilter(OrganizeEntity filter, PageInfoEntity pageInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Organize 树形数据列表
        /// </summary>
        public IEnumerable<TreeSelectEntity> GetOrganizeTreeSelect()
        {
            IEnumerable<OrganizeEntity> organizeList = BaseRepository.GetAll("FullName,ParentId,EnCode", "ORDER BY OrderNo ASC");
            var rootOrganizeList = organizeList.Where(x => x.EnCode == "top").OrderBy(x => x.OrderNo);
            List<TreeSelectEntity> treeSelectList = new List<TreeSelectEntity>();
            foreach (var item in rootOrganizeList)
            {
                TreeSelectEntity tree = new TreeSelectEntity
                {
                    id = item.EnCode,
                    name = item.FullName,
                    open = true
                };
                GetOrganizeChildren(treeSelectList, organizeList, tree, item.EnCode);
                treeSelectList.Add(tree);
            }
            return treeSelectList;
        }

        /// <summary>
        /// 递归遍历treeSelectList
        /// </summary>
        private void GetOrganizeChildren(List<TreeSelectEntity> treeSelectList, IEnumerable<OrganizeEntity> organizeList, TreeSelectEntity tree, string deptno)
        {
            var childOrganizeList = organizeList.Where(x => x.ParentId == deptno).OrderBy(x => x.OrderNo);
            if (childOrganizeList != null && childOrganizeList.Count() > 0)
            {
                List<TreeSelectEntity> _children = new List<TreeSelectEntity>();
                foreach (var item in childOrganizeList)
                {
                    TreeSelectEntity _tree = new TreeSelectEntity
                    {
                        id = item.EnCode,
                        //code = item.EnCode,
                        name = item.FullName,
                        open = true
                    };
                    _children.Add(_tree);
                    tree.children = _children;
                    GetOrganizeChildren(treeSelectList, organizeList, _tree, item.EnCode);
                }
            }
        }
    }
}