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
using System.Windows.Shapes;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// DeviceInfoForm.xaml 的交互逻辑
	/// </summary>
	public partial class DeviceInfoForm : Window
	{
		public DeviceInfoForm()
		{
			InitializeComponent();

		}
		//public DeviceInfoForm(string DeviceName)
		//{
		//	InitializeComponent();

		//	this.Title = string.Format("设备信息 [{0}]", DeviceName);
		//	DeviceInfoFormViewModel viewModel = new DeviceInfoFormViewModel(DeviceName);
		//	this.DataContext = viewModel;
		//}

		public DeviceInfoForm(DeviceBase device)
		{
			if (device != null)
			{
				InitializeComponent();
				DataContext = device;
			}

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

	}
}
