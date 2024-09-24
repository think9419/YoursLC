using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class SysParameter
    {
        public static List<valueTextEntity> GetFileType()
        {
            List<valueTextEntity> list = new List<valueTextEntity>();

            list.Add(new valueTextEntity { Value = "jpg|png|gif|bmp|jpeg|svg", Text = "图片类 jpg|png|gif|bmp|jpeg|svg", ClassID = "" });
            list.Add(new valueTextEntity { Value = "xls|xlsx|pdf|doc|docx|txt|ppt|pptx", Text = "文档类 xls|xlsx|pdf|doc|docx|txt|ppt|pptx", ClassID = "" });
            list.Add(new valueTextEntity { Value = "zip|rar|7z", Text = "文件类 zip|rar|7z", ClassID = "" });
            list.Add(new valueTextEntity { Value = "flv|swf|mkv|avi|rm|rmvb|mpeg|mp4", Text = "视频类 flv|swf|mkv|avi|rm|rmvb|mpeg|mp4", ClassID = "" });
            list.Add(new valueTextEntity { Value = "mp3|wav|wmv|mov", Text = "音频类 mp3|wav|wmv|mov", ClassID = "" });

            return list;
        }

        public static List<valueTextEntity> GetImgType()
        {
            List<valueTextEntity> list = new List<valueTextEntity>();

            list.Add(new valueTextEntity { Value = "jpg|png|gif|bmp|jpeg|svg", Text = "jpg|png|gif|bmp|jpeg|svg", ClassID = "" });

            return list;
        }

        /// <summary>
        /// 获取系统指标值
        /// </summary>
        public static List<valueTextEntity> GetList()
        {
            ComService comService = new ComService();
            List<valueTextEntity> list = new List<valueTextEntity>();

            DataTable dtParm = comService.GetDataTable("select * from indexparm  ");
            foreach (DataRow dr in dtParm.Rows)
            {
                list.Add(new valueTextEntity { Value = dr["ParmId"].ToString(), Text = "<sapn style='color: #FFAB00;'>{自定义参数}</sapn>@" + dr["ParmName"].ToString() + "{" + dr["ParmId"].ToString() + "}", ClassID = "parm" });
            }

            var sysList = EnumHelp.GetEnumList<SysParameterEnum>();
            foreach (SysParameterEnum obj in sysList)
            {
                list.Add(new valueTextEntity { Value = "@" + obj.ToString(), Text = "{系统参数}@" + obj.ToString() + EnumHelp.GetDescriptionByEnum<SysParameterEnum>(obj) + "{@" + obj.ToString() + "}", ClassID = "sys" });
            }

            return list;
        }

        /// <summary>
        /// 获取系统指标值
        /// </summary>
        public static List<valueTextEntity> GetList(string type)
        {
            //type变量类型--1日期2字符3数字4附件5图片
            List<valueTextEntity> list = new List<valueTextEntity>();

            if (type.StartsWith("1"))
            {
                foreach (SysTimeParameterEnum obj in EnumHelp.GetEnumList<SysTimeParameterEnum>())
                {
                    list.Add(new valueTextEntity { Value = "@" + obj.ToString(), Text = EnumHelp.GetDescriptionByEnum<SysTimeParameterEnum>(obj), ClassID = "sys" });
                }
            }

            if (type.StartsWith("2"))
            {
                foreach (SysStrParameterEnum obj in EnumHelp.GetEnumList<SysStrParameterEnum>())
                {
                    list.Add(new valueTextEntity { Value = "@" + obj.ToString(), Text = EnumHelp.GetDescriptionByEnum<SysStrParameterEnum>(obj), ClassID = "sys" });
                }
            }

            if (type.StartsWith("3"))
            {
                if (type == "3000")
                {
                    foreach (SysNumParameterEnum obj in EnumHelp.GetEnumList<SysNumParameterEnum2>())
                    {
                        list.Add(new valueTextEntity { Value = "@" + obj.ToString(), Text = EnumHelp.GetDescriptionByEnum<SysNumParameterEnum2>(obj), ClassID = "sys" });
                    }
                }
                else
                {
                    foreach (SysNumParameterEnum obj in EnumHelp.GetEnumList<SysNumParameterEnum>())
                    {
                        list.Add(new valueTextEntity { Value = "@" + obj.ToString(), Text = EnumHelp.GetDescriptionByEnum<SysNumParameterEnum>(obj), ClassID = "sys" });
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取系统指标值
        /// </summary>
        public static void GetList(ref DataTable dtReturn)
        {
            var list = EnumHelp.GetEnumList<SysParameterEnum>();
            foreach (SysParameterEnum obj in list)
            {
                DataRow row = dtReturn.NewRow();
                row["Value"] = "@" + obj.ToString();
                row["Text"] = "{系统参数}@" + obj.ToString() + EnumHelp.GetDescriptionByEnum<SysParameterEnum>(obj);
                dtReturn.Rows.Add(row);
            }
        }
    }
}