using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Think9.Services.Base
{
    public class BaseUtil
    {
        private static string Split = "&Ξ#";

        //如果sql中不包含obj的属性就将该属性移除
        public static object RemoveObjAttributesBySql(object obj, string sql)
        {
            JObject objNew = JObject.FromObject(obj);
            Type t = obj.GetType();
            var typeArr = t.GetProperties();
            if (obj != null)
            {
                foreach (var pi in typeArr)
                {
                    if (!sql.ToLower().Contains("@" + pi.Name.ToLower()))
                    {
                        objNew.Remove(pi.Name);
                    }
                }
            }

            return objNew;
        }

        public static string GetErrStr(Exception ex)
        {
            string err = "";

            if (ex != null)
            {
                if (ex.InnerException != null)
                {
                    err += ex.InnerException.Message + " ";
                }

                err += ex.Message.ToString() + " ";
                err += ex.StackTrace.ToString() + " ";
            }

            return err.Replace("\u00601", " ").Replace("\u0027", " ").Replace("\r\n", " ");
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string ReplaceHtml(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring))
            {
                return Htmlstring;
            }
            Htmlstring = Regex.Replace(Htmlstring, "&#xA;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\r\n", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\r", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\n", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\"", "\\\"", RegexOptions.IgnoreCase);

            return Htmlstring;
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string ReplaceHtmlSymbol(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring))
            {
                return "";
            }
            Htmlstring = Regex.Replace(Htmlstring, "&#xA;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\r\n", " <br />", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\r", " <br />", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\n", " <br />", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "\"", "\\\"", RegexOptions.IgnoreCase);

            return Htmlstring;
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHtml(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&hellip;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&mdash;", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");

            Htmlstring = HtmlEncoder.Default.Encode(Htmlstring).Trim();
            return Htmlstring;
        }

        /// <summary>
        /// 把 string[] 按照分隔符组装成 string
        /// </summary>
        /// <param name="sOutput"></param>
        /// <returns></returns>
        public static string[] GetStrArray(string sOutput, string sPeater)
        {
            string[] stringSeparators = new string[] { sPeater };
            string[] _return = sOutput.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            return _return;
        }

        /// <summary>
        ///  按照id前缀重排得到树形列
        /// </summary>
        public static DataTable GetTreeDt(DataTable dtAll, string classid)
        {
            DataTable dtReturn = new DataTable("valueText");
            dtReturn.Columns.Add("ClassID", typeof(String));
            dtReturn.Columns.Add("Value", typeof(String));
            dtReturn.Columns.Add("Text", typeof(String));
            dtReturn.Columns.Add("Exa", typeof(String));

            string oldId = Split;
            string Add = "╋";
            foreach (DataRow dr in dtAll.Rows)
            {
                if (!oldId.Contains(Split + dr["id"].ToString().Trim() + Split))
                {
                    DataRow row = dtReturn.NewRow();
                    row["Value"] = dr["id"].ToString().Trim();
                    row["Text"] = Add + dr["name"].ToString().Trim();
                    row["Exa"] = dr["name"].ToString().Trim();
                    row["ClassID"] = classid;
                    dtReturn.Rows.Add(row);
                    oldId += dr["id"].ToString().Trim() + Split;

                    BaseUtil.CreateRow(ref oldId, dr["id"].ToString().Trim(), classid, dtAll, dtReturn, "├『");//
                }
            }

            return dtReturn;
        }

        /// <summary>
        /// 按照id前缀重排得到树形列表
        /// </summary>
        public static void GetTreeDt(DataTable dtReturn, DataTable dtAll, string classid)
        {
            string OldId = Split;
            string Add = "╋";
            foreach (DataRow dr in dtAll.Rows)
            {
                if (!OldId.Contains(Split + dr["id"].ToString().Trim() + Split))
                {
                    DataRow row = dtReturn.NewRow();
                    row["Value"] = dr["id"].ToString().Trim();
                    row["Text"] = Add + dr["name"].ToString().Trim();
                    row["Exa"] = dr["name"].ToString().Trim();
                    row["ClassID"] = classid;
                    dtReturn.Rows.Add(row);
                    OldId += dr["id"].ToString().Trim() + Split;

                    BaseUtil.CreateRow(ref OldId, dr["id"].ToString().Trim(), classid, dtAll, dtReturn, "├『");//
                }
            }
        }

        /// <summary>
        /// 添加一个row
        /// </summary>
        private static void CreateRow(ref string oldId, string id, string classid, DataTable dtAll, DataTable dtReturn, string strAdd)
        {
            strAdd = "---" + strAdd + "";
            foreach (DataRow dr in dtAll.Rows)
            {
                if (dr["id"].ToString().Trim().StartsWith(id) && !oldId.Contains(Split + dr["id"].ToString().Trim() + Split))
                {
                    DataRow row = dtReturn.NewRow();
                    row["Value"] = dr["id"].ToString().Trim();
                    row["Text"] = strAdd + dr["name"].ToString() + "』";
                    row["Exa"] = dr["name"].ToString().Trim();
                    row["ClassID"] = classid;
                    dtReturn.Rows.Add(row);
                    oldId += dr["id"].ToString().Trim() + Split;

                    BaseUtil.CreateRow(ref oldId, dr["Id"].ToString(), classid, dtAll, dtReturn, strAdd);
                }
            }
        }

        /// <summary>
        /// 得到指标类型的小数位数
        /// </summary>
        public static int GetDataDigitByDataType(string DataType)
        {
            int iDigit = 0;
            //日期
            if (DataType.Substring(0, 1) == "1")
            {
                iDigit = 0;
            }
            //文字
            if (DataType.Substring(0, 1) == "2")
            {
                iDigit = 0;
            }
            //数字
            if (DataType.Substring(0, 1) == "3")
            {
                iDigit = int.Parse(DataType.Substring(2, 2));
            }
            //附件
            if (DataType.Substring(0, 1) == "4")
            {
                iDigit = 0;
            }
            //图片
            if (DataType.Substring(0, 1) == "5")
            {
                iDigit = 0;
            }

            return iDigit;
        }

        /// <summary>
        /// 得到指标类型的长度
        /// </summary>
        public static int GetDataMlenByDataType(string dataTypeId)
        {
            int iMlen = 0;
            //日期
            if (dataTypeId.Substring(0, 1) == "1")
            {
                iMlen = 8;
            }
            //文字
            if (dataTypeId.Substring(0, 1) == "2")
            {
                iMlen = int.Parse(dataTypeId.Substring(1, 3));
            }
            //数字
            if (dataTypeId.Substring(0, 1) == "3")
            {
                iMlen = 9;
            }
            //图片
            if (dataTypeId.Substring(0, 1) == "5")
            {
                iMlen = 200;
            }

            return iMlen;
        }

        /// <summary>
        /// 统一处理名称，对应flowrunlist表runName字段
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="flowid">流程id</param>
        /// <param name="flname">流程名称</param>
        /// <param name="dt">当前数据行</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetRunName(string userid, string flowid, string flname, DataTable dt = null, string info = null)
        {
            string timeStr = String.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            return "【" + userid + "-" + timeStr + "】" + flname;

            //if (flowid == "***")
            //{
            //    if (dt != null)
            //    {
            //        //可在此自定义
            //    }
            //}
        }

        /// <summary>
        /// 得到编号，对应flowrunlist表ruNumber字段
        /// </summary>
        public static string GetRuNumber(string flowid, long listid)
        {
            if (flowid.StartsWith("bi_"))
            {
                return flowid.Replace("bi_", "") + listid.ToString();
            }
            else
            {
                long i = (listid + 37) * 3 + 23;

                return DateTime.Now.Year.ToString() + i.ToString().PadLeft(8, '0') + DateTime.Now.Millisecond.ToString().PadLeft(4, '0');
            }
        }
    }
}