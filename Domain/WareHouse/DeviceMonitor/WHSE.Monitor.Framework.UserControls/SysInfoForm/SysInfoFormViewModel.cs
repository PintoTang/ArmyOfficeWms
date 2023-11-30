using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WHSE.Monitor.Framework.UserControls
{
	public class SysInfoFormViewModel : SysInfoBase, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		private int myVar;
		public int MyProperty
		{
			get
			{
				return myVar;
			}
			set
			{
				myVar = value;
				PropertyChanged(this, new PropertyChangedEventArgs("property"));
			}
		}


	}
}
