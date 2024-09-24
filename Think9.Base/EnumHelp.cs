using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Think9.Services.Base
{
    public static class EnumHelp
    {
        #region 获取枚举列表

        /// <summary>
        /// 通过枚举对象获取枚举列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<T> GetEnumList<T>(this T value)
        {
            var list = new List<T>();
            if (value is Enum)
            {
                var valData = Convert.ToInt32((T)Enum.Parse(typeof(T), value.ToString()));
                var tps = Enum.GetValues(typeof(T));

                list.AddRange(from object tp in tps where ((int)Convert.ToInt32((T)Enum.Parse(typeof(T), tp.ToString())) & valData) == valData select (T)tp);
            }

            return list;
        }

        /// <summary>
        /// 通过枚举类型获取枚举列表;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<T> GetEnumList<T>() where T : Enum
        {
            List<T> list = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            return list;
        }

        /// <summary>
        /// Gets all items for an enum value.（通过枚举对象获取所有枚举）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>(this Enum value)
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

        /// <summary>
        /// Gets all items for an enum type.（通过枚举类型获取所有枚举）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

        public static string GetDescriptionByEnum<T>(this object obj)
        {
            var tEnum = System.Enum.Parse(typeof(T), obj.ParseToString()) as System.Enum;
            var description = tEnum.GetDescription();
            return description;
        }

        /// <summary>
        /// 获取枚举值对应的描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDescription(this System.Enum enumType)
        {
            FieldInfo EnumInfo = enumType.GetType().GetField(enumType.ToString());
            if (EnumInfo != null)
            {
                DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (EnumAttributes.Length > 0)
                {
                    return EnumAttributes[0].Description;
                }
            }
            return enumType.ToString();
        }

        /// <summary>
        /// 将object转换为string，若转换失败，则返回""。不抛出异常。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParseToString(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return string.Empty;
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion 获取枚举列表
    }
}