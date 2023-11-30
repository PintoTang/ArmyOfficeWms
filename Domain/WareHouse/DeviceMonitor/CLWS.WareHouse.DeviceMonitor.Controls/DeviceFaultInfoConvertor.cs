using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WHSE.Monitor.Framework.UserControls
{
	public class DeviceFaultInfoConvertor : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			List<byte> fault = (List<byte>)value;
			return FaultMessageConvertor.FaultInfoToString(fault);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
