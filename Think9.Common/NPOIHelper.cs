using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Think9.Util;

namespace Think9.Services.Com
{
    /// <summary>
    /// 利用NPOI类库，实现对Excel进行操作的辅助类
    /// </summary>
    public class NPOIHelper
    {
        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static DataTable ExcelToTable(string file, List<string> listHeader, ref string nColumns)
        {
            string errLocation = "";
            DataTable dt = new DataTable();
            int iColumns = 0;
            IWorkbook workbook;
            try
            {
                string fileExt = Path.GetExtension(file).ToLower();
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                    if (fileExt == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    else if (fileExt == ".xls")
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    else
                    {
                        workbook = null;
                    }

                    if (workbook == null)
                    { return null; }

                    ISheet sheet = workbook.GetSheetAt(0);

                    //表头
                    IRow header = sheet.GetRow(sheet.FirstRowNum);
                    nColumns = header.LastCellNum.ToString();
                    List<int> columns = new List<int>();
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        errLocation = "GetValueType(header.GetCell(i)) 获取单元格类型，当前列为" + i + "";
                        iColumns = i + 1;
                        object obj = GetValueType(header.GetCell(i));
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("列" + iColumns.ToString()));
                        }
                        else
                        {
                            dt.Columns.Add(new DataColumn("列" + iColumns.ToString()));
                        }

                        listHeader.Add("列" + iColumns.ToString());

                        columns.Add(i);
                    }

                    //数据
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)//行
                    {
                        DataRow dr = dt.NewRow();
                        bool hasValue = false;
                        foreach (int j in columns)//列
                        {
                            errLocation = "GetValueType(sheet.GetRow(i).GetCell(j)) 读取行(" + i + ")列(" + j + ")";
                            dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                            if (dr[j] != null && dr[j].ToString() != string.Empty)
                            {
                                hasValue = true;
                            }
                        }
                        if (hasValue)
                        {
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(GetErr(ex, errLocation));
            }

            return dt;
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;

                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;

                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;

                case CellType.String: //STRING:
                    return cell.StringCellValue;

                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;

                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }

        /// <summary>
        /// 读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        private static string GetErr(Exception ex, string location = "")
        {
            string err = "" + ex.GetType().Name;
            if (ex.GetType() == typeof(OverflowException))
            {
                err = "算术运算超出范围 ";
            }
            if (ex.GetType() == typeof(NullReferenceException))
            {
                err = "尝试使用空引用 ";
            }
            if (ex.GetType() == typeof(IndexOutOfRangeException))
            {
                err = "尝试访问数组或集合的不存在的索引 ";
            }
            if (ex.GetType() == typeof(ArgumentException))
            {
                err = "参数无效 ";
            }
            if (ex.GetType() == typeof(FormatException))
            {
                err = "字符串格式不正确 ";
            }
            if (ex.GetType() == typeof(System.IO.IOException))
            {
                err = "I/O 操作失败 ";
            }

            if (!string.IsNullOrEmpty(location))
            {
                err += " 出错位置：" + location;
            }

            if (ex != null)
            {
                err += ex.Message;
                err += Environment.NewLine;
                Exception originalException = ex.GetOriginalException();
                if (originalException != null)
                {
                    if (originalException.Message != ex.Message)
                    {
                        err += originalException.Message;
                        err += Environment.NewLine;
                        err += originalException.StackTrace;
                    }
                }
            }

            return err;
        }
    }
}