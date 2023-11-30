using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CL.WCS.Monitor.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	public abstract class DeviceStateConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ResourceDictionary resource = ResoursceHelper.FromResource(ResourcePath);			
			DeviceDisplayStateEnum deviceDisplayStateEnum = (DeviceDisplayStateEnum)value;
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



		public abstract string ResourcePath { get; }

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
