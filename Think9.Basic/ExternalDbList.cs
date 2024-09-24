using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class DbTypeList
    {
        public static List<valueTextEntity> GetList()
        {
            List<valueTextEntity> list = new List<valueTextEntity>();

            var _list = EnumHelp.GetEnumList<ExternalDbTypeEnum>();
            foreach (ExternalDbTypeEnum obj in _list)
            {
                list.Add(new valueTextEntity { Value = obj.ToString(), Text = EnumHelp.GetDescriptionByEnum<ExternalDbTypeEnum>(obj) });
            }

            return list;
        }
    }
}