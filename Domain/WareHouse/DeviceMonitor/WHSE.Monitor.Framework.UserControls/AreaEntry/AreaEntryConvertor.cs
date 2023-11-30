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
	public class AreaEntryConvertor:  IValueConverter
	{
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
