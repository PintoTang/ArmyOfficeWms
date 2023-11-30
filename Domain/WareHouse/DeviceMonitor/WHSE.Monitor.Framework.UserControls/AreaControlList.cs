using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WHSE.Monitor.Framework.UserControls
{
	public class AreaControlList
	{

		#region InstanceCode		
		
		private static AreaControlList instance;
		private AreaControlList()
		{

		}
		public static AreaControlList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AreaControlList();
				}
				return instance;
			}

		}
		#endregion


		private List<AreaBean> _areaBeanList = new List<AreaBean>();

		public List<AreaBean> AreaBeanList
		{
			get { return _areaBeanList; }
			set { _areaBeanList = value; }
		}


	}
}
