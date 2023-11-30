using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class RgvStateConvertor : DeviceStateConvertor
	{
		public override string ResourcePath
		{
			get { return @"RGV/RgvDictionary.xaml"; }
		}
	}
}
