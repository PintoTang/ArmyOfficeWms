using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;

namespace WHSE.Monitor.Framework.UserControls
{
	public class DeviceInfoConvertor : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string result="";
			if (value is DeviceName)
			{
				DeviceName deviceName=(DeviceName)value;
				if (deviceName!=null){				
				result=deviceName.FullName;
				}
	{
		 
	}
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
