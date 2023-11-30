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
	/// SysInfoForm.xaml 的交互逻辑
	/// </summary>
	public partial class SysInfoForm : Window
	{
		public SysInfoForm()
		{
			InitializeComponent();
			SysInfoFormViewModel viewModel = new SysInfoFormViewModel();
			this.DataContext = viewModel;
		}

		private string _groupName;
		public SysInfoForm(string GroupName)
		{
			InitializeComponent();
			_groupName = GroupName;
			SysInfoFormViewModel viewModel = new SysInfoFormViewModel();
			this.DataContext = viewModel;
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.DataContext == null)
			{
				return;
			}
			SysInfoFormViewModel viewModel = this.DataContext as SysInfoFormViewModel;
			if (viewModel.SysGroupList == null)
			{
				return;
			}
			foreach (var sysGroup in viewModel.SysGroupList)
			{
				if ((sysGroup.DataContext as SysGroupViewModel).Children == null)
				{
					continue;
				}				
					AddSysInfo(sysGroup);
			

				

			}


		}

		private void AddSysInfo(SysGroup sysGroup)
		{
			
			Expander expander = new Expander
			{
				Header = sysGroup.SysGroupName,
				Background = (Brush)this.FindResource("ExpanelHeaderBg")

			};
			if (!string.IsNullOrEmpty(_groupName))
			{
				if (sysGroup.SysGroupName == _groupName)
				{
					expander.IsExpanded = true;
				}
				else
				{
					expander.IsExpanded = false;
				}
			}
			else
			{
				expander.IsExpanded = true;
			}

			WrapPanel wrapPanel = new WrapPanel
			{
				Background = (Brush)this.FindResource("ExpanelContentBg")
			};
			expander.Content = wrapPanel;
			sPanel.Children.Add(expander);
			foreach (var sys in (sysGroup.DataContext as SysGroupViewModel).Children)
			{
				
				sys.Margin = new Thickness(10);
				sys.Width = 100;
				sys.Height = 100;
				try
				{
					if (sys.Parent!=null)
					{
						(sys.Parent as WrapPanel).Children.Clear();
					}
					wrapPanel.Children.Add(sys);
				}
				catch (Exception ex)
				{
					
					throw ex;
				}
			
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
		
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			
		}
	}
}
