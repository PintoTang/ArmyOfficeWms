using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CL.WCS.Monitor.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// SysGroupBar.xaml 的交互逻辑
	/// </summary>
	public partial class SysGroupBar : UserControl
	{

		//SysInfoBarViewModel viewModel;
		public SysGroupBar()
		{
			InitializeComponent();

			//viewModel = new SysInfoBarViewModel();
			//DataContext = viewModel;
			SysInfoBase sysInfoBase = new SysInfoBase();

		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			List<SysGroup> sysInfoList = SysInfoList.Instance.SysGroupList;
			if (sysInfoList == null)
			{
				return;
			}
			sPanel.Children.Clear();
			foreach (SysGroup item in sysInfoList)
			{
			
				if (item == null)
				{
					continue;
				}

				//SysGroupName = item.SysGroupName,					
				item.Height = sPanel.Height;
				item.Margin = new Thickness(20, 0, 0, 0);
				//SysGroupViewModel groupinfo = sysGroup.DataContext as SysGroupViewModel;
			
				sPanel.Children.Add(item);
				
				//groupinfo.Children = item.SystemInfoList;

			}

		}
	}
}
