using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WHSE.Monitor.Framework.UserControls
{
    /// <summary>
    /// AreaEntry转换
    /// </summary>
	public class AreaEntryConvertor:  IValueConverter
	{
        /// <summary>
        /// DeviceDisplayStateEnum 转换 DeviceDisplayStateEnum
        /// </summary>
        /// <param name="value">DeviceDisplayStateEnum</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">object</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>DeviceDisplayStateEnum</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{			
			DeviceDisplayStateEnum state = (DeviceDisplayStateEnum)value;
			string result = "";
			switch (state)
			{
				case DeviceDisplayStateEnum.Normal:
					result = "Normal";
					break;
				case DeviceDisplayStateEnum.Failure:
					result = "Failure";
					break;
				case DeviceDisplayStateEnum.Manual:
					result = "Manual";
					break;
				default:
					break;
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
