using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CLDC.CLWS.CLWCS.Framework
{
    /// <summary>
    /// 枚举的扩展方法
    /// </summary>
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum item)
        {
            string name = item.ToString();
            var desc = item.GetType().GetField(name)?.GetCustomAttribute<DescriptionAttribute>();
            return desc?.Description ?? name;
        }

        public static long ToInt64(this Enum item)
        {
            return Convert.ToInt64(item);
        }

        public static List<Dictionary<string, object>> ToList(this Enum value, bool ignoreNull = false)
        {
            var enumType = value.GetType();

            if (!enumType.IsEnum)
                return null;

            return Enum.GetValues(enumType).Cast<Enum>()
                .Where(m => !ignoreNull || !m.ToString().Equals("Null")).Select(x => new Dictionary<string, object>
                {
                    ["Label"] = x.ToDescription(),
                    ["Value"] = x
                }).ToList();
        }

        public static List<Dictionary<string, object>> ToList<T>(bool ignoreNull = false)
        {
            var enumType = typeof(T);

            if (!enumType.IsEnum)
                return null;

            return Enum.GetValues(enumType).Cast<Enum>()
                 .Where(m => !ignoreNull || !m.ToString().Equals("Null")).Select(x => new Dictionary<string, object>
                 {
                     ["Label"] = x.ToDescription(),
                     ["Value"] = x
                 }).ToList();
        }
        /// <summary>
        /// 根据属性描述获取枚举值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="des">属性说明</param>
        /// <returns>枚举值</returns>
        public static T ToEnum<T>(this string des) where T : struct, IConvertible
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                return default(T);
            }
            T[] enums = (T[])Enum.GetValues(type);
            T temp;
            if (!Enum.TryParse(des, out temp))
            {
                temp = default(T);
            }
            for (int i = 0; i < enums.Length; i++)
            {
                string name = enums[i].ToString();
                FieldInfo field = type.GetField(name);
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs == null || objs.Length == 0)
                {
                    continue;
                }
                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
                string edes = descriptionAttribute.Description;
                if (des == edes)
                {
                    temp = enums[i];
                    break;
                }
            }

            return temp;

        }

    }
}