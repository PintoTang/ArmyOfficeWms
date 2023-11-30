using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CL.WCS.Monitor.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	public class RollerConvertorState : DeviceStateConvertor
	{
		public override string ResourcePath
		{
			get
			{
				return @"Roller\RollerDictionary.xaml";
			}
		}
	}
}