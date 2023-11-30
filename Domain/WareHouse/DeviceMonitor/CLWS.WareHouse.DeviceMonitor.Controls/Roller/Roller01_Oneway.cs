using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WHSE.Monitor.Framework.UserControls
{
	public class Roller01_Oneway : Roller01
	{
		public Roller01_Oneway()
		{
			this.ArrowBackward.Visibility = Visibility.Hidden;
			this.Tag = "单向辊筒";
		}
	}
}
