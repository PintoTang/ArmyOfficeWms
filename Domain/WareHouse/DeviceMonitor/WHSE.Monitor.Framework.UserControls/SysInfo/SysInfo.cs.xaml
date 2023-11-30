using System;
using System.Collections.Generic;
using System.Globalization;
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
using CL.WCS.Monitor.DataPool;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// Server.xaml 的交互逻辑
	/// </summary>
	public partial class SysInfo : UserControl
	{
		public SysInfo()
		{
			InitializeComponent();
		}

		private string _userControlName = "";

		/// <summary>
		/// 系统名称
		/// </summary>
		public string UserControlName
		{
			get
			{
				return _userControlName;
			}
			set
			{
				_userControlName = value;
				lb_sysName.Content = value;


				SysInfoViewModel viewModel = new SysInfoViewModel();
				DataContext = viewModel;
				viewModel.SysState = SysStateDataPool.Instance.GetSysState(value);
				viewModel.SysName = value;
				SysStateDataPool.Instance.RegisterViewRefreshDelegate(
					value,
					state =>
					{
						viewModel.SysState = state;

						if (SysStateEnum.OFFLINE == state)
						{
							DateTime latestHeartbeatTime = SysStateDataPool.Instance.GetLatestHeartbeatTime(value);
							if (latestHeartbeatTime == new DateTime())
							{
								viewModel.LatestHeartbeatTime = "";
							}
							else
							{
								viewModel.LatestHeartbeatTime = latestHeartbeatTime.ToString("G");
							}
						}
						else
						{
							viewModel.LatestHeartbeatTime = "";
						}
					}
				);
			}
		}

		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			
		}
	}

}
