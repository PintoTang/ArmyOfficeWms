using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WHSE.Monitor.Framework.UserControls
{
	public class Roller_Bothway : Roller_Base
	{
		public Roller_Bothway()
		{
			//btn_way.Style = this.FindResource(this.GetType().Name) as Style;

			lb_DeviceName.Content = "双向辊筒";
		
		}
	}
}
