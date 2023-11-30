using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class DeviceInfoHelper
	{
		public static void ShowDetails(DeviceBase device)
		{
			DeviceInfoForm form = new DeviceInfoForm(device);
			form.Topmost = true;
			form.ShowDialog();

		}
	}
}
