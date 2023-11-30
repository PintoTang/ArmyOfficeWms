using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WHSE.Monitor.Framework.UserControls;

namespace WHSE.Monitor.Framework.UserControls
{
	public class StackingCraneStateConvertor : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceDisplayStateEnum deviceDisplayStateEnum = (DeviceDisplayStateEnum)value;
			ResourceDictionary resource = ResoursceHelper.FromResource(@"StackingCrane\StackingCraneDictionary.xaml");
			Style result = null;
			switch (deviceDisplayStateEnum)
			{
				case DeviceDisplayStateEnum.Normal:
					result = resource["Normal"] as Style;
					break;
				case DeviceDisplayStateEnum.Failure:
					result = resource["Failure"] as Style;
					break;
				case DeviceDisplayStateEnum.EmergencyStop:
					result = resource["EmergencyStop"] as Style;
					break;
				case DeviceDisplayStateEnum.Manual:
					result = resource["Manual"] as Style;
					break;
				case DeviceDisplayStateEnum.Running:
					result = resource["Running"] as Style;
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
