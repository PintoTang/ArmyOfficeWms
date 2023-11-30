using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class PackageList
	{
		#region IntanceCode
		private static PackageList instance;
		private PackageList()
		{

		}
		public static PackageList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PackageList();
				}
				return instance;
			}

		}
		#endregion

		private List<PackageBean> _packageBeanList=new List<PackageBean>();

		/// <summary>
		/// 设备上运行的货品集合
		/// </summary>
		public List<PackageBean> PackageBeanList
		{
			get { return _packageBeanList; }
			set { _packageBeanList = value; }
		}

	}
}
