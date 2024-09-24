using System;
using System.Data;

namespace Think9.Services.Base
{
    public class BaseTool
    {
        private static string Split = "Ξ";

        /// <summary>
        /// 把 string[] 按照分隔符组装成 string
        /// </summary>
        /// <param name="sOutput"></param>
        /// <returns></returns>
        public static string[] GetStrArray(string sOutput, string sPeater)
        {
            string[] stringSeparators = new string[] { sPeater };
            string[] stReturn = sOutput.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            return stReturn;
        }

        /// <summary>
        /// 得到树形列表
        /// </summary>
        public static DataTable GetTreeDt(DataTable dtAll, string classid)
        {
            DataTable dtReturn = new DataTable("valueText");
            dtReturn.Columns.Add("ClassID", typeof(String));
            dtReturn.Columns.Add("Value", typeof(String));
            dtReturn.Columns.Add("Text", typeof(String));
            dtReturn.Columns.Add("Exa", typeof(String));

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

                    BaseTool.CreateRow(ref OldId, dr["id"].ToString().Trim(), classid, dtAll, dtReturn, "├『");//
                }
            }

            return dtReturn;
        }

        /// <summary>
        /// 得到树形列表
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

                    BaseTool.CreateRow(ref OldId, dr["id"].ToString().Trim(), classid, dtAll, dtReturn, "├『");//
                }
            }
        }

        /// <summary>
        /// 添加一个row
        /// </summary>
        private static void CreateRow(ref string strOldId, string id, string classid, DataTable dtAll, DataTable dtReturn, string strAdd)
        {
            strAdd = "---" + strAdd + "";
            foreach (DataRow dr in dtAll.Rows)
            {
                if (dr["id"].ToString().Trim().StartsWith(id) && !strOldId.Contains(Split + dr["id"].ToString().Trim() + Split))
                {
                    DataRow row = dtReturn.NewRow();
                    row["Value"] = dr["id"].ToString().Trim();
                    row["Text"] = strAdd + dr["name"].ToString() + "』";
                    row["Exa"] = dr["name"].ToString().Trim();
                    row["ClassID"] = classid;
                    dtReturn.Rows.Add(row);
                    strOldId += dr["id"].ToString().Trim() + Split;

                    BaseTool.CreateRow(ref strOldId, dr["Id"].ToString(), classid, dtAll, dtReturn, strAdd);
                }
            }
        }

        /// <summary>
        /// 得到小数位数
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
            //图片
            if (DataType.Substring(0, 1) == "5")
            {
                iDigit = 0;
            }

            return iDigit;
        }

        /// <summary>
        /// 得到长度
        /// </summary>
        public static int GetDataMlenByDataType(string sDataTypeId)
        {
            int iMlen = 0;
            //日期
            if (sDataTypeId.Substring(0, 1) == "1")
            {
                iMlen = 8;
            }
            //文字
            if (sDataTypeId.Substring(0, 1) == "2")
            {
                iMlen = int.Parse(sDataTypeId.Substring(1, 3));
            }
            //数字
            if (sDataTypeId.Substring(0, 1) == "3")
            {
                iMlen = 9;
            }
            //图片
            if (sDataTypeId.Substring(0, 1) == "5")
            {
                iMlen = 200;
            }

            return iMlen;
        }
    }
}