using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Think9.Models;
using Think9.Services.Base;
using Think9.Util.Helper;

namespace Think9.Services.Com
{
    /// <summary>
    ///
    /// </summary>
    public class ExcelData
    {
        private ComService comService = new ComService();

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="listidsStr"></param>
        /// <param name="show"></param>
        /// <param name="fwid"></param>
        /// <param name="filename"></param>
        /// <param name="list"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public string ImportDataFromExcel(ref string listidsStr, ref string show, string fwid, string filename, List<InfoListEntity> list, CurrentUserEntity user)
        {
            string err = "";
            string timeBegin = DateTime.Now.ToShortTimeString();
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\TempFile\\");
            string filePath = Path.Combine(uploads, filename);

            DataTable dtIndexColumn = DataTableHelp.ListToDataTable<InfoListEntity>(list);

            List<DataTable> listDt = GetExcelData(filePath, dtIndexColumn, fwid, false, ref err);
            if (!string.IsNullOrEmpty(err))
            {
                return err;
            }

            //是否辅助表？1是2不是
            if (comService.GetSingleField("select isAux  FROM tbbasic  WHERE FlowId = '" + fwid + "' ") == "1")
            {
                DataTable dtResult = ImportDataForAux(ref listidsStr, fwid, user, null, listDt);
                show = "共导入" + dtResult.Rows.Count.ToString() + "条数据";
            }
            else
            {
                //当前流程步骤，基本信息录入表返回空
                CurrentPrcsEntity mPrcs = Think9.Services.Com.FlowCom.GetFistStept(fwid);
                err = mPrcs == null ? "" : mPrcs.ERR;
                if (!string.IsNullOrEmpty(err))
                {
                    return err;
                }
                DataTable dtResult = ImportData(ref listidsStr, fwid, user, mPrcs, listDt);
                show = "共导入" + dtResult.Rows.Count.ToString() + "条数据";
            }

            return err;
        }

        /// <summary>
        /// 非辅助表导入数据
        /// </summary>
        /// <param name="strListid"></param>
        /// <param name="fwid"></param>
        /// <param name="currentUser"></param>
        /// <param name="mPrcs"></param>
        /// <param name="listDt"></param>
        /// <returns></returns>
        private DataTable ImportData(ref string strListid, string fwid, CurrentUserEntity currentUser, CurrentPrcsEntity mPrcs, List<DataTable> listDt)
        {
            List<string> listSql = new List<string>();
            long listid = 0;
            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string tbname = "【导入】";

            string tbColumns = comService.GetTableFieldsString(tbid);

            foreach (DataTable dt in listDt)
            {
                if (dt.Rows.Count > 0)
                {
                    listid = PageCom.InsertEmptyReturnID(ref err, fwid, tbname, currentUser, mPrcs);
                    strListid += "," + listid;
                    if (string.IsNullOrEmpty(err))
                    {
                        string sql = GetUpSql(tbid, listid, tbColumns, dt);
                        listSql.Add(sql);

                        Record.Add(currentUser == null ? "!NullEx" : currentUser.Account, listid.ToString(), fwid, "#数据导入#");
                    }
                }
            }

            DataTable dtResult = comService.ExecuteSql(listSql);

            return dtResult;
        }

        /// <summary>
        /// 辅助表导入数据
        /// </summary>
        /// <param name="strListid"></param>
        /// <param name="fwid"></param>
        /// <param name="currentUser"></param>
        /// <param name="mPrcs"></param>
        /// <param name="listDt"></param>
        /// <returns></returns>
        private DataTable ImportDataForAux(ref string strListid, string fwid, CurrentUserEntity currentUser, CurrentPrcsEntity mPrcs, List<DataTable> listDt)
        {
            List<string> listSql = new List<string>();
            long listid = 0;
            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string tbname = "【导入】";

            string tbColumns = comService.GetTableFieldsString(tbid);

            foreach (DataTable dt in listDt)
            {
                if (dt.Rows.Count > 0)
                {
                    listid = 0;
                    strListid += "," + listid;
                    if (string.IsNullOrEmpty(err))
                    {
                        string sql = GetInsertSql(tbid, tbColumns, dt);
                        listSql.Add(sql);

                        Record.Add(currentUser == null ? "!NullEx" : currentUser.Account, listid.ToString(), tbid, "#数据导入#");
                    }
                }
            }

            DataTable dtResult = comService.ExecuteSql(listSql);

            return dtResult;
        }

        /// <summary>
        /// 返回编辑sql
        /// </summary>
        private string GetUpSql(string tbId, long listid, string tbColumns, DataTable dtValue)
        {
            string indexid = "";
            string dataType = "";
            string value = "";
            int num = 0;
            DateTime dtDate;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update " + tbId + " set ");
            foreach (DataRow row in dtValue.Rows)
            {
                indexid = row["IndexId"].ToString().Trim();
                dataType = row["DataType"].ToString().Trim().Substring(0, 1);//变量类型--1日期2字符3数字4附件5图片
                value = row["NewValue"].ToString().Trim();//值

                //这个很重要！
                if (tbColumns.Contains("#" + indexid + "#"))
                {
                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "1")
                    {
                        //数字
                        if (!string.IsNullOrEmpty(value) && ValidatorHelper.IsNumberic(value))
                        {
                            //decimal d = Convert.ToDecimal(value) - 2;
                            //int day = (int)d;;
                            int day = Convert.ToInt32(Convert.ToDecimal(value)) - 2;
                            value = Convert.ToDateTime("1900-1-1").AddDays(Convert.ToDouble(day)).ToString("yyyy/MM/dd");
                        }

                        if (num == 0)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (DateTime.TryParse(value, out dtDate))
                                {
                                    strSql.Append("" + indexid + "='" + value + "'");
                                }
                                else
                                {
                                    strSql.Append("" + indexid + "= null ");
                                }
                            }
                            else
                            {
                                strSql.Append("" + indexid + "= null ");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (DateTime.TryParse(value, out dtDate))
                                {
                                    strSql.Append("," + indexid + "='" + value + "'");
                                }
                                else
                                {
                                    strSql.Append("," + indexid + "= null ");
                                }
                            }
                            else
                            {
                                strSql.Append("," + indexid + "= null ");
                            }
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "2")
                    {
                        if (num == 0)
                        {
                            strSql.Append("" + indexid + "='" + value + "'");
                        }
                        else
                        {
                            strSql.Append("," + indexid + "='" + value + "'");
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "3")
                    {
                        if (num == 0)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                strSql.Append("" + indexid + "=" + value + "");
                            }
                            else
                            {
                                strSql.Append("" + indexid + "= null ");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                strSql.Append("," + indexid + "=" + value + "");
                            }
                            else
                            {
                                strSql.Append("," + indexid + "= null ");
                            }
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片 zzz20190422
                    if (dataType == "4")
                    {
                        if (num == 0)
                        {
                            strSql.Append("" + indexid + "='" + value + "'");
                        }
                        else
                        {
                            strSql.Append("," + indexid + "='" + value + "'");
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "5")
                    {
                        if (num == 0)
                        {
                            strSql.Append("" + indexid + "='" + value + "'");
                        }
                        else
                        {
                            strSql.Append("," + indexid + "='" + value + "'");
                        }
                        num++;
                    }
                }
            }

            return strSql.ToString() + " where listid=" + listid + " ";
        }

        /// <summary>
        /// 返回插入sql
        /// </summary>
        private string GetInsertSql(string fwId, string tbColumns, DataTable dtValue)
        {
            string tbId = fwId.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string indexid = "";
            string dataType = "";
            string value = "";
            int num = 0;

            StringBuilder sqlField = new StringBuilder();
            StringBuilder sqlSome = new StringBuilder();

            foreach (DataRow row in dtValue.Rows)
            {
                indexid = row["IndexId"].ToString().Trim();
                dataType = row["DataType"].ToString().Trim().Substring(0, 1);//变量类型--1日期2字符3数字4附件5图片
                value = row["NewValue"].ToString().Trim();//值

                //这个很重要！
                if (tbColumns.Contains("#" + indexid + "#"))
                {
                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "1")
                    {
                        if (num == 0)
                        {
                            sqlField.Append("" + indexid + "");
                            sqlSome.Append("'" + value + "'");
                        }
                        else
                        {
                            sqlField.Append("," + indexid + "");
                            sqlSome.Append(",'" + value + "'");
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "2")
                    {
                        if (num == 0)
                        {
                            sqlField.Append("" + indexid + "");
                            sqlSome.Append("'" + value + "'");
                        }
                        else
                        {
                            sqlField.Append("," + indexid + "");
                            sqlSome.Append(",'" + value + "'");
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "3")
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (num == 0)
                            {
                                sqlField.Append("" + indexid + "");
                                sqlSome.Append("" + value + "");
                            }
                            else
                            {
                                sqlField.Append("," + indexid + "");
                                sqlSome.Append("," + value + "");
                            }
                            num++;
                        }
                    }

                    //变量类型--1日期2字符3数字4附件5图片 zzz20190422
                    if (dataType == "4")
                    {
                        if (num == 0)
                        {
                            sqlField.Append("" + indexid + "");
                            sqlSome.Append("'" + value + "'");
                        }
                        else
                        {
                            sqlField.Append("," + indexid + "");
                            sqlSome.Append(",'" + value + "'");
                        }
                        num++;
                    }

                    //变量类型--1日期2字符3数字4附件5图片
                    if (dataType == "5")
                    {
                        if (num == 0)
                        {
                            sqlField.Append("" + indexid + "");
                            sqlSome.Append("'" + value + "'");
                        }
                        else
                        {
                            sqlField.Append("," + indexid + "");
                            sqlSome.Append(",'" + value + "'");
                        }
                        num++;
                    }
                }
            }

            return "INSERT INTO " + tbId + "(" + sqlField.ToString() + ") VALUES (" + sqlSome.ToString() + ");SELECT @@IDENTITY";
        }

        public List<InfoListEntity> GetColumnList(string tbid, string column)
        {
            int num = 0;
            string info1 = "";
            string info2 = "";
            string _index = "";
            string _column = "";
            string str = "";

            List<InfoListEntity> list = new List<InfoListEntity>();

            DataTable dtIndex = comService.GetDataTable("Select * from tbindex where TbId='" + tbid + "' ORDER BY IndexOrderNo");
            foreach (DataRow dr in dtIndex.Rows)
            {
                _index += "<option value='" + dr["IndexId"].ToString() + "'>" + dr["IndexName"].ToString() + "</option>";
            }

            if (column.Replace(",", "") != "")
            {
            }

            string[] arr = BaseUtil.GetStrArray(column, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null && !string.IsNullOrEmpty(arr[i].ToString().Trim()))
                {
                    _column += "<option value='" + arr[i].ToString().Trim().Replace("列", "") + "'>" + arr[i].ToString().Trim() + "</option>";
                }
            }

            foreach (DataRow dr in dtIndex.Rows)
            {
                num++;
                info2 = _index.Replace("<option value='" + dr["IndexId"].ToString() + "'>", "<option value='" + dr["IndexId"].ToString() + "' selected>");

                info1 = _column.Replace("<option value='" + num.ToString() + "'>", "<option value='" + num.ToString() + "' selected>"); ;

                list.Add(new InfoListEntity { info1 = info1, info2 = info2 });
            }

            return list;
        }

        public List<DataTable> GetExcelData(string filePath, DataTable dtIndexColumn, string fwid, bool isAddTop, ref string err)
        {
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string nColumns = "0";
            int j = 0;
            string postion;
            string newValue;
            List<string> listHeader = new List<string>();

            List<DataTable> listDt = new List<DataTable>();

            DataTable dtExcel = ExcelToTable(filePath, listHeader, ref nColumns);

            if (dtExcel == null)
            {
                err = "无法读取数据";
                return null;
            }

            DataTable dtIndex = comService.GetDataTable("Select * from tbindex where TbId='" + tbid + "' ORDER BY IndexOrderNo");

            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {
                bool isAdd = true;
                if (isAddTop && i == 0)
                {
                    isAdd = false;
                }

                if (isAdd)
                {
                    postion = "第" + (i + 1).ToString() + "行";
                    DataTable dtValue = DataTableHelp.NewIndexValueDt();

                    foreach (DataRow drindex in dtIndex.Rows)
                    {
                        DataRow row = dtValue.NewRow();
                        row["IndexId"] = drindex["IndexId"].ToString();
                        row["indexname"] = drindex["IndexName"].ToString();
                        row["Value"] = "";
                        row["NewValue"] = "";
                        row["DataType"] = drindex["DataType"].ToString();//数据类型
                        row["isEmpty"] = drindex["isEmpty"].ToString();//值是否可以为空1可以2不可以
                        row["isPK"] = drindex["isPK"].ToString();//主键？1是2不是
                        row["isUnique"] = drindex["isUnique"].ToString();//唯一？1是2不是
                        row["ControlType"] = drindex["ControlType"].ToString();//
                        row["Validate"] = drindex["ControlBy3"].ToString();//校验验证

                        string sColumn = GetColumn(dtIndexColumn, drindex["IndexId"].ToString()).Trim();
                        if (sColumn == "-1" || sColumn == "")
                        {
                            row["NewValue"] = "";
                        }
                        else
                        {
                            j = int.Parse(sColumn) - 1;
                            newValue = dtExcel.Rows[i][j].ToString().Trim();
                            row["NewValue"] = newValue;
                        }

                        dtValue.Rows.Add(row);
                    }

                    err = CheckExcelDataValue(dtValue, postion);
                    if (string.IsNullOrEmpty(err))
                    {
                        listDt.Add(dtValue);
                    }
                    else
                    {
                        return listDt;
                    }
                }
            }

            return listDt;
        }

        private string GetColumn(DataTable dtIndexColumn, string indexid)
        {
            string nColumn = "-1";

            foreach (DataRow dr in dtIndexColumn.Rows)
            {
                if (indexid == dr["info2"].ToString())
                {
                    nColumn = dr["info1"].ToString();
                }
            }

            return nColumn;
        }

        /// <summary>
        /// 获取DataTable前几条数据
        /// </summary>
        /// <param name="topItem">前N条数据</param>
        /// <param name="oDT">源DataTable</param>
        /// <returns></returns>
        public DataTable DtSelectTop(int topItem, DataTable oDT)
        {
            if (oDT.Rows.Count < topItem) return oDT;

            DataTable NewTable = oDT.Clone();
            DataRow[] rows = oDT.Select("1=1");
            for (int i = 0; i < topItem; i++)
            {
                NewTable.ImportRow((DataRow)rows[i]);
            }
            return NewTable;
        }

        public DataTable ExcelToTable(string filePath, List<string> listHeader, ref string nColumns)
        {
            return NPOIHelper.ExcelToTable(filePath, listHeader, ref nColumns);
        }

        public string GetGridTableHtml(DataTable dtExcel, List<string> listHeader, int columns)
        {
            StringPlus str = new StringPlus();
            string cell = "文本框";
            string ColumnWith = "100";

            str.AppendSpaceLine(3, "<table class=\"layui-table\">");

            str.AppendSpaceLine(4, "<colgroup>");
            foreach (string head in listHeader)
            {
                str.AppendSpaceLine(5, "<col width=\"" + ColumnWith + "\">");
            }

            str.AppendSpaceLine(4, "</colgroup>");

            str.AppendSpaceLine(4, "<thead><tr>");
            foreach (string head in listHeader)
            {
                str.AppendSpaceLine(5, "<th>" + head + "</th>");
            }

            str.AppendSpaceLine(4, "</tr></thead>");

            str.AppendSpaceLine(4, "<tbody>");
            foreach (DataRow dr in dtExcel.Rows)
            {
                str.AppendSpaceLine(4, "<tr>");
                for (int i = 0; i < columns; i++)
                {
                    cell = dr[i].ToString();
                    str.AppendSpaceLine(5, "<th>" + cell + "</th>");
                }
                str.AppendSpaceLine(4, "</tr>");
            }

            str.AppendSpaceLine(4, "</tbody>");

            str.AppendSpaceLine(3, "</table>");

            return str.ToString().TrimEnd();
        }

        private string CheckExcelDataValue(DataTable dsExcel, string sRow)
        {
            string err = "";
            bool isCheck = false;
            decimal maxd1 = (decimal)Math.Pow(10, 17) + 1;//最大最小值暂定为17次方
            decimal maxd2 = -1 * maxd1;
            int maxLen;

            //还没加上日期验证
            //标志字符串-- 1 变量类型--1日期2字符3数字4附件5图片
            foreach (DataRow dr in dsExcel.Rows)
            {
                //进行不能为空验证
                if (dr["isEmpty"].ToString() == "2" && dr["NewValue"].ToString().Trim() == "")
                {
                    err += "请输入：" + sRow + dr["IndexName"].ToString() + " ";
                }

                //进行是否数字和数字区间值校验
                if (dr["DataType"].ToString().Substring(0, 1) == "3" && dr["NewValue"].ToString().Trim() != "")
                {
                    isCheck = ValidatorHelper.IsNumberic(dr["NewValue"].ToString().Trim());
                    if (!isCheck)
                    {
                        err += sRow + dr["IndexName"].ToString() + "{" + dr["NewValue"].ToString().Trim() + "}不是数字 ";
                    }
                    //区间值的检测
                    else
                    {
                        if (dr["NewValue"].ToString().Trim().Length > 20)
                        {
                            err += sRow + dr["IndexName"].ToString() + "{" + dr["NewValue"].ToString().Trim() + "}过长 ";
                        }
                        else
                        {
                            if (decimal.Parse(dr["NewValue"].ToString().Trim()) > maxd1 || decimal.Parse(dr["NewValue"].ToString().Trim()) < maxd2)
                            {
                                err += sRow + dr["IndexName"].ToString() + "{" + dr["NewValue"].ToString().Trim() + "}太大或者太小 ";
                            }
                        }
                    }
                }

                //字符验证
                if (dr["DataType"].ToString().Substring(0, 1) == "2" && dr["NewValue"].ToString().Trim() != "")
                {
                    maxLen = BaseUtil.GetDataMlenByDataType(dr["DataType"].ToString());
                    if (maxLen != 0)
                    {
                        if (dr["NewValue"].ToString().Trim().Length > maxLen)
                        {
                            err += sRow + dr["IndexName"].ToString() + "{" + dr["NewValue"].ToString().Trim() + "}超过最大字符限定 ";
                        }
                    }
                }

                //日期验证 ValidatorHelper.IsDateTime(sDefaultV)
                if (dr["DataType"].ToString().Substring(0, 1) == "1" && dr["NewValue"].ToString().Trim() != "")
                {
                    //不是日期同时还不是数字
                    isCheck = ValidatorHelper.IsNumberic(dr["NewValue"].ToString().Trim());
                    if (!ValidatorHelper.IsNumberic(dr["NewValue"].ToString().Trim()) && !ValidatorHelper.IsDateTime(dr["NewValue"].ToString().Trim()))
                    {
                        //err += sRow + dr["IndexName"].ToString() + "{" + dr["NewValue"].ToString().Trim() + "}不是有效日期格式  ";
                    }
                }
            }

            return err;
        }
    }
}