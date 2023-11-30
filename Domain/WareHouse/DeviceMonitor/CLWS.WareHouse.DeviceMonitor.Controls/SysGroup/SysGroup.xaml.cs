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
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// SystemGroup.xaml 的交互逻辑
	/// </summary>
	public partial class SysGroup : UserControl
	{

		//private SysStateEnum systemGroupState;
		public	SysGroupViewModel viewModel;
		public SysGroup()
		{
			InitializeComponent();
			viewModel = new SysGroupViewModel();
			this.DataContext = viewModel;
			
		}


	 
		public string SysGroupName {

			get { 
				return lb_SysName.Content.ToString(); 
			}
			set { 
				lb_SysName.Content = value;
				viewModel.GroupName = value;
			}
		}

		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ShowSysInfoForm();
		}

		private void ShowSysInfoForm()
		{
			SysInfoForm form = new SysInfoForm(SysGroupName);
			form.ShowDialog();
		}

		private void btnIcon_Click(object sender, RoutedEventArgs e)
		{
			ShowSysInfoForm();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			
		
			
		}

		
		
		

		
	}
}
