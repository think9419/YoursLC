using System.Data;

namespace Think9.Services.Com
{
    public class TableHelp
    {
        /// <summary>
        /// /得到指标名称
        /// </summary>
        /// <param name="dtindex"></param>
        /// <param name="tbid"></param>
        /// <param name="indexid"></param>
        /// <returns></returns>
        public static string GetIndexName(DataTable dtindex, string tbid, string indexid)
        {
            string name = "";
            foreach (DataRow row in dtindex.Rows)
            {
                if (row["TbId"].ToString() == tbid && row["IndexId"].ToString() == indexid)
                {
                    name = row["IndexName"].ToString();
                    break;
                }
            }

            return name;
        }

        public static string ReplaceOperator(string str)
        {
            return str.Replace("加", "+").Replace("减", "-").Replace("乘", "*").Replace("除", "/");
        }
    }
}