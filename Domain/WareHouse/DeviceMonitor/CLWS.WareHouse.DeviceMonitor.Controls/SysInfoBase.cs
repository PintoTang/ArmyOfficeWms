using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	public class SysInfoBase
	{
	
		public SysInfoBase()
		{
			if (SysInfoList.Instance.SysGroupList.Count == 0)
			{
				InitSysInfo();
			}

		}

		
		private void InitSysInfo()
		{
			String path = Environment.CurrentDirectory + "\\Config\\" + ConfigurationManager.AppSettings["ConfigPath"].ToString() + "\\SysGroupConfig.xml";

			XElement xElement = XElement.Load(path);
			var xElementSysGroup = xElement.Elements("Group");
			if (xElementSysGroup != null)
			{
				foreach (var xElementSys in xElementSysGroup)
				{
					if (xElementSys.Elements().Count() == 0)
					{
						Console.WriteLine("{0}组为空", xElementSys.Value);
						continue;
					}

					SysGroup sysGroup = new SysGroup
					{						
						SysGroupName = xElementSys.Attribute("GroupName").Value.ToString()
					};
					sysGroup.viewModel.Children.Clear();
					foreach (var sysElement in xElementSys.Elements())
					{
						//添加项
						XAttribute sysName = sysElement.Attribute("SysName");
					
						SysInfo sysInfo = new SysInfo
						{
							UserControlName = sysName.Value
						};
						sysGroup.viewModel.Children.Add(sysInfo);

						SysInfoViewModel sysinfoVW = sysInfo.DataContext as SysInfoViewModel;

						sysinfoVW.ChangeSysinfoState += sysGroup.viewModel.ChangeGroupState;						
					}
					SysInfoList.Instance.SysGroupList.Add(sysGroup);
				}
			}
		}

		
		
		/// <summary>
		/// IT系统组列表
		/// </summary>
		public List<SysGroup> SysGroupList
		{
			get { return SysInfoList.Instance.SysGroupList; }
			set { SysInfoList.Instance.SysGroupList = value; }
		}



	}
}
