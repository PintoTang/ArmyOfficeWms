using System;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;

namespace CL.WCS.DataModelPckg.View
{
    /// <summary>
    /// 地址转换
    /// </summary>
    public class AddrConvertToValue : IValueConverter
    {
        /// <summary>
        /// 地址转换成String
        /// </summary>
        /// <param name="value">地址</param>
        /// <param name="targetType">类型</param>
        /// <param name="parameter">参数对象</param>
        /// <param name="culture"></param>
        /// <returns>CultureInfo</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Addr)
            {
                Addr address = (Addr)value;
                return address.ToString();
            }
            else
            {
                return "";
            }

        }
        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is string)
                {
                    return new Addr(value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
