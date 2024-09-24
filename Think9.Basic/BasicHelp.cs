using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class BasicHelp
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="dsindex"></param>
        /// <param name="maxNumber">最大列数</param>
        /// <param name="number">第几行</param>
        /// <param name="from">add或edit用于控制编辑时锁定</param>
        /// <returns></returns>
        public static string GetDisabledStrForGridColumn(DataTable dsindex, int maxNumber, int number, string from)
        {
            string strLock = "";
            string isLock = "";
            string numLocked = "";
            string indexid = "";
            for (int i = 1; i <= maxNumber; i++)
            {
                isLock = "";
                indexid = "v" + i.ToString();
                foreach (DataRow dr in dsindex.Rows)
                {
                    if (dr["IndexId"].ToString() == indexid)
                    {
                        isLock = dr["isLock"].ToString();//是否锁定？1是2否3按行锁定
                        numLocked = " " + dr["isLock2"].ToString() + " ";//锁定的行号 以空格间隔
                        break;
                    }
                }

                //锁定
                if (isLock == "1")
                {
                    strLock += "[" + indexid + "]";
                }
                //按行锁定
                if (isLock == "3" && numLocked.Contains(" " + number.ToString() + " "))
                {
                    strLock += "[" + indexid + "]";
                }
                //编辑时锁定
                if (isLock == "9" && from != "add")
                {
                    strLock += "[" + indexid + "]";
                }
            }

            return strLock;
        }

        public static string GetGridIdByIndexid(string indexid)
        {
            string gridId = "";
            //indexid最多三位数，即v1-v99，以下逻辑没有问题
            if (indexid.EndsWith("v1") || indexid.EndsWith("v2") || indexid.EndsWith("v3") || indexid.EndsWith("v4") || indexid.EndsWith("v5") || indexid.EndsWith("v6") || indexid.EndsWith("v7") || indexid.EndsWith("v8") || indexid.EndsWith("v9"))
            {
                gridId = "tb_" + indexid.Substring(0, indexid.Length - 2);
                if (indexid.StartsWith("tb_"))
                {
                    gridId = indexid.Substring(0, indexid.Length - 2);
                }
            }
            else
            {
                gridId = "tb_" + indexid.Substring(0, indexid.Length - 3);
                if (indexid.StartsWith("tb_"))
                {
                    gridId = indexid.Substring(0, indexid.Length - 3);
                }
            }

            return gridId;
        }

        /// <summary>
        /// 获取分割后字符
        /// </summary>
        /// <returns></returns>
        public static string GetSplitStrByChar(string value, string _char, int num)
        {
            string str = value;
            if (value.Contains(_char))
            {
                string[] arr = BaseUtil.GetStrArray(value, _char);//.分割
                if (arr[num] != null)
                {
                    str = arr[num].ToString().Trim();
                }
            }
            return str;
        }

        public static string GetWhereByUserAndFlowId(CurrentUserEntity user, string flowid)
        {
            string tbid = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string where = "where state=1 ";
            ComService comService = new ComService();

            string userid = user == null ? ";!NullEx;" : user.Account;
            string deptno = user == null ? ";!NullEx;" : user.DeptNo;

            /// 查看编辑模式 可在录入表管理-权限管理中设置
            ///11查看编辑用户本人新建的数据
            ///22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
            ///21查看编辑用户所属单位(部门)新建的数据
            ///32查看用户所属下级部门新建的数据  编辑本人新建的数据
            ///31查看编辑用户所属下级部门新建的数据
            ///42查看所有数据  编辑本人新建的数据
            ///41查看编辑所有数据
            //自定义模式？
            string searchmode = comService.GetSingleField("select TypeId from tbsearchmode where TbId='" + flowid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' and  UserId='" + userid + "'");
            if (string.IsNullOrEmpty(searchmode))
            {
                searchmode = comService.GetSingleField("select SearchMode from flow where flowid='" + flowid + "'");
            }
            searchmode = string.IsNullOrEmpty(searchmode) ? "11" : searchmode;

            //基础信息表
            if (flowid.StartsWith("bi_"))
            {
                where = "WHERE (createUser = '" + userid + "') AND (state=1)";
                switch (searchmode.Substring(0, 1))
                {
                    case "1"://查看编辑用户本人新建的数据
                        where = "WHERE (createUser = '" + userid + "') AND (state=1)";
                        break;

                    case "2"://查看用户所属单位(部门)新建的数据
                        where = "WHERE (createDept = '" + deptno + "') AND (state=1)";
                        break;

                    case "3"://查看用户所属下级部门新建的数据
                        string str = ";" + deptno + ";";
                        where = "WHERE (createDeptStr  like'%" + str + "%') AND (state=1)";
                        break;

                    case "4"://查看所有数据
                        where = "WHERE (state=1)";
                        break;

                    default:
                        where = "WHERE (createUser = '" + userid + "') AND (state=1)";
                        break;
                }
            }
            else
            {
                where = "WHERE (flowrunlist.createUser = '" + userid + "') AND (" + tbid + ".state=1)";
                switch (searchmode.Substring(0, 1))
                {
                    case "1"://查看编辑用户本人新建的数据
                        where = "WHERE (flowrunlist.createUser = '" + userid + "') AND (" + tbid + ".state=1)";
                        break;

                    case "2"://查看用户所属单位(部门)新建的数据
                        where = "WHERE (flowrunlist.createDept = '" + deptno + "') AND (" + tbid + ".state=1)";
                        break;

                    case "3"://查看用户所属下级部门新建的数据
                        string str = ";" + deptno + ";";
                        where = "WHERE (flowrunlist.createDeptStr  like'%" + str + "%') AND (" + tbid + ".state=1)";
                        break;

                    case "4"://查看所有数据
                        where = "WHERE (" + tbid + ".state=1)";
                        break;

                    default:
                        where = "WHERE (flowrunlist.createUser = '" + userid + "') AND (" + tbid + ".state=1)";
                        break;
                }
            }

            return where;
        }

        public static string GetSearchModeName(string flowid, string userid)
        {
            string some = "";
            ComService comService = new ComService();

            //自定义模式没？
            string searchmode = comService.GetSingleField("select TypeId from tbsearchmode where TbId='" + flowid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' and  UserId='" + userid + "'");
            if (string.IsNullOrEmpty(searchmode))
            {
                searchmode = comService.GetSingleField("select SearchMode from flow where flowid='" + flowid + "'");
            }
            searchmode = string.IsNullOrEmpty(searchmode) ? "11" : searchmode;

            switch (searchmode)
            {
                case "11":
                    some = "【可查看本人新建的数据 || 可编辑删除本人新建的数据】";
                    break;

                case "22":
                    some = "【可查看所属单位(部门)用户新建的数据 || 可编辑删除本人新建的数据】";
                    break;

                case "21":
                    some = "【可查看所属单位(部门)用户新建的数据 || 可编辑删除所属单位(部门)用户新建的数据】";
                    break;

                case "32":
                    some = "【可查看下级单位(部门)用户新建的数据 || 可编辑删除本人新建的数据】";
                    break;

                case "31":
                    some = "【可查看下级单位(部门)用户新建的数据 || 可编辑删除下级单位(部门)用户新建的数据】";
                    break;

                case "42":
                    some = "【可查看所有数据 || 可编辑删除本人新建的数据】";
                    break;

                case "41":
                    some = "【可查看所有数据 || 可编辑删除所有数据】";
                    break;

                default:
                    some = "";
                    break;
            }

            return some;
        }

        public static string GetSearchMode(string searchmode, string tbid, string userid)
        {
            ComService comService = new ComService();

            //自定义模式没？
            string _searchmode = comService.GetSingleField("select TypeId from tbsearchmode where TbId='" + tbid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' and  UserId='" + userid + "'");
            if (string.IsNullOrEmpty(_searchmode))
            {
                _searchmode = searchmode;
            }
            _searchmode = string.IsNullOrEmpty(_searchmode) ? "11" : _searchmode;

            return _searchmode;
        }

        /// <summary>
        ///
        /// </summary>
        public static object GetParamObject(CurrentUserEntity user)
        {
            if (user == null)
            {
                return new { currentUserName = "!NullEx", currentUserId = "!NullEx", currentDeptNo = "!NullEx", currentDeptName = "!NullEx", currentRoleNo = "!NullEx", currentRoleName = "!NullEx", timeToday = DateTime.Now.ToShortDateString(), timeNow = DateTime.Now };
            }
            else
            {
                return new { currentUserName = user.RealName, currentUserId = user.Account, currentDeptNo = user.DeptNo, currentDeptName = user.DeptName, currentRoleNo = user.RoleNo, currentRoleName = user.RoleName, timeToday = DateTime.Now.ToShortDateString(), timeNow = DateTime.Now };
            }
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <returns></returns>
        public static string GetValueFrmList(IEnumerable<valueTextEntity> list, string classid, string value)
        {
            string str = "";
            foreach (valueTextEntity item in list)
            {
                if (item.ClassID == classid && item.Value == value)
                {
                    str = item.Text == null ? "" : item.Text;
                    break;
                }
            }
            return str;
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <returns></returns>
        public static string GetValueFrmList(IEnumerable<ControlEntity> list, string controlid)
        {
            string str = "";
            foreach (ControlEntity item in list)
            {
                if (item.ControlID == controlid)
                {
                    str = item.ControlValue == null ? "" : item.ControlValue;
                    break;
                }
            }
            return str;
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <returns></returns>
        public static int? GetIntValueFrmList(IEnumerable<ControlEntity> list, string controlid)
        {
            string str = "";
            foreach (ControlEntity item in list)
            {
                if (item.ControlID == controlid)
                {
                    str = item.ControlValue == null ? "" : item.ControlValue;
                    break;
                }
            }

            return ExtConvert.ToIntOrNull(str);
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimalValueFrmList(IEnumerable<ControlEntity> list, string controlid)
        {
            string str = "";
            foreach (ControlEntity item in list)
            {
                if (item.ControlID == controlid)
                {
                    str = item.ControlValue == null ? "" : item.ControlValue;
                    break;
                }
            }

            return ExtConvert.ToDecimalOrNull(str);
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetDateValueFrmList(IEnumerable<ControlEntity> list, string controlid)
        {
            string str = "";
            foreach (ControlEntity item in list)
            {
                if (item.ControlID == controlid)
                {
                    str = item.ControlValue == null ? "" : item.ControlValue;
                    break;
                }
            }

            return ExtConvert.ToDateOrNull(str);
        }

        public static void GetRowAndIdByFlag(string flag, ref string id, ref string row)
        {
            string[] arr = BaseUtil.GetStrArray(flag, "#");

            id = arr[1].ToString();
            if (arr[2].ToString() == "0")
            {
                row = " 当前行";
            }
            else
            {
                row = " 第" + arr[2].ToString() + "行";
            }
        }

        public static int GetRowByFlag(string flag)
        {
            int row = 0;
            if (!string.IsNullOrEmpty(flag))
            {
                string[] arr = BaseUtil.GetStrArray(flag, "#");
                if (arr[2] != null)
                {
                    row = int.Parse(arr[2].ToString());
                }
            }
            return row;
        }

        public static void GetTbAndIdByFlag(string flag, ref string tbid, ref string id)
        {
            string[] arr = BaseUtil.GetStrArray(flag, "#");

            id = arr[1].ToString();
            tbid = arr[0].ToString();
        }
    }
}

///// <summary>
///// 取值
///// </summary>
///// <returns></returns>
//public static string GetValueFrmList(IEnumerable<GridListEntity> list, string controlid, string dateType = "")
//{
//    string str = "";
//    if (list == null)
//    {
//        return str;
//    }
//    foreach (GridListEntity item in list)
//    {
//        switch (controlid)
//        {
//            case "v1":
//                str = item.v1;
//                break;

//            case "v2":
//                str = item.v2;
//                break;

//            case "v3":
//                str = item.v3;
//                break;

//            case "v4":
//                str = item.v4;
//                break;

//            case "v5":
//                str = item.v5;
//                break;

//            case "v6":
//                str = item.v6;
//                break;

//            case "v7":
//                str = item.v7;
//                break;

//            case "v8":
//                str = item.v8;
//                break;

//            case "v9":
//                str = item.v9;
//                break;

//            case "v10":
//                str = item.v10;
//                break;

//            case "v11":
//                str = item.v11;
//                break;

//            case "v12":
//                str = item.v12;
//                break;

//            case "v13":
//                str = item.v13;
//                break;

//            case "v14":
//                str = item.v14;
//                break;

//            case "v15":
//                str = item.v15;
//                break;

//            case "v16":
//                str = item.v16;
//                break;

//            case "v17":
//                str = item.v17;
//                break;

//            case "v18":
//                str = item.v18;
//                break;

//            case "v19":
//                str = item.v19;
//                break;

//            case "v20":
//                str = item.v20;
//                break;

//            case "v21":
//                str = item.v21;
//                break;

//            case "v22":
//                str = item.v22;
//                break;

//            case "v23":
//                str = item.v23;
//                break;

//            case "v24":
//                str = item.v24;
//                break;

//            case "v25":
//                str = item.v25;
//                break;

//            case "v26":
//                str = item.v26;
//                break;

//            case "v27":
//                str = item.v27;
//                break;

//            case "v28":
//                str = item.v28;
//                break;

//            case "v29":
//                str = item.v29;
//                break;

//            case "v30":
//                str = item.v30;
//                break;
//        }
//    }

//    if (str != "" && dateType != "")
//    {
//        if (dateType == "1")
//        {
//        }
//    }
//    return str;
//}

//public static int? GetIntValueFrmList(IEnumerable<GridListEntity> list, string controlid)
//{
//    string str = "";
//    if (list == null)
//    {
//        return null;
//    }
//    foreach (GridListEntity item in list)
//    {
//        switch (controlid)
//        {
//            case "v1":
//                str = item.v1;
//                break;

//            case "v2":
//                str = item.v2;
//                break;

//            case "v3":
//                str = item.v3;
//                break;

//            case "v4":
//                str = item.v4;
//                break;

//            case "v5":
//                str = item.v5;
//                break;

//            case "v6":
//                str = item.v6;
//                break;

//            case "v7":
//                str = item.v7;
//                break;

//            case "v8":
//                str = item.v8;
//                break;

//            case "v9":
//                str = item.v9;
//                break;

//            case "v10":
//                str = item.v10;
//                break;

//            case "v11":
//                str = item.v11;
//                break;

//            case "v12":
//                str = item.v12;
//                break;

//            case "v13":
//                str = item.v13;
//                break;

//            case "v14":
//                str = item.v14;
//                break;

//            case "v15":
//                str = item.v15;
//                break;

//            case "v16":
//                str = item.v16;
//                break;

//            case "v17":
//                str = item.v17;
//                break;

//            case "v18":
//                str = item.v18;
//                break;

//            case "v19":
//                str = item.v19;
//                break;

//            case "v20":
//                str = item.v20;
//                break;

//            case "v21":
//                str = item.v21;
//                break;

//            case "v22":
//                str = item.v22;
//                break;

//            case "v23":
//                str = item.v23;
//                break;

//            case "v24":
//                str = item.v24;
//                break;

//            case "v25":
//                str = item.v25;
//                break;

//            case "v26":
//                str = item.v26;
//                break;

//            case "v27":
//                str = item.v27;
//                break;

//            case "v28":
//                str = item.v28;
//                break;

//            case "v29":
//                str = item.v29;
//                break;

//            case "v30":
//                str = item.v30;
//                break;
//        }
//    }

//    return ExtConvert.ToIntOrNull(str);
//}

//public static decimal? GetDecimalValueFrmList(IEnumerable<GridListEntity> list, string controlid)
//{
//    string str = "";
//    if (list == null)
//    {
//        return null;
//    }
//    foreach (GridListEntity item in list)
//    {
//        switch (controlid)
//        {
//            case "v1":
//                str = item.v1;
//                break;

//            case "v2":
//                str = item.v2;
//                break;

//            case "v3":
//                str = item.v3;
//                break;

//            case "v4":
//                str = item.v4;
//                break;

//            case "v5":
//                str = item.v5;
//                break;

//            case "v6":
//                str = item.v6;
//                break;

//            case "v7":
//                str = item.v7;
//                break;

//            case "v8":
//                str = item.v8;
//                break;

//            case "v9":
//                str = item.v9;
//                break;

//            case "v10":
//                str = item.v10;
//                break;

//            case "v11":
//                str = item.v11;
//                break;

//            case "v12":
//                str = item.v12;
//                break;

//            case "v13":
//                str = item.v13;
//                break;

//            case "v14":
//                str = item.v14;
//                break;

//            case "v15":
//                str = item.v15;
//                break;

//            case "v16":
//                str = item.v16;
//                break;

//            case "v17":
//                str = item.v17;
//                break;

//            case "v18":
//                str = item.v18;
//                break;

//            case "v19":
//                str = item.v19;
//                break;

//            case "v20":
//                str = item.v20;
//                break;

//            case "v21":
//                str = item.v21;
//                break;

//            case "v22":
//                str = item.v22;
//                break;

//            case "v23":
//                str = item.v23;
//                break;

//            case "v24":
//                str = item.v24;
//                break;

//            case "v25":
//                str = item.v25;
//                break;

//            case "v26":
//                str = item.v26;
//                break;

//            case "v27":
//                str = item.v27;
//                break;

//            case "v28":
//                str = item.v28;
//                break;

//            case "v29":
//                str = item.v29;
//                break;

//            case "v30":
//                str = item.v30;
//                break;
//        }
//    }

//    return ExtConvert.ToDecimalOrNull(str);
//}

//public static DateTime? GetDateValueFrmList(IEnumerable<GridListEntity> list, string controlid)
//{
//    string str = "";
//    if (list == null)
//    {
//        return null;
//    }
//    foreach (GridListEntity item in list)
//    {
//        switch (controlid)
//        {
//            case "v1":
//                str = item.v1;
//                break;

//            case "v2":
//                str = item.v2;
//                break;

//            case "v3":
//                str = item.v3;
//                break;

//            case "v4":
//                str = item.v4;
//                break;

//            case "v5":
//                str = item.v5;
//                break;

//            case "v6":
//                str = item.v6;
//                break;

//            case "v7":
//                str = item.v7;
//                break;

//            case "v8":
//                str = item.v8;
//                break;

//            case "v9":
//                str = item.v9;
//                break;

//            case "v10":
//                str = item.v10;
//                break;

//            case "v11":
//                str = item.v11;
//                break;

//            case "v12":
//                str = item.v12;
//                break;

//            case "v13":
//                str = item.v13;
//                break;

//            case "v14":
//                str = item.v14;
//                break;

//            case "v15":
//                str = item.v15;
//                break;

//            case "v16":
//                str = item.v16;
//                break;

//            case "v17":
//                str = item.v17;
//                break;

//            case "v18":
//                str = item.v18;
//                break;

//            case "v19":
//                str = item.v19;
//                break;

//            case "v20":
//                str = item.v20;
//                break;

//            case "v21":
//                str = item.v21;
//                break;

//            case "v22":
//                str = item.v22;
//                break;

//            case "v23":
//                str = item.v23;
//                break;

//            case "v24":
//                str = item.v24;
//                break;

//            case "v25":
//                str = item.v25;
//                break;

//            case "v26":
//                str = item.v26;
//                break;

//            case "v27":
//                str = item.v27;
//                break;

//            case "v28":
//                str = item.v28;
//                break;

//            case "v29":
//                str = item.v29;
//                break;

//            case "v30":
//                str = item.v30;
//                break;
//        }
//    }

//    return ExtConvert.ToDateOrNull(str);
//}