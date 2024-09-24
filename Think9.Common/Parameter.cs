using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class Parameter
    {
        /// <summary>
        /// 获取系统指标值
        /// </summary>
        public static List<valueTextEntity> GetSysParameterList()
        {
            List<valueTextEntity> list = new List<valueTextEntity>();

            var tlist = EnumHelp.GetEnumList<SysParameterEnum>();
            foreach (SysParameterEnum obj in tlist)
            {
                list.Add(new valueTextEntity { Value = "@" + obj.ToString(), Text = "{系统参数}@" + obj.ToString() + EnumHelp.GetDescriptionByEnum<SysParameterEnum>(obj) });
            }

            return list;
        }

        /// <summary>
        /// 获取系统指标值
        /// </summary>
        public static void GetSysParameterList(ref DataTable dtReturn)
        {
            var tlist = EnumHelp.GetEnumList<SysParameterEnum>();
            foreach (SysParameterEnum obj in tlist)
            {
                DataRow row = dtReturn.NewRow();
                row["Value"] = "@" + obj.ToString();
                row["Text"] = "{系统参数}@" + obj.ToString() + EnumHelp.GetDescriptionByEnum<SysParameterEnum>(obj);
                dtReturn.Rows.Add(row);
            }
        }
    }
}