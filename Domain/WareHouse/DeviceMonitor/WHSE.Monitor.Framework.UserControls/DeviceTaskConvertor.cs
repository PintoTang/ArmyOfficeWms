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
	public class DeviceTaskConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				DeviceBase viewModel = value as DeviceBase;

				if (viewModel.PackageMoveInfoList.Count > 0)
				{
					return "TaskStyleYes";
				}


			}
			return "TaskStyleNo";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
