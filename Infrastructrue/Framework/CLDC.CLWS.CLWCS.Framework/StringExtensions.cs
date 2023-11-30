using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Framework
{
    public static class StringExtensions
    {
        /// <summary>
        /// 判断字符串是否为Null、空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNull(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// 判断字符串是否不为Null、空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NotNull(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// 与字符串进行比较，忽略大小写
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string s, string value)
        {
            return s.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 首字母转小写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstCharToLower(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            string str = s.First().ToString().ToLower() + s.Substring(1);
            return str;
        }

        /// <summary>
        /// 首字母转大写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            string str = s.First().ToString().ToUpper() + s.Substring(1);
            return str;
        }

        public static string ToPath(this string s)
        {
            if (s.IsNull())
                return string.Empty;

            return s.Replace(@"\", "/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> StringToList(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            List<string> list = str.Split(new char[] { ';', '；', '、', ',', '，', '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list;
        }
    }
}