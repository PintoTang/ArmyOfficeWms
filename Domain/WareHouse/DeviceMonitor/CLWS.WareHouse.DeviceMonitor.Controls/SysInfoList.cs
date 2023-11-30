using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class SysInfoList
	{

		private static SysInfoList instance = null;
		private SysInfoList() { 
		
		}
		public static SysInfoList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SysInfoList();
				}
				return instance;
			}
			

		}
		private List<SysGroup> _sysGroupList = new List<SysGroup>();
		/// <summary>
		/// IT系统组列表
		/// </summary>
		public List<SysGroup> SysGroupList
		{
			get { return _sysGroupList; }
			set { _sysGroupList = value; }
		}

	}
}
