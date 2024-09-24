using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class ValidateList
    {
        public static List<valueTextEntity> GetList()
        {
            List<valueTextEntity> list = new List<valueTextEntity>();

            foreach (ValidateEnum obj in EnumHelp.GetEnumList<ValidateEnum>())
            {
                list.Add(new valueTextEntity { Value = obj.ToString(), Text = EnumHelp.GetDescriptionByEnum<ValidateEnum>(obj) });
            }

            return list;
        }
    }
}