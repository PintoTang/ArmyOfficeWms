using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class HoisterConvertor:  DeviceStateConvertor
	{

		public override string ResourcePath
		{
			get { return @"Hoister\HoisterDictionary.xaml"; }
			
		}
	}
}
