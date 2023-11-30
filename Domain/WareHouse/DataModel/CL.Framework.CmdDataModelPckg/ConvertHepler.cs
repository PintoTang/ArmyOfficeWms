using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.CmdDataModelPckg
{
    /// <summary>
    /// 数据转换助手
    /// </summary>
    public static class ConvertHepler
    {
        /// <summary>
        /// 字符转DateTime
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string datetime)
        {
            DateTime defaultValue;
            DateTime.TryParse(datetime, out defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// 字符转Int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertToInt(string value)
        {
            int defaultValue;
            int.TryParse(value, out defaultValue);
            return defaultValue;
        }

    }
}
