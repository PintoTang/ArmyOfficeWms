using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.Model.DataModel
{
	public delegate bool PackageMoveInfoOutputDelegate(List<PackageMoveInfoBean> packageMoveInfoBeanList);

	public class PackageMoveInfoBean
	{
		public string PackageBarcode { set; get; }
		public DeviceName PrevDevice { set; get; }
		public DeviceName CurrDevice { set; get; }
		public DeviceName DstDevice { set; get; }
		public int TrailID { set; get; }
	}
}
