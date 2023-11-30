using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	public class PackageBean
	{
		private string _packageNo;

		public string PackageBarcode
		{
			get { return _packageNo; }
			set { _packageNo = value; }
		}

		private DeviceBase _currertLocaton;

		public DeviceBase CurrertLocaton
		{
			get { return _currertLocaton; }
			set { _currertLocaton = value; }
		}

		private PackageMoveInfoBean _packageMoveInfo;

		public PackageMoveInfoBean PackageMoveInfo
		{
			get { return _packageMoveInfo; }
			set { _packageMoveInfo = value; }
		}
		
	}
}
