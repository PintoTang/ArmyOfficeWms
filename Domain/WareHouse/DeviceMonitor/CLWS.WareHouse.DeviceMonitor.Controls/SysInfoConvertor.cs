using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WHSE.Monitor.Framework.Model.DataModel;


namespace WHSE.Monitor.Framework.UserControls
{
	public class SysInfoConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ResourceDictionary myResourceDictionary = ResoursceHelper.CommResource;

			ControlTemplate controlTemplate = null;			
			if (value != null)
			{
				SysStateEnum sysState = (SysStateEnum)value;
				switch (sysState)
				{
					case SysStateEnum.OFFLINE:					
						controlTemplate = myResourceDictionary["OfflineIcon"] as ControlTemplate;
						break;
					case SysStateEnum.RUNNING:						
						controlTemplate = myResourceDictionary["OnlineIcon"] as ControlTemplate;
						break;
					case SysStateEnum.PAUSE:						
						controlTemplate = myResourceDictionary["PauseIcon"] as ControlTemplate;
						break;
					default:
						break;
				}
			}
			return controlTemplate;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
