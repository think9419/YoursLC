using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public partial class PageCom
    {
        private static string split = Think9.Services.Base.BaseConfig.ComSplit;//字符分割 用于多选项的分割等
        private static string _dbtype = Think9.Services.Base.Configs.GetDBProvider("DBProvider");//数据库类型
        private static ComService comService = new ComService();

        public PageCom()
        {
        }

        public static long GeListIdByHttpRequest(string maintbid, Microsoft.AspNetCore.Http.HttpRequest request)
        {
            long listid = -100;
            string where = "";
            string dataType = "";
            string indexId = "";

            DataTable dtFieldList = comService.GetTbFieldsList(maintbid);
            foreach (DataRow row in dtFieldList.Rows)
            {
                indexId = row["COLUMN_NAME"].ToString();
                dataType = row["DATA_type"].ToString();
                if (!string.IsNullOrEmpty(request.Query[indexId]))
                {
                    // todo 还需补全
                    if (dataType == "long" || dataType.Contains("int") || dataType.Contains("decimal") || dataType.Contains("numeric") || dataType.Contains("real") || dataType.Contains("float"))
                    {
                        if (where == "")
                        {
                            where += indexId + " = " + request.Query[indexId].ToString() + "";
                        }
                        else
                        {
                            where += " AND " + indexId + " = " + request.Query[indexId].ToString() + "";
                        }
                    }
                    else
                    {
                        if (where == "")
                        {
                            where += indexId + " = '" + request.Query[indexId].ToString() + "'";
                        }
                        else
                        {
                            where += " AND " + indexId + " = '" + request.Query[indexId].ToString() + "'";
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                DataTable dt = comService.GetDataTable(maintbid, "listid", where, "");
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        listid = long.Parse(dt.Rows[0]["listid"].ToString());
                    }
                }
                else
                {
                    listid = 0;
                }
            }

            return listid;
        }

        /// <summary>
        /// 返回待接手where
        /// </summary>
        /// <param name="user">当前用户信息</param>
        /// <param name="key"></param>
        /// <param name="fwid"></param>
        /// <returns></returns>
        public static string GetManageWhere(CurrentUserEntity user, string fwid, string key)
        {
            string where = "where FlowId='" + fwid + "' ";
            string Account = user.Account == null ? ";!NullEx;" : ";" + user.Account + ";";
            string DeptNo = user.DeptNo == null ? ";!NullEx;" : "." + user.DeptNo + ".";
            string RoleNo = user.RoleNo == null ? ";!NullEx;" : ";" + user.RoleNo + ";";

            string isall = "2";//是否可以管理所有数据？1可以2不可以--只能管理本部门及下级部门
            string ManageUser = comService.GetSingleField("select ManageUser from Flow where FlowId = '" + fwid + "'");
            if (ManageUser.Contains(Account) || ManageUser.Trim() == "#all#")
            {
                isall = "1";
            }
            if (isall != "1")
            {
                where += "  and (createDeptStr like'%" + DeptNo + "%')";
            }

            if (!string.IsNullOrEmpty(key))
            {
                where += "  and (runName like'%" + key + "%' or ruNumber like'%" + key + "%'   ) ";
            }

            return where;
        }

        /// <summary>
        /// 返回待接手where
        /// </summary>
        /// <param name="user">当前用户信息</param>
        /// <param name="key"></param>
        /// <param name="fwid"></param>
        /// <returns></returns>
        public static string GetManageWhere2(CurrentUserEntity user, string fwid, string key)
        {
            string where = "where 1=1 ";
            string Account = user.Account == null ? ";!NullEx;" : ";" + user.Account + ";";
            string DeptNo = user.DeptNo == null ? ";!NullEx;" : "." + user.DeptNo + ".";
            string RoleNo = user.RoleNo == null ? ";!NullEx;" : ";" + user.RoleNo + ";";

            string isAll = "2";//是否可以管理所有数据？1可以2不可以--只能管理本部门及下级部门
            string ManageUser = comService.GetSingleField("select ManageUser from Flow where FlowId = '" + fwid + "'");
            if (ManageUser.Contains(Account) || ManageUser.Trim() == "#all#")
            {
                isAll = "1";
            }

            if (isAll != "1")
            {
                where += "  and (createDeptStr like'%" + DeptNo + "%')";
            }

            if (!string.IsNullOrEmpty(key))
            {
                where += "  and (runName like'%" + key + "%') ";
            }

            return where;
        }

        /// <summary>
        ///添加一条空数据
        /// </summary>
        /// <param name="maintbid">录入表主表编码或者多表的主表编码</param>
        /// <param name="flowid">0独立模式不与flowrunlist关联</param>
        /// <returns></returns>
        public static long InsertEmptyReturnID(ref string err, string flowid, string flowname, CurrentUserEntity user, CurrentPrcsEntity mCurrentPrcs)
        {
            long listid = 0;
            err = "";
            string maintbid = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_");

            if (user != null)
            {
                string userid = user.Account;
                string deptno = user.DeptNo;
                string depnostr = user.DeptNoStr;
                //基础信息表 listid自增长
                if (flowid.StartsWith("bi_"))
                {
                    List<string> columns = new List<string>();
                    columns.Add("isLock");
                    columns.Add("createTime");
                    columns.Add("createUser");
                    columns.Add("createDept");
                    columns.Add("createDeptStr");
                    columns.Add("runName");
                    columns.Add("state");

                    object param = new { isLock = 0, state = 1, createTime = DateTime.Now.ToString(), createUser = userid, createDept = deptno, createDeptStr = depnostr, runName = BaseUtil.GetRunName(userid, flowid, flowname) };

                    listid = comService.InsertAndReturnID(maintbid, columns, param);

                    Record.Add("system", listid.ToString(), flowid, "#主表新增空数据##N{1}#");
                }
                else
                {
                    listid = FlowRunList.InsertFlowrunListReturnID(ref err, maintbid, flowid, flowname, mCurrentPrcs, user);
                    if (listid > 0)
                    {
                        List<string> columns = new List<string>();

                        columns.Add("listid");
                        columns.Add("state");

                        object param = new { listid = listid, state = 1 };

                        comService.Insert(maintbid, columns, param);

                        Record.Add("system", listid.ToString(), flowid, "#主表新增空数据##N{1}#");
                    }
                }
            }

            return listid;
        }

        public static long InsertEmptyReturnID(ref string err, string flowid, string flowname, CurrentUserEntity user, CurrentPrcsEntity mCurrentPrcs, object param)
        {
            long listid = 0;
            err = "";
            string maintbid = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            List<string> columns = new List<string>();

            if (user != null)
            {
                //基础信息表 listid自增长
                if (flowid.StartsWith("bi_"))
                {
                    columns.Add("isLock");
                    columns.Add("createTime");
                    columns.Add("createUser");
                    columns.Add("createDept");
                    columns.Add("createDeptStr");
                    columns.Add("runName");
                    columns.Add("state");
                    //隐藏指标
                    foreach (DataRow drIndex in comService.GetDataTable("select IndexId as id from tbindex where TbId='" + maintbid + "' and isShow='2'").Rows)
                    {
                        columns.Add(drIndex["id"].ToString());
                    }

                    listid = comService.InsertAndReturnID(maintbid, columns, param);

                    Record.Add("system", listid.ToString(), flowid, "#主表新增空数据##N{1}#");
                }
                else
                {
                    columns.Add("listid");
                    columns.Add("state");
                    //隐藏指标
                    foreach (DataRow drIndex in comService.GetDataTable("select IndexId as id from tbindex where TbId='" + maintbid + "' and isShow='2'").Rows)
                    {
                        columns.Add(drIndex["id"].ToString());
                    }

                    comService.Insert(maintbid, columns, param);

                    Record.Add("system", listid.ToString(), flowid, "#主表新增空数据##N{1}#");
                }
            }

            return listid;
        }

        public static TbBasicEntity GetDetailButon(string listid, string fwid, string isOriginal = "n")
        {
            TbBasicEntity model = new TbBasicEntity();

            string txt = "";
            string warn = "";
            string id = "";
            DataTable dt = comService.GetDataTable("select * from tbbut where TbID='" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "'");

            txt = "附件";
            warn = "";
            id = "n";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["BtnId"].ToString() == "attdetails")
                {
                    txt = dr["BtnText"].ToString();
                    warn = dr["BtnWarn"].ToString();
                    id = "y";
                    break;
                }
            }
            model.ButAtt = id;
            model.ButAttWarn = warn;
            if (id == "y")
            {
                string attId = "";
                if (fwid.StartsWith("bi_"))
                {
                    attId = comService.GetSingleField("select attachmentId  FROM " + fwid.Replace("bi_", "tb_") + " WHERE listid= " + ListIdService.GetOriginalListId(listid, isOriginal).ToString());
                }
                else
                {
                    attId = comService.GetSingleField("select attachmentId  FROM flowrunlist WHERE listid= " + ListIdService.GetOriginalListId(listid, isOriginal).ToString());
                }

                if (string.IsNullOrEmpty(attId))
                {
                    model.ButAttTxt = txt + "(0)";
                }
                else
                {
                    model.ButAttTxt = txt + "(" + comService.GetSingleField("select count(*) FROM flowattachment WHERE attachmentId='" + attId + "' ") + ")";
                }
            }

            txt = "打开PDF";
            id = "n";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["BtnId"].ToString() == "pdfdetails")
                {
                    txt = dr["BtnText"].ToString();
                    id = "y";
                    break;
                }
            }
            model.ButPDFDetails = id;
            model.ButPDFDetailsTxt = txt;

            txt = "导出EXCEL";
            id = "n";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["BtnId"].ToString() == "exceldetails")
                {
                    txt = dr["BtnText"].ToString();
                    id = "y";
                    break;
                }
            }
            model.ButExcelDetails = id;
            model.ButExcelDetailsTxt = txt;

            txt = "导出DOC";
            id = "n";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["BtnId"].ToString() == "docdetails")
                {
                    txt = dr["BtnText"].ToString();
                    id = "y";
                    break;
                }
            }
            model.ButDOCDetails = id;
            model.ButDOCDetailsTxt = txt;

            return model;
        }

        private static List<ModelRule> GetRuleModelListByTbID(string tbid, DataTable dtindex, string _dbtype)
        {
            DataTable dtrulesingle = comService.GetDataTable("rulesingle", "*", "", "");
            DataTable dtrulemultiple = comService.GetDataTable("rulemultiple", "*", "", "");
            DataTable dtrulemultiplefiled = comService.GetDataTable("rulemultiplefiled", "*", "", "");
            DataTable dtruletree = comService.GetDataTable("ruletree", "*", "", "");

            List<ModelRule> mList = new List<ModelRule>();

            string _where = "";

            string _order = "";
            string _order2 = "";

            string _select = "";
            string _select2 = "";

            string _from = "";
            string _from_PG = "";
            string _from_ORACLE = "";

            string sql_list = "";
            string sql_list_PG = "";
            string sql_list_ORACLE = "";

            string sql_form = "";
            string sql_form_PG = "";
            string sql_form_ORACLE = "";

            string sControlType = "";
            foreach (DataRow row in dtindex.Rows)
            {
                if (row["RuleId"].ToString() != "")
                {
                    List<IdValueModel> list = new List<IdValueModel>();
                    ModelRule model = new ModelRule();

                    //1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片
                    sControlType = "1";
                    if (row["ControlType"].ToString().Trim() != "")
                    {
                        sControlType = row["ControlType"].ToString();
                    }
                    sControlType = sControlType == "" ? "1" : sControlType;

                    model.TbId = row["TbId"].ToString();
                    model.IndexId = row["IndexId"].ToString();
                    model.IndexName = row["IndexName"].ToString();
                    model.isSelMuch = row["isSelMuch"].ToString();//是否可多选？1可以2不可以

                    model.ControlType = sControlType;
                    model.DataType = row["DataType"].ToString();
                    model.RuleId = row["RuleId"].ToString();
                    model.RuleType = row["RuleType"].ToString();
                    model.DataType = row["DataType"].ToString();

                    //6单列
                    if (model.RuleType == "6")
                    {
                        foreach (DataRow drule in dtrulesingle.Rows)
                        {
                            if (drule["RuleId"].ToString() == model.RuleId)
                            {
                                model.DbID = drule["DbID"].ToString();
                                model.SearchFiled = drule["RuleBy"].ToString();
                                model.ValueFiled = drule["ValueFiled"].ToString();//值字段
                                model.TxtFiled = drule["TxtFiled"].ToString();//显示字段
                                model.OrderFiled = drule["OrderFiled"].ToString();//排序字段
                                model.ValueScope = drule["ValueScope"].ToString();//1固定取值2动态取值
                                model.FrmTB = drule["TbId"].ToString();//取值数据表
                                model.LimitStr = drule["LimitStr"].ToString().Replace("\\\"", "\"");//筛选条件
                                model.OrderType = drule["OrderType"].ToString();
                                model.DictItemId = drule["DictItemId"].ToString();

                                list.Add(new IdValueModel { ID = "Value", Value = drule["ValueFiled"].ToString(), Text = "" });
                                list.Add(new IdValueModel { ID = "info1", Value = drule["ValueFiled"].ToString(), Text = "编码" });
                                list.Add(new IdValueModel { ID = "info2", Value = drule["TxtFiled"].ToString(), Text = "名称" });
                            }
                        }
                    }

                    //6多列
                    if (model.RuleType == "7")
                    {
                        foreach (DataRow drule in dtrulemultiple.Rows)
                        {
                            if (drule["RuleId"].ToString() == model.RuleId)
                            {
                                model.DbID = drule["DbID"].ToString();
                                model.SearchFiled = drule["RuleBy"].ToString();
                                model.ValueFiled = drule["ValueFiled"].ToString();//值字段
                                model.TxtFiled = drule["TxtFiled"].ToString();//显示字段
                                model.OrderFiled = drule["OrderFiled"].ToString();//排序字段
                                model.ValueScope = drule["ValueScope"].ToString();//1固定取值2动态取值
                                model.FrmTB = drule["TbId"].ToString();//取值数据表
                                model.LimitStr = drule["LimitStr"].ToString().Replace("\\\"", "\"");//筛选条件
                                model.OrderType = drule["OrderType"].ToString();
                                model.DictItemId = drule["DictItemId"].ToString();

                                list.Add(new IdValueModel { ID = "Value", Value = drule["ValueFiled"].ToString(), Text = "" });

                                break;
                            }
                        }

                        int icount = 0;
                        foreach (DataRow drule in dtrulemultiplefiled.Rows)
                        {
                            //注意最多10列
                            if (drule["RuleId"].ToString() == model.RuleId && int.Parse(drule["FiledOrder"].ToString()) > 0 && icount <= 10)
                            {
                                icount++;
                                string _filedValue = drule["FiledValue"].ToString();
                                string _name = TableHelp.GetIndexName(dtindex, model.FrmTB, _filedValue);
                                list.Add(new IdValueModel { ID = "info" + icount.ToString(), Value = _filedValue, Text = _name });
                            }
                        }
                    }

                    //树型
                    if (model.RuleType == "8")
                    {
                        foreach (DataRow drule in dtruletree.Rows)
                        {
                            if (drule["RuleId"].ToString() == model.RuleId)
                            {
                                model.DbID = drule["DbID"].ToString();
                                model.SearchFiled = drule["RuleBy"].ToString();
                                model.ValueFiled = drule["ValueFiled"].ToString();//值字段
                                model.TxtFiled = drule["TxtFiled"].ToString();//显示字段
                                model.OrderFiled = drule["OrderFiled"].ToString();//排序字段
                                model.ValueScope = drule["ValueScope"].ToString();//1固定取值2动态取值
                                model.FrmTB = drule["TbId"].ToString();//取值数据表
                                model.LimitStr = drule["LimitStr"].ToString().Replace("\\\"", "\"");//筛选条件
                                model.OrderType = drule["OrderType"].ToString();
                                model.DictItemId = drule["DictItemId"].ToString();

                                list.Add(new IdValueModel { ID = "Value", Value = drule["ValueFiled"].ToString(), Text = "" });
                                list.Add(new IdValueModel { ID = "info1", Value = drule["ValueFiled"].ToString(), Text = "编码" });
                                list.Add(new IdValueModel { ID = "info2", Value = drule["TxtFiled"].ToString(), Text = "名称" });

                                break;
                            }
                        }
                    }

                    //树选
                    if (model.RuleType == "8")
                    {
                        _select = model.ValueFiled + " as id," + model.TxtFiled + " as name";
                        _select2 = "\"" + model.ValueFiled + "\"" + " as id," + "\"" + model.TxtFiled + "\"" + " as name";
                        if (model.ValueScope == "1")
                        {
                            _where = "ItemCode='" + model.DictItemId + "'";

                            _from = "sys_itemsdetail";
                            _from_PG = "\"sys_itemsdetail\"";
                            _from_ORACLE = "\"sys_itemsdetail\"";

                            _order = "LENGTH(DetailCode), OrderNo";
                            _order2 = "LENGTH(DetailCode), OrderNo";
                            if (_dbtype != "mysql")
                            {
                                _order = "LEN(DetailCode), OrderNo";
                                _order2 = "LEN(DetailCode), OrderNo";
                            }
                            if (model.OrderType == "2")
                            {
                                _order += " desc";
                                _order2 += " desc";
                            }

                            sql_list = "select " + _select + " FROM " + _from + " where " + _where + " ORDER BY " + _order;
                            sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " ORDER BY " + _order2;
                            sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " ORDER BY " + _order2;

                            sql_form = "select " + _select + " FROM " + _from + " where " + _where + " ORDER BY " + _order;
                            sql_form_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " ORDER BY " + _order2;
                            sql_form_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " ORDER BY " + _order2;
                        }
                        else
                        {
                            _from = model.FrmTB;
                            _from_PG = "\"" + model.FrmTB + "\"";
                            _from_ORACLE = "\"" + model.FrmTB + "\"";

                            _where = " ";

                            _order = "LENGTH(" + model.ValueFiled + ") ";
                            _order2 = "LENGTH(" + model.ValueFiled + ") ";
                            if (_dbtype != "mysql")
                            {
                                _order = "LEN(" + model.ValueFiled + ") ";
                                _order2 = "LEN(" + model.ValueFiled + ") ";
                            }

                            if (model.OrderFiled.Trim() != "")
                            {
                                _order += " ," + model.OrderFiled.Trim();
                                _order2 += " ," + model.OrderFiled.Trim();
                                if (model.OrderType == "2")
                                {
                                    _order += " desc";
                                    _order2 += " desc";
                                }
                            }

                            sql_list = "select " + _select + " FROM " + _from + " ORDER BY " + _order;
                            sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " ORDER BY " + _order2;
                            sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " ORDER BY " + _order2;

                            sql_form = "select " + _select + " FROM " + _from + " ORDER BY " + _order;
                            sql_form_PG = "select " + _select2 + " FROM " + _from_PG + " ORDER BY " + _order2;
                            sql_form_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " ORDER BY " + _order2;
                        }
                    }
                    else
                    {
                        _select = model.ValueFiled + " as id," + model.TxtFiled + " as name";
                        _select2 = "\"" + model.ValueFiled + "\"" + " as id," + "\"" + model.TxtFiled + "\"" + " as name";
                        if (model.ValueScope == "1")
                        {
                            _from = "sys_itemsdetail";
                            _from_PG = "\"sys_itemsdetail\"";
                            _from_ORACLE = "\"sys_itemsdetail\"";

                            _where = "ItemCode = '" + model.DictItemId + "'";
                            _order = "OrderNo";
                            if (model.OrderType == "2")
                            {
                                _order += " desc";
                            }

                            if (!string.IsNullOrEmpty(model.LimitStr))
                            {
                                sql_form = "select " + _select + " FROM " + _from + " where " + _where + " AND " + model.LimitStr + " order by " + _order;
                                sql_form_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " AND " + model.LimitStr + " order by " + _order2;
                                sql_form_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " AND " + model.LimitStr + " order by " + _order2;
                                //包含系统参数
                                if (model.LimitStr.Trim().Contains("@"))
                                {
                                    sql_list = "select " + _select + " FROM " + _from + " where " + _where + " order by " + _order;
                                    sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " order by " + _order2;
                                    sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " order by " + _order2;
                                }
                                else
                                {
                                    sql_list = "select " + _select + " FROM " + _from + " where " + _where + " AND " + model.LimitStr + " order by " + _order;
                                    sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " AND " + model.LimitStr + " order by " + _order2;
                                    sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " AND " + model.LimitStr + " order by " + _order2;
                                }
                            }
                            else
                            {
                                sql_list = "select " + _select + " FROM " + _from + " where " + _where + " order by " + _order;
                                sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " order by " + _order2;
                                sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " order by " + _order2;

                                sql_form = "select " + _select + " FROM " + _from + " where " + _where + " order by " + _order;
                                sql_form_PG = "select " + _select2 + " FROM " + _from_PG + " where " + _where + " order by " + _order2;
                                sql_form_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + _where + " order by " + _order2;
                            }
                        }
                        else
                        {
                            _where = "";

                            _from = model.FrmTB;
                            _from_PG = "\"" + model.FrmTB + "\"";
                            _from_ORACLE = "\"" + model.FrmTB + "\"";

                            _order = model.OrderFiled;
                            _order2 = "\"" + model.OrderFiled + "\"";
                            if (model.OrderType == "2" && _order.Trim() != "")
                            {
                                _order += " desc";
                                _order2 += " desc";
                            }

                            if (!string.IsNullOrEmpty(model.LimitStr))
                            {
                                sql_form = "select " + _select + " FROM " + _from + " where " + model.LimitStr + " order by " + _order;
                                sql_form_PG = "select " + _select2 + " FROM " + _from_PG + " where " + model.LimitStr + " order by " + _order2;
                                sql_form_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + model.LimitStr + " order by " + _order2;

                                //包含系统参数
                                if (model.LimitStr.Trim().Contains("@"))
                                {
                                    sql_list = "select " + _select + " FROM " + _from + " order by " + _order;
                                    sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " order by " + _order2;
                                    sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " order by " + _order2;
                                }
                                else
                                {
                                    sql_list = "select " + _select + " FROM " + _from + " where " + model.LimitStr + " order by " + _order;
                                    sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " where " + model.LimitStr + " order by " + _order2;
                                    sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " where " + model.LimitStr + " order by " + _order2;
                                }
                            }
                            else
                            {
                                sql_list = "select " + _select + " FROM " + _from + " order by " + _order;
                                sql_list_PG = "select " + _select2 + " FROM " + _from_PG + " order by " + _order2;
                                sql_list_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " order by " + _order2;

                                sql_form = "select " + _select + " FROM " + _from + " order by " + _order;
                                sql_form_PG = "select " + _select2 + " FROM " + _from_PG + " order by " + _order2;
                                sql_form_ORACLE = "select " + _select2 + " FROM " + _from_ORACLE + " order by " + _order2;
                            }
                        }
                    }

                    model.SqlForm = sql_form;
                    model.SqlForm_PG = sql_form_PG;
                    model.SqlForm_ORACLE = sql_form_ORACLE;

                    model.SqlList = sql_list;
                    model.SqlList_PG = sql_list_PG;
                    model.SqlList_ORACLE = sql_list_ORACLE;

                    model.list = list;
                    mList.Add(model);
                }
            }

            return mList;
        }
    }
}