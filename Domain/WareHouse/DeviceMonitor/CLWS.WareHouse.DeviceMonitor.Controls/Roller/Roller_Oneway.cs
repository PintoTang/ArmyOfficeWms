using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WHSE.Monitor.Framework.UserControls
{
	public class Roller_Oneway : Roller_Base
	{
		public Roller_Oneway()
		{
			//btn_way.Style = this.FindResource(this.GetType().Name) as Style;
			left.Width = new GridLength(0);
			lb_DeviceName.Content = "单向辊筒";
		}
	}
}
