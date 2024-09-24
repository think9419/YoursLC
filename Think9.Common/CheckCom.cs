using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Services.Com
{
    public class CheckCom
    {
        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");
        private static readonly DataTable _table = new();
        private static ComService comService = new ComService();

        /// <summary>
        /// 自定义校验
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="prcsNo"></param>
        /// <param name="dsMain"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetCustomValidate(string flowId, string prcsNo, DataTable dsMain, IEnumerable<GridListEntity> list = null)
        {
            string err = "";
            string mainTbId = flowId.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string flowType = "0";//流程类型？1固定2自由流程 0无流程
            if (flowId.StartsWith("fw_"))
            {
                foreach (DataRow dr in comService.GetDataTable("select flowType from flow where flowid = '" + flowId + "'").Rows)
                {
                    flowType = dr["flowType"].ToString();
                }
            }

            //先检测主表
            DataTable dsCheck = comService.GetDataTable("select * from tbvaluecheck where TbId = '" + mainTbId + "' and isUse='1' order by IOrder");
            if (dsCheck.Rows.Count > 0)
            {
                DataTable dsindex = comService.GetDataTable("select * from tbindex where TbId = '" + mainTbId + "'");
                DataTable dtlist = GetMainValidate(dsMain, dsindex, dsCheck, flowId, flowType, prcsNo, mainTbId);

                err = ListValidate(dtlist, mainTbId);
            }

            //开始子表校验
            if (string.IsNullOrEmpty(err))
            {
                if (list != null)
                {
                    DataTable dsTb = comService.GetDataTable("select TbId from tbbasic where ParentId = '" + mainTbId + "' ");
                    foreach (DataRow drtb in dsTb.Rows)
                    {
                        string gridTbId = drtb["TbId"].ToString().ToLower();
                        dsCheck = comService.GetDataTable("select * from tbvaluecheck where TbId = '" + gridTbId + "' and isUse='1' order by IOrder");
                        if (dsCheck.Rows.Count > 0)
                        {
                            DataTable dtGrid = DataTableHelp.IEnumerableToDataTable<GridListEntity>(list.Where(x => x.flag.ToLower().StartsWith("#" + gridTbId + "#")));
                            int iGridRowsCount = dtGrid.Rows.Count;

                            DataTable dtlist = GetGridValidate(dtGrid, iGridRowsCount, dsCheck, flowId, flowType, prcsNo, gridTbId);

                            err = ListValidate(dtlist, mainTbId);
                            if (!string.IsNullOrEmpty(err))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return err;
        }

        /// <summary>
        ///主表 唯一及主键检测
        /// </summary>
        public static string CheckMainTbValueBKAndUnique(long listid, string tbid, object param, DataTable dt)
        {
            string err = "";
            string where = " where 1=1 ";
            if (listid != 0)
            {
                where = " where ListID <> " + listid + " ";//编辑时
            }
            string str = " 不满足主键条件 已有数据:";
            bool isCheck = false;

            DataTable index = comService.GetDataTable("tbindex", "IndexId,IndexName,TbId,DataType,isUnique,isPK", "where (TbId = @TbId) and (isPK=@isPK or isUnique=@isUnique)  ", "", new { TbId = tbid, isPK = 1, isUnique = 1 });
            foreach (DataRow row in index.Rows)
            {
                //主键检测
                if (row["isPK"].ToString() == "1")
                {
                    if (string.IsNullOrEmpty(dt.Rows[0][row["IndexId"].ToString()].ToString()))
                    {
                        err += " 主键" + row["IndexName"].ToString() + "是必填项";
                    }
                    else
                    {
                        isCheck = true;
                        where += " and " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();
                        str += row["IndexName"].ToString() + "=" + dt.Rows[0][row["IndexId"].ToString()].ToString() + " ";
                    }
                }
            }

            if (isCheck && string.IsNullOrEmpty(err))
            {
                if (comService.GetTotal(tbid, where, param) > 0)
                {
                    err += str;
                }
            }

            foreach (DataRow row in index.Rows)
            {
                //唯一检测
                if (row["isUnique"].ToString() == "1")
                {
                    if (string.IsNullOrEmpty(dt.Rows[0][row["IndexId"].ToString()].ToString()))
                    {
                        err += " 唯一键" + row["IndexName"].ToString() + "是必填项";
                    }
                    else
                    {
                        where = " where " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();
                        if (listid != 0)
                        {
                            where = " where ListID <> " + listid + " and " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();//编辑时
                        }
                        str = " 不满足唯一条件 已有数据:" + row["IndexName"].ToString() + "=" + dt.Rows[0][row["IndexId"].ToString()].ToString();

                        if (comService.GetTotal(tbid, where, param) > 0)
                        {
                            err += str;
                        }
                    }
                }
            }
            return err;
        }

        /// <summary>
        ///子表 唯一及主键检测
        /// </summary>
        public static string CheckGridTbValueBKAndUnique(long listid, string tbid, object param, DataTable dt)
        {
            DataTable index = comService.GetDataTable("tbindex", "IndexId,IndexName,TbId,DataType,isUnique,isPK", "WHERE (TbId = @TbId) and (isPK=@isPK OR isUnique=@isUnique)  ", "", new { TbId = tbid, isPK = 1, isUnique = 1 });
            string err = "";
            string where = " WHERE ListID = " + listid + " ";
            string str = "不满足唯一条件 已有数据:";
            bool bCheck = false;

            foreach (DataRow row in index.Rows)
            {
                //主键检测row["IndexName"].ToString()
                if (row["isPK"].ToString() == "1")
                {
                    if (string.IsNullOrEmpty(dt.Rows[0][row["IndexId"].ToString()].ToString()))
                    {
                        err += " 主键" + row["IndexName"].ToString() + "是必填项";
                    }
                    else
                    {
                        bCheck = true;
                        where += " AND " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();
                        str += row["IndexName"].ToString() + "=" + dt.Rows[0][row["IndexId"].ToString()].ToString() + " ";
                    }
                }
            }

            if (bCheck && string.IsNullOrEmpty(err))
            {
                if (comService.GetTotal(tbid, where, param) > 0)
                {
                    err += str;
                }
            }

            foreach (DataRow row in index.Rows)
            {
                //唯一检测
                if (row["isUnique"].ToString() == "1")
                {
                    if (string.IsNullOrEmpty(dt.Rows[0][row["IndexId"].ToString()].ToString()))
                    {
                        err += " 唯一键" + row["IndexName"].ToString() + "是必填项";
                    }
                    else
                    {
                        where = " WHERE ListID = " + listid + " AND " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();
                        str = "不满足唯一条件 已有数据:" + row["IndexName"].ToString() + "=" + dt.Rows[0][row["IndexId"].ToString()].ToString();

                        if (comService.GetTotal(tbid, where, param) > 0)
                        {
                            err += str;
                        }
                    }
                }
            }

            return err;
        }

        /// <summary>
        ///子表 唯一及主键检测
        /// </summary>
        public static string CheckGridTbValueBKAndUnique(long listid, long id, string tbid, object param, DataTable dt, DataTable index)
        {
            string err = "";
            string where = " WHERE ListID = " + listid + " AND ID <> " + id;
            string str = "不满足唯一条件" + tbid + "有多个数据:";
            bool bCheck = false;

            foreach (DataRow row in index.Rows)
            {
                //主键检测row["IndexName"].ToString()
                if (row["isPK"].ToString() == "1")
                {
                    if (string.IsNullOrEmpty(dt.Rows[0][row["IndexId"].ToString()].ToString()))
                    {
                        err += " 主键" + row["IndexName"].ToString() + "是必填项";
                    }
                    else
                    {
                        bCheck = true;
                        where += " AND " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();
                        str += row["IndexName"].ToString() + "=" + dt.Rows[0][row["IndexId"].ToString()].ToString() + " ";
                    }
                }
            }

            if (bCheck && string.IsNullOrEmpty(err))
            {
                if (comService.GetTotal(tbid, where, param) > 0)
                {
                    err += str;
                }
            }

            foreach (DataRow row in index.Rows)
            {
                //唯一检测
                if (row["isUnique"].ToString() == "1")
                {
                    if (string.IsNullOrEmpty(dt.Rows[0][row["IndexId"].ToString()].ToString()))
                    {
                        err += " 唯一键" + row["IndexName"].ToString() + "是必填项";
                    }
                    else
                    {
                        where = " WHERE ListID = " + listid + " AND ID <> " + id + " AND " + row["IndexId"].ToString() + " = @" + row["IndexId"].ToString();
                        str = "不满足唯一条件" + tbid + "有多个数据:" + row["IndexName"].ToString() + "=" + dt.Rows[0][row["IndexId"].ToString()].ToString();

                        if (comService.GetTotal(tbid, where, param) > 0)
                        {
                            err += str;
                        }
                    }
                }
            }

            return err;
        }

        public static string CheckedTableBeforeBegin(string fwid, string isRelease = "n")
        {
            string err = "";
            string str = "";
            string fileName;
            string tbid = fwid.Replace("fw_", "tb_").Replace("bi_", "tb_");

            fileName = directoryPath + "\\Views\\" + tbid.Replace("tb_", "") + "\\Index.cshtml";
            if (!File.Exists(fileName))
            {
                err += string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, ""));
            }
            else
            {
                if (isRelease == "y")
                {
                    str = Think9.Util.Helper.FileHelper.FileToString(fileName);
                    if (str.Contains("/" + tbid.Replace("tb_", "") + "/"))
                    {
                        err += string.Format(" 文件{0}使用了『调试模式』调用方式，需『重新生成』;", fileName.Replace(directoryPath, ""));
                    }
                }
            }

            fileName = directoryPath + "\\Views\\" + tbid.Replace("tb_", "") + "\\Form.cshtml";
            if (!File.Exists(fileName))
            {
                err += string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, ""));
            }
            fileName = directoryPath + "\\Views\\" + tbid.Replace("tb_", "") + "\\Detail.cshtml";
            if (!File.Exists(fileName))
            {
                err += string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, ""));
            }
            fileName = directoryPath + "\\wwwroot\\Self_JS\\" + tbid.Replace("tb_", "") + ".js";
            if (!File.Exists(fileName))
            {
                err += string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, ""));
            }

            if (!comService.IsDataTableExists(tbid))
            {
                err += " 未创建数据表" + tbid + " - 进入录入表自定义|录入表管理页面，左侧选择录入表，点击数据表按钮再点击创建数据表按钮；";
            }
            else
            {
                DataTable table = comService.GetDataTable("tbbasic", "TbId,ParentId", "where (ParentId = @TbId) ", "", new { TbId = tbid });
                foreach (DataRow row in table.Rows)
                {
                    if (!comService.IsDataTableExists(row["TbId"].ToString()))
                    {
                        err += " 未创建数据表" + row["TbId"].ToString() + " - 进入录入表自定义|录入表管理页面，左侧选择录入表，点击数据表按钮再点击创建数据表按钮；";
                    }
                }
            }
            return err;
        }

        public static string CheckedReportBeforeBegin(string rpid)
        {
            string err = "";
            string fileName = directoryPath + "\\Views\\" + rpid.Replace("rp_", "") + "\\Index.cshtml";
            if (!File.Exists(fileName))
            {
                err += string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, ""));
            }

            return err;
        }

        //添加前校验
        public static string CheckedBeforeAdd(string fwid, CurrentPrcsEntity mPrcs, CurrentUserEntity user)
        {
            if (user == null)
            {
                return "当前用户对象为空，请重新登录";
            }
            string err = "";
            string sUser = ";" + user.Account + ";";
            string sDep = ";" + user.DeptNo + ";";
            string sRole = ";" + user.RoleNo + ";";

            //基础信息表
            if (fwid.StartsWith("bi_"))
            {
                ServiceFlow flowService = new ServiceFlow();
                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
                if (mflow == null)
                {
                    err = "FlowEntity对象不存在";
                }
                else
                {
                    mflow.EditUser = mflow.EditUser == null ? "" : mflow.EditUser;
                    if (mflow.isUse == "2")
                    {
                        err = "流程已禁用 不能再编辑或办理！";
                    }
                    else
                    {
                        mflow.EditUser = mflow.EditUser == null ? "" : mflow.EditUser;
                        if (mflow.EditUser != "#all#")
                        {
                            if (!mflow.EditUser.Contains(sUser))
                            {
                                err = user.Account + "非指定用户，不能新建、编辑和删除数据！";
                            }
                        }
                    }
                }
            }
            else
            {
                err = user.Account + "无新建权限！";
                if (mPrcs.PrcsUser == "#all#" || mPrcs.PrcsDept == "#all#" || mPrcs.PrcsPriv == "#all#")
                {
                    err = "";
                }
                else
                {
                    if (mPrcs.PrcsUser.Contains(sUser) || mPrcs.PrcsDept.Contains(sDep) || mPrcs.PrcsPriv.Contains(sRole))
                    {
                        err = "";
                    }
                }
            }

            return err;
        }

        // 编辑前校验
        public static string CheckedBeforeEdit(string fwid, string listid, CurrentPrcsEntity mPrcs, CurrentUserEntity user, int lockUpTime = 0)
        {
            string err = "";
            if (user == null)
            {
                err = "当前用户对象为空，请重新登录";
                return err;
            }

            string sUser = ";" + user.Account + ";";
            string sDep = ";" + user.DeptNo + ";";
            string sRole = ";" + user.RoleNo + ";";

            //基础信息表
            if (fwid.StartsWith("bi_"))
            {
                ServiceFlow flowService = new ServiceFlow();
                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
                if (mflow == null)
                {
                    err = "流程不存在";
                    return err;
                }

                mflow.EditUser = mflow.EditUser == null ? "" : mflow.EditUser;
                if (mflow.isUse == "2")
                {
                    err = "已禁用,不能再编辑或办理！";
                    return err;
                }
                else
                {
                    if (mflow.EditUser != "#all#")
                    {
                        if (!mflow.EditUser.Contains(sUser))
                        {
                            err = user.Account + "非指定用户，不能新建、编辑和删除数据！";
                            return err;
                        }
                    }
                }

                string createUser = "";
                string isLock = "1";
                DataTable dt = comService.GetDataTable(fwid.Replace("bi_", "tb_"), "isLock,createUser", "listid=" + listid + "", "");
                if (dt.Rows.Count > 0)
                {
                    isLock = dt.Rows[0]["isLock"].ToString();
                    createUser = dt.Rows[0]["createUser"].ToString();
                }

                if (isLock == "1")
                {
                    err = "当前数据已锁定！";
                    return err;
                }

                //11查看编辑用户本人新建的数据
                //22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
                //21查看编辑用户所属单位(部门)新建的数据
                //32查看用户所属下级部门新建的数据  编辑本人新建的数据
                //31查看编辑用户所属下级部门新建的数据
                //42查看所有数据  编辑本人新建的数据
                //41查看编辑所有数据
                //mflow.SearchMode = string.IsNullOrEmpty(mflow.SearchMode) ? "11" : mflow.SearchMode;
                mflow.SearchMode = BasicHelp.GetSearchMode(mflow.SearchMode, fwid, user == null ? "!NullEx" : user.Account);
                if (mflow.SearchMode.Substring(1, 1) != "1")
                {
                    if (user.Account != createUser)
                    {
                        err = user.Account + "对当前数据，无编辑、删除权限！";
                        return err;
                    }
                }
            }
            else
            {
                // 0待新建
                if (mPrcs.runFlag == "0")
                {
                    err = user.Account + "无新建权限！";
                    if (mPrcs.PrcsUser == "#all#" || mPrcs.PrcsDept == "#all#" || mPrcs.PrcsPriv == "#all#")
                    {
                        err = "";
                    }
                    else
                    {
                        if (mPrcs.PrcsUser.Contains(sUser) || mPrcs.PrcsDept.Contains(sDep) || mPrcs.PrcsPriv.Contains(sRole))
                        {
                            err = "";
                        }
                    }
                }

                //1待接手
                if (mPrcs.runFlag == "1")
                {
                    err = user.Account + "非当前授权接手办理人员";
                    if (mPrcs.PrcsUser == "#all#" || mPrcs.PrcsDept == "#all#" || mPrcs.PrcsPriv == "#all#")
                    {
                        err = "";
                    }
                    else
                    {
                        if (mPrcs.PrcsUser.Contains(sUser) || mPrcs.PrcsDept.Contains(sDep) || mPrcs.PrcsPriv.Contains(sRole))
                        {
                            err = "";
                        }
                    }
                }

                //2办理中
                if (mPrcs.runFlag == "2")
                {
                    err = user.Account + "非当前办理人员，不能编辑或办理！备注：基础信息表不受当前办理人员限制";
                    if (mPrcs.currentPrcsUser1.Contains(sUser))
                    {
                        err = "";
                    }
                }

                //3已办结
                if (mPrcs.runFlag == "3")
                {
                    err = "已结束 在数据管理页面中将其转交后才能编辑、办理和删除！";
                }

                if (!string.IsNullOrEmpty(err))
                {
                    return err;
                }

                //mPrcs.SearchMode;
                //11查看编辑用户本人新建的数据
                //22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
                //21查看编辑用户所属单位(部门)新建的数据
                //32查看用户所属下级部门新建的数据  编辑本人新建的数据
                //31查看编辑用户所属下级部门新建的数据
                //42查看所有数据  编辑本人新建的数据
                //41查看编辑所有数据
                //mPrcs.SearchMode = string.IsNullOrEmpty(mPrcs.SearchMode) ? "11" : mPrcs.SearchMode;
                mPrcs.SearchMode = BasicHelp.GetSearchMode(mPrcs.SearchMode, fwid, user == null ? "!NullEx" : user.Account);
                if (mPrcs.SearchMode.Substring(1, 1) != "1")
                {
                    if (user.Account != mPrcs.createUser)
                    {
                        err = user.Account + "按照设置的查看编辑模式您对当前数据只能查看，无编辑、删除权限！";
                    }
                }
            }

            //lockUpTime锁定期限，非0则用户List页面点击编辑按钮则锁定数据，Form页面点击保存按钮则解锁数据，锁定期间并且没有超过锁定期限规定的时间，不能再编辑数据
            if (string.IsNullOrEmpty(err))
            {
                err = LockedService.LockedBeforeEdit(fwid, listid, user.Account, lockUpTime);
            }

            return err;
        }

        //校验权限
        public static string CheckedBeforeDel(string fwid, string listid, CurrentPrcsEntity mPrcs, CurrentUserEntity user, int lockUpTime = 0)
        {
            string err = "";
            if (user == null)
            {
                err = "当前用户对象为空，请重新登录";
                return err;
            }

            string sUser = ";" + user.Account + ";";
            string sDep = ";" + user.DeptNo + ";";
            string sRole = ";" + user.RoleNo + ";";
            string createUser = "";
            string isLock = "1";

            //基础信息表
            if (fwid.StartsWith("bi_"))
            {
                ServiceFlow flowService = new ServiceFlow();
                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
                if (mflow == null)
                {
                    err = "流程不存在";
                    return err;
                }

                mflow.EditUser = mflow.EditUser == null ? "" : mflow.EditUser;
                if (mflow.isUse == "2")
                {
                    err = "已禁用 不能再编辑或办理！";
                    return err;
                }
                else
                {
                    if (mflow.EditUser != "#all#")
                    {
                        if (!mflow.EditUser.Contains(sUser))
                        {
                            err = user.Account + "非指定用户，不能新建、编辑和删除数据！";
                            return err;
                        }
                    }
                }

                DataTable dt = comService.GetDataTable(fwid.Replace("bi_", "tb_"), "isLock,createUser", "listid=" + listid + "", "");
                if (dt.Rows.Count > 0)
                {
                    isLock = dt.Rows[0]["isLock"].ToString();
                    createUser = dt.Rows[0]["createUser"].ToString();
                }

                if (isLock == "1")
                {
                    err = "当前数据已锁定！";
                    return err;
                }

                //11查看编辑用户本人新建的数据
                //22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
                //21查看编辑用户所属单位(部门)新建的数据
                //32查看用户所属下级部门新建的数据  编辑本人新建的数据
                //31查看编辑用户所属下级部门新建的数据
                //42查看所有数据  编辑本人新建的数据
                //41查看编辑所有数据
                //mflow.SearchMode = string.IsNullOrEmpty(mflow.SearchMode) ? "11" : mflow.SearchMode;
                mflow.SearchMode = BasicHelp.GetSearchMode(mflow.SearchMode, fwid, user == null ? "!NullEx" : user.Account);
                if (mflow.SearchMode.Substring(1, 1) != "1")
                {
                    if (user.Account != createUser)
                    {
                        err = user.Account + "对当前数据，无编辑、删除权限！";
                        return err;
                    }
                }
            }
            else
            {
                // 0待新建
                if (mPrcs.runFlag == "0")
                {
                    err = user.Account + "无新建权限！";
                    if (mPrcs.PrcsUser == "#all#" || mPrcs.PrcsDept == "#all#" || mPrcs.PrcsPriv == "#all#")
                    {
                        err = "";
                    }
                    else
                    {
                        if (mPrcs.PrcsUser.Contains(sUser) || mPrcs.PrcsDept.Contains(sDep) || mPrcs.PrcsPriv.Contains(sRole))
                        {
                            err = "";
                        }
                    }
                }

                //1待接手
                if (mPrcs.runFlag == "1")
                {
                    err = user.Account + "非当前授权接手办理人员";
                    if (mPrcs.PrcsUser == "#all#" || mPrcs.PrcsDept == "#all#" || mPrcs.PrcsPriv == "#all#")
                    {
                        err = "";
                    }
                    else
                    {
                        if (mPrcs.PrcsUser.Contains(sUser) || mPrcs.PrcsDept.Contains(sDep) || mPrcs.PrcsPriv.Contains(sRole))
                        {
                            err = "";
                        }
                    }
                }

                //2办理中
                if (mPrcs.runFlag == "2")
                {
                    err = user.Account + "非当前办理人员，不能编辑或办理！备注：基础信息表不受当前办理人员限制";
                    if (mPrcs.currentPrcsUser1.Contains(sUser))
                    {
                        err = "";
                    }
                }

                //3已办结
                if (mPrcs.runFlag == "3")
                {
                    err = "已结束 在数据管理页面中将其转交后才能编辑、办理和删除！";
                }

                if (!string.IsNullOrEmpty(err))
                {
                    return err;
                }

                //mPrcs.SearchMode;
                //11查看编辑用户本人新建的数据
                //22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
                //21查看编辑用户所属单位(部门)新建的数据
                //32查看用户所属下级部门新建的数据  编辑本人新建的数据
                //31查看编辑用户所属下级部门新建的数据
                //42查看所有数据  编辑本人新建的数据
                //41查看编辑所有数据
                if (mPrcs.SearchMode.Substring(1, 1) != "1")
                {
                    if (user.Account != mPrcs.createUser)
                    {
                        err = user.Account + "对当前数据无编辑、删除权限！";
                    }
                }
            }

            //lockUpTime锁定期限，非0则用户List页面点击编辑按钮则锁定数据，Form页面点击保存按钮则解锁数据，锁定期间并且没有超过锁定期限规定的时间，不能再编辑数据
            if (string.IsNullOrEmpty(err))
            {
                err = LockedService.Checked(fwid, listid, lockUpTime);
            }

            return err;
        }

        /// <summary>
        /// 检测输入的值是否有错误--用于子表的检测
        /// </summary>
        /// <returns></returns>
        private static DataTable GetGridValidate(DataTable dsGridTb, int iGridRowsCount, DataTable dsCheck, string sFlowId, string sFlowType, string sPrcsNo, string gridTbId)
        {
            DataTable dsindex = comService.GetDataTable("select * from tbindex where TbId = '" + gridTbId + "'");
            long maxGridNum = comService.GetTotal("tbindex", "where TbId='" + gridTbId + "'");

            string _leftValue = "";
            string _compare = "";
            string _rightValue = "";
            string _explain = "";
            string _nullCase = "";
            string _rowsNum = "";
            string _flowStr = "";//适用流程
            bool isCheck = false;//是否校验
            string _Original = "";

            DataTable dt = NewDataTableCheck();
            foreach (DataRow rcheck in dsCheck.Rows)
            {
                _leftValue = TableHelp.ReplaceOperator(rcheck["LeftValue"].ToString().ToLower());
                _rightValue = TableHelp.ReplaceOperator(rcheck["RightValue"].ToString().ToLower());
                _compare = rcheck["Compare"].ToString();
                _explain = rcheck["Explain"].ToString();
                _nullCase = rcheck["NullCase"].ToString();
                _flowStr = rcheck["FlowStr"].ToString();//适用流程
                _Original = rcheck["LeftValue"].ToString() + rcheck["Compare"].ToString() + rcheck["RightValue"].ToString();

                if (_leftValue.Contains("[") || _rightValue.Contains("["))
                {
                    isCheck = GetIsCheck(sFlowType, _flowStr, sPrcsNo);//是否校验
                }
                else
                {
                    isCheck = GetIsCheck(sFlowType, _flowStr, sPrcsNo);//是否校验
                }

                //根据设定的流程（情况），适用才校验
                if (isCheck)
                {
                    ReplaceCharacterByValue(ref _leftValue, ref _rightValue, gridTbId, (int)maxGridNum, _nullCase, dsGridTb, dsindex);

                    //子表编码.指标编码.Length-表示当前行指标字符长度，子表编码.指标编码.Value-表示当前行指标数值
                    //子表编码.指标编码[行号].Length-表示该行指标字符长度，子表编码.指标编码[行号].Value-表示该行指标数值，如tb_tb001.v2[3].Value表示第二列(v2)第三行([3])指标数值
                    //替换完后，还存在就要逐行判断
                    if (_leftValue.Contains(".value") || _leftValue.Contains(".length") || _rightValue.Contains(".value") || _rightValue.Contains(".length"))
                    {
                        foreach (DataRow dr in dsGridTb.Rows)
                        {
                            //这个很重要，要重新赋值
                            _leftValue = TableHelp.ReplaceOperator(rcheck["LeftValue"].ToString().ToLower());
                            _rightValue = TableHelp.ReplaceOperator(rcheck["RightValue"].ToString().ToLower());

                            string num = GetNum(dr["flag"].ToString());//第几行

                            //第0行是增加行，跳过
                            if (num != "0")
                            {
                                ReplaceCharacterByValue(ref _leftValue, ref _rightValue, gridTbId, (int)maxGridNum, _nullCase, dr, dsindex);

                                //还存在
                                if (_leftValue.Contains("].value") || _rightValue.Contains("].value") || _leftValue.Contains("].length") || _rightValue.Contains("].length"))
                                {
                                    ReplaceCharacterByValue(ref _leftValue, ref _rightValue, gridTbId, (int)maxGridNum, _nullCase, dsGridTb, dsindex);
                                }

                                DataRow row = dt.NewRow();
                                row["LeftValue"] = _leftValue;
                                row["RightValue"] = _rightValue;
                                row["Compare"] = _compare;
                                row["Explain"] = "第" + num + "行" + _explain;
                                row["IOrder"] = rcheck["IOrder"].ToString();
                                row["RowsNum"] = num;//第几行
                                row["FlowStr"] = _flowStr;//适用流程
                                row["Original"] = _Original;//适用流程

                                dt.Rows.Add(row);
                            }
                        }
                    }
                    else
                    {
                        DataRow row = dt.NewRow();
                        row["LeftValue"] = _leftValue;
                        row["RightValue"] = _rightValue;
                        row["Compare"] = _compare;
                        row["Explain"] = _explain;
                        row["IOrder"] = rcheck["IOrder"].ToString();
                        row["RowsNum"] = _rowsNum;//第几行
                        row["FlowStr"] = _flowStr;//适用流程
                        row["Original"] = _Original;//适用流程

                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 检测输入的值是否有错误--用于主表的检测
        /// </summary>
        /// <returns></returns>
        private static DataTable GetMainValidate(DataTable dsMain, DataTable dsindex, DataTable dsCheck, string sFlowId, string sFlowType, string sPrcsNo, string sTbId)
        {
            string _indexId = "";
            string _value = "";
            string _dataType = "";

            string _leftValue = "";
            string _compare = "";
            string _rightValue = "";
            string _explain = "";
            string _nullCase = "";
            string _flowStr = "";
            string _Original = "";

            bool isCheck = false;//是否校验

            DataTable dt = NewDataTableCheck();
            foreach (DataRow rcheck in dsCheck.Rows)
            {
                _leftValue = TableHelp.ReplaceOperator(rcheck["LeftValue"].ToString().ToLower());
                _rightValue = TableHelp.ReplaceOperator(rcheck["RightValue"].ToString().ToLower());
                _compare = rcheck["Compare"].ToString();
                _explain = rcheck["Explain"].ToString();
                _nullCase = rcheck["NullCase"].ToString();
                _flowStr = rcheck["FlowStr"].ToString();//适用流程
                _Original = rcheck["LeftValue"].ToString() + rcheck["Compare"].ToString() + rcheck["RightValue"].ToString();

                isCheck = GetIsCheck(sFlowType, _flowStr, sPrcsNo);//是否校验

                //根据设定的流程（情况），适用才校验
                if (isCheck)
                {
                    foreach (DataRow drindex in dsindex.Rows)
                    {
                        _indexId = drindex["IndexId"].ToString().Trim();
                        _dataType = drindex["DataType"].ToString().Trim();

                        _value = dsMain.Rows[0][_indexId].ToString().Trim();

                        ReplaceCharacterByValue(ref _leftValue, ref _rightValue, sTbId, _value, _indexId, _dataType, _nullCase, "");
                    }

                    DataRow row = dt.NewRow();
                    row["LeftValue"] = _leftValue;
                    row["RightValue"] = _rightValue;
                    row["Compare"] = _compare;
                    row["Explain"] = _explain;
                    row["IOrder"] = rcheck["IOrder"].ToString();
                    row["FlowStr"] = _flowStr;//适用流程
                    row["Original"] = _Original;//适用流程
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        private static void ReplaceCharacterByValue(ref string sLeftValue, ref string sRightValue, string sTbId, string sValue, string sIndexId, string sDataType, string sNullCase, string sAdd)
        {
            string sCompare = "";
            string sNewValue = "";

            sCompare = sAdd + sIndexId;
            if (sLeftValue.Contains(sCompare.ToLower()) || sRightValue.Contains(sCompare.ToLower()))
            {
                //替换字符长度  格式为：录入表编码+.+指标编码+.length
                sCompare = sTbId + "." + sAdd + sIndexId + ".length";
                sNewValue = sValue.Length.ToString();

                if (sLeftValue.Contains(sCompare.ToLower()))
                {
                    sLeftValue = sLeftValue.Replace(sCompare.ToLower(), sNewValue);
                }
                if (sRightValue.Contains(sCompare.ToLower()))
                {
                    sRightValue = sRightValue.Replace(sCompare.ToLower(), sNewValue);
                }

                //替换字符长度  格式为：指标编码+
                sCompare = sAdd + sIndexId + ".length";
                sNewValue = sValue.Length.ToString();

                if (sLeftValue.Contains(sCompare.ToLower()))
                {
                    sLeftValue = sLeftValue.Replace(sCompare.ToLower(), sNewValue);
                }
                if (sRightValue.Contains(sCompare.ToLower()))
                {
                    sRightValue = sRightValue.Replace(sCompare.ToLower(), sNewValue);
                }

                //很重要ZZZ
                sNewValue = sValue;

                //为空值并且属于数值型
                if (sValue == "" && sDataType.StartsWith("3"))
                {
                    //空值处理
                    if (sNullCase == "0")
                    {
                        sNewValue = "0";
                    }
                    else
                    {
                        sNewValue = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                    }
                }

                //为空值并且属于日期型
                if (sValue == "" && sDataType.StartsWith("1"))
                {
                    //空值处理
                    if (sNullCase == "0")
                    {
                        sNewValue = "9999-12-31";
                    }
                    else
                    {
                        sNewValue = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                    }
                }

                //替换指标值   格式为：录入表编码+.+指标编码+.value
                sCompare = sTbId + "." + sAdd + sIndexId + ".value";
                if (sLeftValue.Contains(sCompare.ToLower()))
                {
                    sLeftValue = sLeftValue.Replace(sCompare.ToLower(), sNewValue);
                }
                if (sRightValue.Contains(sCompare.ToLower()))
                {
                    sRightValue = sRightValue.Replace(sCompare.ToLower(), sNewValue);
                }

                //替换指标值   格式为：指标编码+.value
                sCompare = sAdd + sIndexId + ".value";
                if (sLeftValue.Contains(sCompare.ToLower()))
                {
                    sLeftValue = sLeftValue.Replace(sCompare.ToLower(), sNewValue);
                }
                if (sRightValue.Contains(sCompare.ToLower()))
                {
                    sRightValue = sRightValue.Replace(sCompare.ToLower(), sNewValue);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <param name="gridTbId">子表id</param>
        /// <param name="maxGridNum">最多列数</param>
        /// <param name="indexDataType">指标类型</param>
        /// <param name="nullCase">空值处理</param>
        /// <param name="dsGridTb"></param>
        private static void ReplaceCharacterByValue(ref string leftValue, ref string rightValue, string gridTbId, int maxGridNum, string nullCase, DataTable dsGridTb, DataTable dsindex)
        {
            string _old;
            string _value;
            string indexDataType = "";

            //子表编码.指标编码.Length-表示当前行指标字符长度，子表编码.指标编码.Value-表示当前行指标数值
            //子表编码.指标编码[行号].Length-表示该行指标字符长度，子表编码.指标编码[行号].Value-表示该行指标数值，如tb_tb001.v2[3].Value表示第二列(v2)第三行([3])指标数值

            //maxGridNum最多列数
            for (int i = 1; i <= maxGridNum; i++)
            {
                _old = "v" + i.ToString() + "[";
                if (leftValue.Contains(_old.ToLower()) || rightValue.Contains(_old.ToLower()))
                {
                    //iGridRowsCount最大行号
                    for (int j = 1; j <= dsGridTb.Rows.Count; j++)
                    {
                        //字符长度替换 格式为：录入表编码+.+指标编码+.length
                        _old = gridTbId + ".v" + i.ToString() + "[" + j.ToString() + "].length";
                        if (leftValue.Contains(_old.ToLower()) || rightValue.Contains(_old.ToLower()))
                        {
                            _value = GetLength(dsGridTb, gridTbId, i.ToString(), j.ToString());

                            leftValue = leftValue.Replace(_old.ToLower(), _value);
                            rightValue = rightValue.Replace(_old.ToLower(), _value);
                        }

                        //字符长度替换 格式为：指标编码+.length
                        _old = "v" + i.ToString() + "[" + j.ToString() + "].length";
                        if (leftValue.Contains(_old.ToLower()) || rightValue.Contains(_old.ToLower()))
                        {
                            _value = GetLength(dsGridTb, gridTbId, i.ToString(), j.ToString());

                            leftValue = leftValue.Replace(_old.ToLower(), _value);
                            rightValue = rightValue.Replace(_old.ToLower(), _value);
                        }

                        //值替换  格式为：录入表编码+.+指标编码+.length
                        _old = gridTbId + ".v" + i.ToString() + "[" + j.ToString() + "].value";
                        if (leftValue.Contains(_old.ToLower()) || rightValue.Contains(_old.ToLower()))
                        {
                            indexDataType = GetIndexDataType(dsindex, i.ToString());
                            _value = GetValue(dsGridTb, gridTbId, i.ToString(), j.ToString());
                            //注意条件--为空值并且属于数值型
                            if (_value == "" && indexDataType.StartsWith("3"))
                            {
                                //空值处理
                                if (nullCase == "0")
                                {
                                    _value = "0";
                                }
                                else
                                {
                                    _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                                }
                            }

                            //注意条件--为空值并且属于日期型
                            if (_value == "" && indexDataType.StartsWith("1"))
                            {
                                //空值处理
                                if (nullCase == "0")
                                {
                                    _value = "9999-12-31";
                                }
                                else
                                {
                                    _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                                }
                            }

                            leftValue = leftValue.Replace(_old.ToLower(), _value);
                            rightValue = rightValue.Replace(_old.ToLower(), _value);
                        }

                        //值替换  格式为：指标编码+.value
                        _old = "v" + i.ToString() + "[" + j.ToString() + "].value";
                        if (leftValue.Contains(_old.ToLower()) || rightValue.Contains(_old.ToLower()))
                        {
                            indexDataType = GetIndexDataType(dsindex, i.ToString());
                            _value = GetValue(dsGridTb, gridTbId, i.ToString(), j.ToString());
                            //注意条件--为空值并且属于数值型
                            if (_value == "" && indexDataType.StartsWith("3"))
                            {
                                //空值处理
                                if (nullCase == "0")
                                {
                                    _value = "0";
                                }
                                else
                                {
                                    _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                                }
                            }

                            //注意条件--为空值并且属于日期型
                            if (_value == "" && indexDataType.StartsWith("1"))
                            {
                                //空值处理
                                if (nullCase == "0")
                                {
                                    _value = "9999-12-31";
                                }
                                else
                                {
                                    _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                                }
                            }

                            leftValue = leftValue.Replace(_old.ToLower(), _value);
                            rightValue = rightValue.Replace(_old.ToLower(), _value);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <param name="gridTbId">子表id</param>
        /// <param name="maxGridNum">最多列数</param>
        /// <param name="indexDataType">指标类型</param>
        /// <param name="nullCase">空值处理</param>
        /// <param name="dsGridTb"></param>
        private static void ReplaceCharacterByValue(ref string _leftValue, ref string _rightValue, string gridTbId, int maxGridNum, string _nullCase, DataRow dr, DataTable dsindex)
        {
            string _old;
            string _value;
            string _indexDataType = "";

            //子表编码.指标编码.Length-表示当前行指标字符长度，子表编码.指标编码.Value-表示当前行指标数值
            //子表编码.指标编码[行号].Length-表示该行指标字符长度，子表编码.指标编码[行号].Value-表示该行指标数值，如tb_tb001.v2[3].Value表示第二列(v2)第三行([3])指标数值

            //maxGridNum最多列数
            for (int i = 1; i <= maxGridNum; i++)
            {
                _old = gridTbId + ".v" + i.ToString() + ".value";
                if (_leftValue.Contains(_old.ToLower()) || _rightValue.Contains(_old.ToLower()))
                {
                    _indexDataType = GetIndexDataType(dsindex, i.ToString());
                    _value = dr["v" + i.ToString()].ToString().Trim();
                    //注意条件--为空值并且属于数值型
                    if (_value == "" && _indexDataType.StartsWith("3"))
                    {
                        //空值处理
                        if (_nullCase == "0")
                        {
                            _value = "0";
                        }
                        else
                        {
                            _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                        }
                    }
                    //注意条件--为空值并且属于日期型
                    if (_value == "" && _indexDataType.StartsWith("1"))
                    {
                        //空值处理
                        if (_nullCase == "0")
                        {
                            _value = "9999-12-31";
                        }
                        else
                        {
                            _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                        }
                    }

                    _leftValue = _leftValue.Replace(_old.ToLower(), _value);
                    _rightValue = _rightValue.Replace(_old.ToLower(), _value);
                }

                _old = "v" + i.ToString() + ".value";
                if (_leftValue.Contains(_old.ToLower()) || _rightValue.Contains(_old.ToLower()))
                {
                    _indexDataType = GetIndexDataType(dsindex, i.ToString());
                    _value = dr["v" + i.ToString()].ToString().Trim();
                    //注意条件--为空值并且属于数值型
                    if (_value == "" && _indexDataType.StartsWith("3"))
                    {
                        //空值处理
                        if (_nullCase == "0")
                        {
                            _value = "0";
                        }
                        else
                        {
                            _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                        }
                    }
                    //注意条件--为空值并且属于日期型
                    if (_value == "" && _indexDataType.StartsWith("1"))
                    {
                        //空值处理
                        if (_nullCase == "0")
                        {
                            _value = "9999-12-31";
                        }
                        else
                        {
                            _value = "ξ不做校验ξ";//这是标志字符串，如果包含此字符串则跳出验证
                        }
                    }

                    _leftValue = _leftValue.Replace(_old.ToLower(), _value);
                    _rightValue = _rightValue.Replace(_old.ToLower(), _value);
                }

                _old = gridTbId + ".v" + i.ToString() + ".length";
                if (_leftValue.Contains(_old.ToLower()) || _rightValue.Contains(_old.ToLower()))
                {
                    _indexDataType = GetIndexDataType(dsindex, i.ToString());
                    _value = dr["v" + i.ToString()].ToString().Trim().Length.ToString();
                    _leftValue = _leftValue.Replace(_old.ToLower(), _value);
                    _rightValue = _rightValue.Replace(_old.ToLower(), _value);
                }

                _old = "v" + i.ToString() + ".length";
                if (_leftValue.Contains(_old.ToLower()) || _rightValue.Contains(_old.ToLower()))
                {
                    _indexDataType = GetIndexDataType(dsindex, i.ToString());
                    _value = dr["v" + i.ToString()].ToString().Trim().Length.ToString();
                    _leftValue = _leftValue.Replace(_old.ToLower(), _value);
                    _rightValue = _rightValue.Replace(_old.ToLower(), _value);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dsAllValue"></param>
        /// <param name="sTbId"></param>
        /// <param name="sIndexId">第几列</param>
        /// <param name="sRowsNum">第几行</param>
        /// <param name="sIndexFlag"></param>
        /// <returns></returns>
        private static string GetLength(DataTable dsAllValue, string sTbId, string sIndexId, string sRowsNum)
        {
            string value = "0";
            foreach (DataRow dr in dsAllValue.Rows)
            {
                if (dr["flag"].ToString().ToLower().StartsWith("#" + sTbId + "#") && dr["flag"].ToString().ToLower().EndsWith("#" + sRowsNum + "#"))
                {
                    value = dr["Value2"].ToString().Length.ToString();
                    break;
                }
            }

            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dsAllValue"></param>
        /// <param name="sTbId"></param>
        /// <param name="sIndexId">第几列</param>
        /// <param name="sRowsNum">第几行</param>
        /// <param name="sIndexFlag"></param>
        /// <returns></returns>
        private static string GetValue(DataTable dsAllValue, string sTbId, string sIndexId, string sRowsNum)
        {
            string value = "";
            foreach (DataRow dr in dsAllValue.Rows)
            {
                if (dr["flag"].ToString().ToLower().StartsWith("#" + sTbId + "#") && dr["flag"].ToString().ToLower().EndsWith("#" + sRowsNum + "#"))
                {
                    value = dr["v" + sIndexId].ToString();
                    break;
                }
            }

            return value;
        }

        private static string GetIndexDataType(DataTable dsIndex, string sIndexId)
        {
            string sType = "";
            foreach (DataRow dr in dsIndex.Rows)
            {
                if (dr["indexid"].ToString() == "v" + sIndexId)
                {
                    sType = dr["DataType"].ToString();
                    break;
                }
            }

            return sType;
        }

        private static bool GetIsCheck(string sFlowType, string sFlowStr, string sPrcsNo)
        {
            bool isCheck = false;//是否校验
                                 //自由流程或无流程
            if (sFlowType == "0" || sFlowType == "2")
            {
                if (sFlowStr.Contains(";all;"))
                {
                    isCheck = true;
                }
                else
                {
                    if (sFlowStr.Contains(";" + sPrcsNo + ";"))
                    {
                        isCheck = true;
                    }
                }
            }
            else//固定流程
            {
                //如果包含全部流程的标志字符
                if (sFlowStr.Contains(";all;"))
                {
                    isCheck = true;
                }
                else
                {
                    //如果包含全部流程的标志字符
                    if (sFlowStr.Contains(";" + sPrcsNo + ";"))
                    {
                        isCheck = true;
                    }
                }
            }

            return isCheck;
        }

        private static string ListValidate(DataTable dtlist, string tbid)
        {
            string err = "";
            string sLeft = "";
            string sRight = "";
            string sLeftValue = "";
            string sRightValue = "";
            string sExplain = "";
            string sIOrder = "";
            double dLeft;
            double dRight;
            bool bCompare;
            string compare = "";
            string Original = "";

            try
            {
                foreach (DataRow rcheck in dtlist.Rows)
                {
                    bCompare = false;
                    sIOrder = rcheck["IOrder"].ToString();

                    sLeft = TableHelp.ReplaceOperator(rcheck["LeftValue"].ToString().ToLower());
                    sRight = TableHelp.ReplaceOperator(rcheck["RightValue"].ToString().ToLower());

                    sExplain = rcheck["Explain"].ToString();
                    compare = GeCompareStr(rcheck["Compare"].ToString().Trim());
                    Original = rcheck["Original"].ToString(); ;

                    if (sLeft.Contains("ξ不做校验ξ") || sRight.Contains("ξ不做校验ξ"))
                    {
                        //不做校验
                        bCompare = true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(compare))
                        {
                            err += sExplain + "使用了不可识别的比较符" + rcheck["Compare"].ToString();
                        }
                        else
                        {
                            //从sql里进行运算 很方便健壮
                            if (sLeftValue.ToLower().StartsWith("select "))
                            {
                                sLeftValue = comService.GetSingleField(sLeft);
                            }
                            else
                            {
                                sLeftValue = comService.GetSingleField("select " + sLeft);
                            }

                            if (sRightValue.ToLower().StartsWith("select "))
                            {
                                sRightValue = comService.GetSingleField(sRight);
                            }
                            else
                            {
                                sRightValue = comService.GetSingleField("select " + sRight);
                            }

                            object obj = _table.Compute(sLeftValue + compare + sRightValue, null);
                            if (!Convert.ToBoolean(obj))
                            {
                                //err += sExplain + "，原始表达式为：" + Original + "，转化后的表达式为：" + sLeft + compare + sRight;
                                err += sExplain;
                                break;//
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err += "录入表为" + tbid + "，序号为" + sIOrder + "，解释为" + sExplain + "的校验式异常，不能完成校验，原始表达式为：" + Original + "，转化后的表达式为：" + sLeft + compare + sRight + " 错误信息：" + ex.Message;
            }

            return err;
        }

        private static DataTable NewDataTableCheck()
        {
            DataTable dt = new DataTable("TbCheck");
            dt.Columns.Add("LeftValue", typeof(String));
            dt.Columns.Add("Compare", typeof(String));
            dt.Columns.Add("RightValue", typeof(String));
            dt.Columns.Add("Explain", typeof(String));
            dt.Columns.Add("IOrder", typeof(String));
            dt.Columns.Add("RowsNum", typeof(String));//第几行
            dt.Columns.Add("FlowStr", typeof(String));//适用流程
            dt.Columns.Add("Original", typeof(String));//原始表达式

            return dt;
        }

        private static string GetNum(string flag)
        {
            string[] arr = BaseUtil.GetStrArray(flag, "#");
            return arr[2].ToString();
        }

        private static string GeCompareStr(string flag)
        {
            switch (flag.ToString().Trim().ToLower())
            {
                case "等于":
                    return " = ";

                case "大于":
                    return " > ";

                case "大于等于":
                    return " >= ";

                case "小于":
                    return " < ";

                case "小于等于":
                    return " <= ";

                case "不等于":
                    return " != ";

                default:
                    return "";
            }
        }
    }
}