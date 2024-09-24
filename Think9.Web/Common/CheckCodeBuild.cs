using System.Collections.Generic;
using System.Data;
using System.IO;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Services.Basic
{
    public class CheckCodeBuild
    {
        private ChangeModel changeModel = new ChangeModel();
        private CodeBuildServices codeBuild = new CodeBuildServices();
        private ComService comService = new ComService();

        private string _dbtype = HelpCode.GetDBProvider("DBProvider");//数据库类型
        private static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "");

        public List<WarnEntity> GetTablesWarning(string tbid, CurrentUserEntity CurrentUser)
        {
            tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;

            string sql;
            string err = "";
            string str = "";
            string currentTbId = "";
            string fileName;
            string siteRoot = directoryPath;

            List<WarnEntity> list = new List<WarnEntity>();

            TableDtModel dtModel = AppSet.GetTableDtModel(tbid);
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype);

            var idsArray = tbid.Substring(0, tbid.Length - 1).Split(',');
            string[] arr = BaseUtil.GetStrArray(tbid, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null && arr[i].ToString().Trim() != "")
                {
                    currentTbId = arr[i].ToString().Trim();

                    fileName = directoryPath + "\\Views\\" + currentTbId.Replace("tb_", "") + "\\Index.cshtml";
                    if (!File.Exists(fileName))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, "")), level = "err" });
                    }
                    fileName = directoryPath + "\\Views\\" + currentTbId.Replace("tb_", "") + "\\Form.cshtml";
                    if (!File.Exists(fileName))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, "")), level = "err" });
                    }
                    fileName = directoryPath + "\\Views\\" + currentTbId.Replace("tb_", "") + "\\Detail.cshtml";
                    if (!File.Exists(fileName))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, "")), level = "err" });
                    }
                    fileName = directoryPath + "\\wwwroot\\Self_JS\\" + currentTbId.Replace("tb_", "") + ".js";
                    if (!File.Exists(fileName))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, "")), level = "err" });
                    }

                    //检测数据表是否建立和指标是否在数据库有对应字段
                    foreach (DataRow drTb in dtModel.allTB.Rows)
                    {
                        if (drTb["TbId"].ToString() == currentTbId || drTb["ParentId"].ToString() == currentTbId)
                        {
                            if (!comService.IsDataTableExists(drTb["TbId"].ToString()))
                            {
                                list.Add(new WarnEntity { tbid = drTb["tbid"].ToString(), indexid = "", Content = "未建数据表", level = "err" });
                            }
                            else
                            {
                                string ColumnStr = comService.GetTableFieldsString(drTb["tbid"].ToString()).ToLower();
                                foreach (DataRow row in dtModel.dtIndex.Rows)
                                {
                                    if (!ColumnStr.Contains("#" + row["indexid"].ToString().ToLower() + "#") && row["tbid"].ToString() == drTb["tbid"].ToString())
                                    {
                                        list.Add(new WarnEntity { tbid = drTb["tbid"].ToString(), indexid = row["indexid"].ToString(), Content = "数据表无对应字段", level = "err" });
                                    }
                                }
                            }

                            sql = @"SELECT tbrelation.*,relationlist.* FROM  tbrelation INNER JOIN relationlist ON tbrelation.RelationId = relationlist.RelationId where tbrelation.TbID = '" + drTb["tbid"].ToString() + "' and tbrelation.isUse <> '1' ";
                            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                            {
                                str = dr["RelationType"].ToString() == "11" ? "数据读取" : (dr["RelationType"].ToString() == "21" ? "子表数据初始化" : (dr["RelationType"].ToString() == "31" ? "数据回写" : ""));

                                list.Add(new WarnEntity { tbid = drTb["tbid"].ToString(), indexid = "", Content = str + "--" + dr["RelationName"].ToString() + "-已被禁用", level = "warn" });
                            }
                        }
                    }

                    //检测菜单是否建立或者有错误
                    str = "_TB?tbid=" + currentTbId;
                    if (comService.GetTotal("sys_module", "where UrlAddress='" + currentTbId.Replace("tb_", "") + "' OR UrlAddress='/" + currentTbId.Replace("tb_", "") + "' OR UrlAddress='" + str + "' OR UrlAddress='/" + str + "'") == 0)
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = "未建菜单或菜单链接录入错误--菜单链接发布模式为_TB?tbid=主表编码；调试模式为去除前缀tb_的主表编码", level = "err" });
                    }

                    // todo 注意 isAutoCreat给release值会重新生成前端的文件，切记240123
                    //err = codeBuild.GetTableCode(currentTbId, siteRoot, "", CurrentUser, "release", 0);
                    DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
                    err = codeBuild.GetTableCode(dtRecord, currentTbId, siteRoot, "", CurrentUser, "", 0);
                    if (!string.IsNullOrEmpty(err))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = "" + err, level = "err", Detial = "" });
                    }

                    err = "";
                    //检测编译错误
                    MemoryStream BuildAssembly = Think9.Roslyn.Compile.BuildTableAssembly(currentTbId, ref err);
                    if (!string.IsNullOrEmpty(err))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = "" + err, level = "err", Detial = "<a href='/SysTable/TableList/CodeDetail?tbid=" + currentTbId + "' target ='_blank' lay-tips='代码下载本地后，查找出错位置，或者下载代码在IDE中运行'><i class='fa fa-code'></i></a>" });
                    }

                    //changeModel.ReplaceTableCshtml(currentTbId);

                    CheckTable check = new CheckTable(dtModel, modelSet);

                    DataTable dt = check.GetCheckWarn(siteRoot, currentTbId);

                    object param = BasicHelp.GetParamObject(CurrentUser);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["level"].ToString() == "uncheck")
                        {
                            sql = dr["sql"].ToString().Trim().ToLower();
                            if (_dbtype == "mysql" && sql.StartsWith("select top 100 "))
                            {
                                sql = sql.Replace("select top 100 ", "select ") + " limit 100";
                            }

                            //跳过去先不检测呢
                            //try
                            //{
                            //    if (sql != "")
                            //    {
                            //        comService.GetDataTable(sql, param);
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    list.Add(new WarnEntity { tbid = dr["tbid"].ToString(), indexid = dr["indexid"].ToString(), Content = dr["Content"].ToString() + "有错误:" + sql + " ERR:" + ex.Message, level = "err" });
                            //}
                        }
                        else
                        {
                            list.Add(new WarnEntity { tbid = dr["tbid"].ToString(), indexid = dr["indexid"].ToString(), Content = dr["Content"].ToString(), level = dr["level"].ToString(), Detial = dr["Detial"].ToString() });
                        }
                    }
                }
            }

            return list;
        }

        public List<WarnEntity> GetReportsWarning(string tbid, CurrentUserEntity CurrentUser)
        {
            tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;

            string fileName;
            string err = "";
            string str = "";
            string currentTbId = "";
            string siteRoot = directoryPath;

            List<WarnEntity> list = new List<WarnEntity>();

            ReportDtModel dtModel = AppSet.GetReportDtModel(_dbtype);
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype);

            var idsArray = tbid.Substring(0, tbid.Length - 1).Split(',');
            string[] arr = BaseUtil.GetStrArray(tbid, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null && arr[i].ToString().Trim() != "")
                {
                    currentTbId = arr[i].ToString().Trim();

                    fileName = directoryPath + "\\Views\\" + currentTbId.Replace("rp_", "") + "\\Index.cshtml";
                    if (!File.Exists(fileName))
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = string.Format(" 缺少文件{0}，需『重新生成』;", fileName.Replace(directoryPath, "")), level = "err" });
                    }

                    str = "_RP?rpid=" + currentTbId;
                    if (comService.GetTotal("sys_module", "where UrlAddress='" + currentTbId.Replace("rp_", "") + "' OR UrlAddress='/" + currentTbId.Replace("rp_", "") + "' OR UrlAddress='" + str + "' OR UrlAddress='/" + str + "'") == 0)
                    {
                        list.Add(new WarnEntity { tbid = currentTbId, indexid = "", Content = "未建菜单或菜单链接录入错误--菜单链接发布模式为_RP?rpid=统计表编码；调试模式为去除前缀rp_的统计表编码", level = "err" });
                    }

                    CheckReport check = new CheckReport(dtModel, modelSet);

                    DataTable dt = check.GetCheckWarn(currentTbId);

                    object param = BasicHelp.GetParamObject(CurrentUser);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["level"].ToString() == "uncheck")
                        {
                        }
                        else
                        {
                            list.Add(new WarnEntity { postion = dr["postion"].ToString(), tbid = dr["tbid"].ToString(), indexid = dr["indexid"].ToString(), Content = dr["Content"].ToString(), level = dr["level"].ToString(), Detial = dr["Detial"].ToString() });
                        }
                    }
                }
            }

            return list;
        }
    }
}