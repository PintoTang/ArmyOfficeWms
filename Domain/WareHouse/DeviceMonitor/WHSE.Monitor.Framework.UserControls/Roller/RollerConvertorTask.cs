using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CL.WCS.Monitor.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	public class RollerConvertorTask : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool result = (bool)value;
			if (result)
			{
				return "True";
			}
			else
			{
				return "False";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
