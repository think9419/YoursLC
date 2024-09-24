using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public partial class PageCom
    {
        public static string GetUpdateFieldsStr(CurrentUserEntity user, string tbid, string fields)
        {
            string hiddenindex = GetHiddenIndexStr(user, tbid);
            if (hiddenindex.Trim() == "")
            {
                return fields;
            }

            int num = 0;
            string updateFieldsStr = "";
            string[] arr = BaseUtil.GetStrArray(fields, ",");
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (!hiddenindex.Contains(";" + arr[i].ToString().Trim() + ";"))
                {
                    if (num == 0)
                    {
                        updateFieldsStr += arr[i].ToString().Trim();
                    }
                    else
                    {
                        updateFieldsStr += "," + arr[i].ToString().Trim();
                    }
                    num++;
                }
            }
            return updateFieldsStr.Trim();
        }

        public static string GetHiddenIndexStr(CurrentUserEntity user, string tbid, string isHidden = "")
        {
            if (user == null)
            {
                return "";
            }

            string account = string.IsNullOrEmpty(user.Account) ? ";!NullEx;" : ";" + user.Account + ";";
            string deptNo = string.IsNullOrEmpty(user.DeptNo) ? ";!NullEx;" : ";" + user.DeptNo + ";";
            string roleNo = string.IsNullOrEmpty(user.RoleNo) ? ";!NullEx;" : ";" + user.RoleNo + ";";
            ComService comService = new ComService();

            string hiddenindex = "";
            string sql = "select * from tbhiddenindex WHERE (TbId='" + tbid + "')";
            if (!string.IsNullOrEmpty(isHidden))
            {
                sql = "select * from tbhiddenindex WHERE (TbId='" + tbid + "' AND isHidden='" + isHidden + "')";
            }
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                //对象类别 1组织机构 2角色 3用户 4岗位
                if (dr["ObjType"].ToString() == "1" && dr["ObjId"].ToString().Contains(deptNo))
                {
                    hiddenindex += dr["IndexId"].ToString().Trim();
                }
                if (dr["ObjType"].ToString() == "2" && dr["ObjId"].ToString().Contains(roleNo))
                {
                    hiddenindex += dr["IndexId"].ToString().Trim();
                }
                if (dr["ObjType"].ToString() == "3" && dr["ObjId"].ToString().Contains(account))
                {
                    hiddenindex += dr["IndexId"].ToString().Trim();
                }
            }

            return hiddenindex;
        }

        public static DataTable GetMainTbDt(ref string err, string fwid, string listid, string hostUrl, string pathUserImg, string pathImgNoExist, CurrentUserEntity user = null)
        {
            string temp = "";
            string maintbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            DataTable dtExternalDb = comService.GetDataTable("select * from externaldb");
            DataTable dtReturn = new DataTable(maintbid);

            string hiddenIndex = "";//保密字段
            string flowType = comService.GetSingleField("select FlowType from flow where FlowId='" + fwid + "'");
            //固定流程
            if (flowType == "1")
            {
                DataTable _dtlist = comService.GetDataTable("select flowrunlist.listid AS listid,flowrunlist.currentPrcsId AS currentPrcsId,flowprcs.HiddenIndex AS HiddenIndex from (flowrunlist join flowprcs on((flowrunlist.currentPrcsId = flowprcs.PrcsId))) where flowrunlist.listid=" + listid + " ");
                if (_dtlist.Rows.Count > 0)
                {
                    hiddenIndex = _dtlist.Rows[0]["HiddenIndex"].ToString();
                }
            }
            else
            {
                hiddenIndex = GetHiddenIndexStr(user, maintbid, "y");
            }

            try
            {
                DataTable dtindex = comService.GetDataTable("tbindex", "*", "TbId='" + maintbid + "'", "");

                dtReturn.Columns.Add("listid", typeof(String));
                foreach (DataRow row in dtindex.Rows)
                {
                    dtReturn.Columns.Add(row["IndexId"].ToString(), typeof(String));
                    //附件或者图片
                    if (row["DataType"].ToString().StartsWith("4") || row["DataType"].ToString().StartsWith("5"))
                    {
                        dtReturn.Columns.Add(row["IndexId"].ToString() + "_Exa", typeof(String));
                    }
                }

                DataTable dtMain = comService.GetDataTable(maintbid, "*", "where listid=" + listid + "", "");
                if (dtMain.Rows.Count > 0)
                {
                    DataTable dtValueText = DataTableHelp.NewValueTextDt();

                    List<ModelRule> listRule = GetRuleModelListByTbID(maintbid, dtindex, _dbtype);
                    foreach (ModelRule obj in listRule)
                    {
                        if (obj.ControlType == "2" || obj.ControlType == "3" || obj.ControlType == "4")
                        {
                            if (!string.IsNullOrEmpty(obj.SqlList))
                            {
                                DataTable dt = new DataTable();
                                if (obj.DbID == "0")
                                {
                                    dt = comService.GetDataTable(obj.SqlList);
                                }
                                else
                                {
                                    string dbType = "";
                                    string dbCon = "";
                                    string sql = obj.SqlList;
                                    foreach (DataRow dr in dtExternalDb.Rows)
                                    {
                                        if (dr["DbID"].ToString() == obj.DbID)
                                        {
                                            dbType = dr["DbType"].ToString();
                                            dbCon = dr["DbCon"].ToString();
                                            if (dbType == "postgresql")
                                            {
                                                sql = obj.SqlList_PG;
                                            }
                                            if (dbType == "oracle")
                                            {
                                                sql = obj.SqlList_ORACLE;
                                            }
                                            break;
                                        }
                                    }
                                    dt = comService.GetDataTable(dbCon, dbType, sql);
                                }
                                foreach (DataRow dr in dt.Rows)
                                {
                                    DataRow row = dtValueText.NewRow();
                                    row["ClassID"] = obj.IndexId;
                                    row["Value"] = dr["id"].ToString();
                                    row["Text"] = dr["name"].ToString();
                                    dtValueText.Rows.Add(row);
                                }
                            }
                        }
                    }

                    DataRow rowDB = dtReturn.NewRow();
                    foreach (DataRow row in dtindex.Rows)
                    {
                        string sIndexId = row["IndexId"].ToString();
                        string sIndexName = row["IndexName"].ToString();
                        string sDataType = row["DataType"].ToString();
                        string sNO = row["IndexNo"].ToString();//序号
                                                               //1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片
                        string sControlType = row["ControlType"].ToString();
                        sControlType = sControlType == "" ? "1" : sControlType;

                        //保密字段
                        if (hiddenIndex.Contains(";" + sIndexId + ";"))
                        {
                            rowDB[sIndexId] = "******";
                        }
                        else
                        {
                            if (sDataType.StartsWith("1"))//日期
                            {
                                temp = dtMain.Rows[0][sIndexId].ToString().Trim();
                                if (row["isTime"].ToString() == "1")
                                {
                                    rowDB[sIndexId] = temp;
                                }
                                else
                                {
                                    rowDB[sIndexId] = temp;
                                    if (temp != "")
                                    {
                                        rowDB[sIndexId] = DateTime.Parse(temp).ToString("yyyy-MM-dd");
                                    }
                                }
                            }

                            if (sDataType.StartsWith("2"))//字符型
                            {
                                temp = dtMain.Rows[0][sIndexId].ToString();
                                rowDB[sIndexId] = temp;

                                //select下拉选择
                                if (sControlType == "2")
                                {
                                    string strV = "";
                                    foreach (DataRow drVT in dtValueText.Rows)
                                    {
                                        if (drVT["Value"].ToString() == temp && drVT["ClassID"].ToString() == sIndexId)
                                        {
                                            strV = drVT["Text"].ToString();
                                            break;
                                        }
                                    }
                                    rowDB[sIndexId] = strV;
                                }

                                //checkbox复选框
                                if (sControlType == "3")
                                {
                                    string strV = "";
                                    foreach (DataRow drVT in dtValueText.Rows)
                                    {
                                        if (drVT["ClassID"].ToString() == sIndexId)
                                        {
                                            if (temp.Contains(split + drVT["Value"].ToString() + split))
                                            {
                                                strV += "【√】" + drVT["Text"].ToString() + " ";
                                            }
                                            else
                                            {
                                                strV += "〔 〕" + drVT["Text"].ToString() + " ";
                                            }
                                        }
                                    }

                                    rowDB[sIndexId] = strV;
                                }

                                //radio单选框〔 〕◯（●）（○）
                                if (sControlType == "4")
                                {
                                    string strV = "";
                                    foreach (DataRow drVT in dtValueText.Rows)
                                    {
                                        if (drVT["ClassID"].ToString() == sIndexId)
                                        {
                                            if (drVT["Value"].ToString() == temp)
                                            {
                                                strV += "【√】" + drVT["Text"].ToString() + " ";
                                            }
                                            else
                                            {
                                                strV += "〔 〕" + drVT["Text"].ToString() + " ";
                                            }
                                        }
                                    }
                                    rowDB[sIndexId] = strV;
                                }
                            }

                            if (sDataType.StartsWith("3"))//数字
                            {
                                temp = dtMain.Rows[0][sIndexId].ToString().Trim();
                                rowDB[sIndexId] = temp;
                                //金额写法
                                if (sDataType == "3102" && temp != "")
                                {
                                    rowDB[sIndexId] = "¥" + temp;
                                }
                            }

                            if (sDataType.StartsWith("4"))//附件
                            {
                                temp = dtMain.Rows[0][sIndexId].ToString().Trim();
                                rowDB[sIndexId] = temp;

                                if (temp != "")
                                {
                                    if (!string.IsNullOrEmpty(hostUrl))
                                    {
                                        //附件链接
                                        rowDB[sIndexId + "_Exa"] = hostUrl + "/UserFile/" + maintbid + "_Files/" + listid + "/" + dtMain.Rows[0][sIndexId].ToString().Trim();
                                    }
                                }
                            }

                            if (sDataType.StartsWith("5"))//图片
                            {
                                temp = "file:///" + pathImgNoExist;
                                if (dtMain.Rows[0][sIndexId].ToString().Trim() != "")
                                {
                                    if (System.IO.File.Exists(pathUserImg + dtMain.Rows[0][sIndexId].ToString().Trim()))
                                    {
                                        temp = "file:///" + pathUserImg + dtMain.Rows[0][sIndexId].ToString().Trim();
                                        if (!string.IsNullOrEmpty(hostUrl))
                                        {
                                            //显示原图
                                            rowDB[sIndexId + "_Exa"] = hostUrl + "/UserImg/_" + dtMain.Rows[0][sIndexId].ToString().Trim();
                                        }
                                    }
                                }
                                rowDB[sIndexId] = temp;
                            }
                        }
                    }

                    dtReturn.Rows.Add(rowDB);
                }
                else
                {
                    err = "数据不存在";
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            return dtReturn;
        }

        public static DataTable GetGridTbDt(ref string err, string fwid, string listid, string hostUrl, string pathUserImg, string pathImgNoExist, CurrentUserEntity user = null)
        {
            DataTable dtReturn = DataTableHelp.NewGridDt();
            int num = 0;
            string temp = "";
            string grtbId = "";
            DataTable dtindex;
            DataTable dtExternalDb = comService.GetDataTable("select * from externaldb");

            string maintbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");

            foreach (DataRow drtb in comService.GetDataTable("tbbasic", "TbId", "ParentId='" + maintbid + "'", "").Rows)
            {
                grtbId = drtb["TbId"].ToString();

                dtindex = comService.GetDataTable("tbindex", "*", "TbId='" + grtbId + "'", "");
                List<ModelRule> listRule = GetRuleModelListByTbID(grtbId, dtindex, _dbtype);

                DataTable dtValueText = DataTableHelp.NewValueTextDt();

                foreach (ModelRule obj in listRule)
                {
                    if (obj.ControlType == "2")
                    {
                        if (!string.IsNullOrEmpty(obj.SqlList))
                        {
                            DataTable dt = new DataTable();
                            if (obj.DbID == "0")
                            {
                                dt = comService.GetDataTable(obj.SqlList);
                            }
                            else
                            {
                                string dbType = "";
                                string dbCon = "";
                                string sql = obj.SqlList;
                                foreach (DataRow dr in dtExternalDb.Rows)
                                {
                                    if (dr["DbID"].ToString() == obj.DbID)
                                    {
                                        dbType = dr["DbType"].ToString();
                                        dbCon = dr["DbCon"].ToString();
                                        if (dbType == "postgresql")
                                        {
                                            sql = obj.SqlList_PG;
                                        }
                                        if (dbType == "oracle")
                                        {
                                            sql = obj.SqlList_ORACLE;
                                        }
                                        break;
                                    }
                                }
                                dt = comService.GetDataTable(dbCon, dbType, sql);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                DataRow row = dtValueText.NewRow();
                                row["ClassID"] = obj.IndexId;
                                row["Value"] = dr["id"].ToString();
                                row["Text"] = dr["name"].ToString();
                                dtValueText.Rows.Add(row);
                            }
                        }
                    }
                }

                num = 0;
                DataTable dtGrid = comService.GetDataTable(grtbId, "*", "where listid=" + listid + "", "");
                foreach (DataRow dr30 in dtGrid.Rows)
                {
                    num++;

                    DataRow rowDB = dtReturn.NewRow();
                    rowDB["listid"] = listid;
                    rowDB["tbid"] = grtbId;
                    rowDB["num"] = num;

                    foreach (DataRow rowindex in dtindex.Rows)
                    {
                        string _indexId = rowindex["IndexId"].ToString();
                        string _dataType = rowindex["DataType"].ToString();
                        //1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片
                        string _controlType = rowindex["ControlType"].ToString();
                        _controlType = _controlType == "" ? "1" : _controlType;

                        if (_dataType.StartsWith("1"))//日期
                        {
                            temp = dr30[_indexId].ToString().Trim();
                            if (rowindex["isTime"].ToString() == "1")
                            {
                                rowDB[_indexId] = temp;
                            }
                            else
                            {
                                rowDB[_indexId] = temp;
                                if (temp != "")
                                {
                                    rowDB[_indexId] = DateTime.Parse(temp).ToString("yyyy-MM-dd");
                                }
                            }
                        }

                        if (_dataType.StartsWith("2"))//字符型
                        {
                            temp = dr30[_indexId].ToString();
                            rowDB[_indexId] = temp;

                            //select下拉选择
                            if (_controlType == "2")
                            {
                                string strV = "";
                                foreach (DataRow drVT in dtValueText.Rows)
                                {
                                    if (drVT["Value"].ToString() == temp && drVT["ClassID"].ToString() == _indexId)
                                    {
                                        strV = drVT["Text"].ToString();
                                        break;
                                    }
                                }
                                rowDB[_indexId] = strV;
                            }
                        }

                        if (_dataType.StartsWith("3"))//数字
                        {
                            temp = dr30[_indexId].ToString().Trim();
                            rowDB[_indexId] = temp;
                            //金额写法
                            if (_dataType == "3102" && temp != "")
                            {
                                rowDB[_indexId] = "¥" + temp;
                            }
                        }

                        if (_dataType.StartsWith("4"))//附件
                        {
                            temp = dr30[_indexId].ToString().Trim();
                            rowDB[_indexId] = temp;

                            if (temp != "")
                            {
                                if (!string.IsNullOrEmpty(hostUrl))
                                {
                                    //附件链接
                                    rowDB[_indexId + "_Exa"] = hostUrl + "/UserFile/" + maintbid + "_Files/" + listid + "/" + dr30[_indexId].ToString().Trim();
                                }
                            }
                        }

                        if (_dataType.StartsWith("5"))//图片
                        {
                            temp = "file:///" + pathImgNoExist;
                            if (dr30[_indexId].ToString().Trim() != "")
                            {
                                if (System.IO.File.Exists(pathUserImg + dr30[_indexId].ToString().Trim()))
                                {
                                    temp = "file:///" + pathUserImg + dr30[_indexId].ToString().Trim();
                                    if (!string.IsNullOrEmpty(hostUrl))
                                    {
                                        //显示原图
                                        rowDB[_indexId + "_Exa"] = hostUrl + "/UserImg/_" + dr30[_indexId].ToString().Trim();
                                    }
                                }
                            }
                            rowDB[_indexId] = temp;
                        }
                    }

                    dtReturn.Rows.Add(rowDB);
                }

                //合计行
                var _index = DataTableHelp.ToEnumerable<TbIndexEntity>(dtindex);
                if (_index.Where(x => x.ControlType == "1" && x.ListStat == "1" && x.DataType.StartsWith("3")).Count<TbIndexEntity>() > 0)//条件 1数值指标2文本框3统计求和
                {
                    DataRow rowDB = dtReturn.NewRow();
                    rowDB["listid"] = listid;
                    rowDB["tbid"] = grtbId;

                    foreach (TbIndexEntity item in _index)
                    {
                        if (item.ControlType == "1" && item.ListStat == "1" && item.DataType.StartsWith("3"))
                        {
                            rowDB[item.IndexId] = "∑ = " + comService.GetSingleField("select SUM(" + item.IndexId + ") from " + grtbId + " where ListId = " + listid + "");
                        }
                    }

                    dtReturn.Rows.Add(rowDB);
                }
            }

            return dtReturn;
        }
    }
}