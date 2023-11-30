using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WHSE.Monitor.Framework.UserControls
{
	public class FaultMessageConvertor
	{
		public static string FaultInfoToString(List<byte> fault) {

			if (fault==null)
			{
				return "";
			}
			string result = "";
			foreach (var item in fault)
			{
				result += string.Format("[0x{0}]", Convert.ToString(item, 16).ToUpper().PadLeft(2, '0'));
			}
			return result;
		}

	
	}
}
