using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
	/// AreaEntry.xaml 的交互逻辑
	/// </summary>
	public partial class AreaEntry : UserControl
	{
		AreaEntryViewModel viewModel;
		public AreaEntry()
		{
			InitializeComponent();
			viewModel = new AreaEntryViewModel();
			DataContext = viewModel;			
		}

		private Grid mainBox;

		private string _areaClassName;
		/// <summary>
		/// 区域类名
		/// </summary>
		public string AreaClassName
		{
			get { return _areaClassName; }
			set { _areaClassName = value; }
		}


		private string _title;
		/// <summary>
		/// 区域名称
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				labAreaName.Content = value;
				viewModel.AreaName = value;
				
			}
		}





		UserControl area = null;

		private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			InitChildren();
			ShowAreaChild();
		}

		private void InitChildren()
		{
			if (string.IsNullOrEmpty(AreaClassName))
			{
				MessageBox.Show("未设置区域的AreaClassName");
				return;
			}

			FindViewBox();
			if (mainBox == null)
			{
				MessageBox.Show("未能找到主框架容器");
				return;
			}

			RefreshViewBox();
			
		}
		private void ShowAreaChild()
		{
			if (area!=null)
			{				
				mainBox.Children.Clear();
				area.Width = mainBox.ActualWidth;
				area.Height = mainBox.ActualHeight;
				mainBox.Children.Add(area);
			}
			
		}
		private void RefreshViewBox()
		{
			var query = AreaControlList.Instance.AreaBeanList.Where(x => x.AreaName == Title).FirstOrDefault();
			if (query != null)
			{
				if (query.AreaUserControl != null)
				{
					area = query.AreaUserControl;
				}
				else
				{
					MessageBox.Show("找不到区域主界面 " );
				}
			}
			else
			{
				MessageBox.Show("找不到区域主界面，请检测配置文件AreaConfig.xml!");
			}		
		
		}

		private void FindViewBox()
		{
			if (mainBox != null)
			{
				return;
			}	
			DependencyObject parent = VisualTreeHelper.GetParent(this);

			while (parent != null)
			{
				if (parent is Grid)
				{
					Grid grid = (Grid)parent;
					if (grid.Name == "MainBodyView")
					{
						mainBox = grid;
						mainBox.SizeChanged += MainBodyView_SizeChanged;
						break;
					}
				}
				parent = VisualTreeHelper.GetParent(parent);
			}

		}

	

		private void MainBodyView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (mainBox != null && area != null)
			{
				area.Width = mainBox.ActualWidth;
				area.Height = mainBox.ActualHeight;
			}
		}

		private void Area_Loaded(object sender, RoutedEventArgs e)
		{

		
		}
		
		private void FindDevice(object parent)
		{

			Grid grid = parent as Grid;

			int count = VisualTreeHelper.GetChildrenCount(grid);
			for (int i = 0; i < count; i++)
			{
				var control = VisualTreeHelper.GetChild(grid, i);
				if (control is Grid)
				{
					FindDevice(control);
				}
				else
				{
					PropertyInfo propertyInfo = control.GetType().GetProperty("UserControlName");
					if (propertyInfo != null)
					{
						viewModel.DeviceList.Add(control);
						AreaBean query = AreaControlList.Instance.AreaBeanList.Where(x => x.AreaName == Title).FirstOrDefault();
						if (query != null)
						{
							query.Children.Add((UserControl)control);

						}

					}
				}
			}
		}

	}
}
