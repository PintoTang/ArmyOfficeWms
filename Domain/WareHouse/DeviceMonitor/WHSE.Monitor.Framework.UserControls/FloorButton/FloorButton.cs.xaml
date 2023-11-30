using System;
using System.Collections.Generic;
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

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// FloorButton.xaml 的交互逻辑
	/// </summary>
	public partial class FloorButton : UserControl
	{
		public FloorButton()
		{
			InitializeComponent();

		}

		private string _floor;
		/// <summary>
		/// 楼层名称
		/// </summary>
		public string FloorName
		{
			get
			{
				return _floor;
			}
			set
			{
				_floor = value;
				rb.Content = value;
				

			}
		}

		private string _areaName;
		/// <summary>
		/// 控件所在的区域
		/// </summary>
		public string AreaName
		{
			get { return _areaName; }
			set
			{
				_areaName = value;
				rb.GroupName = value;
			}
		}
		

		private bool _isDefaultFloor;

		/// <summary>
		/// 是否默认显示当前楼层
		/// </summary>
		public bool IsDefalutFloor
		{
			get { return _isDefaultFloor; }
			set
			{
				_isDefaultFloor = value;
				rb.IsChecked = value;

			}
		}


		private string _floorClassName;

		public string FloorClassName
		{
			get { return _floorClassName; }
			set { _floorClassName = value; }
		}

		//private string _projectName;

		//public string ProjectName
		//{
		//	get { return _projectName; }
		//	set { _projectName = value; }
		//}




		private void rb_Click(object sender, RoutedEventArgs e)
		{
			if (rb.IsChecked == true)
			{
				AddFloorToArea();
			}

		}
		Viewbox viewbox = null;
		private void AddFloorToArea()
		{
			
				if (viewbox!=null)
				{
					viewbox.Child = FloorUserControl;
					return;
				}
				DependencyObject parent = VisualTreeHelper.GetParent(this);				
				while (parent != null)
				{
					bool reslut = false;

					if (parent is Grid)
					{
						Grid grid = parent as Grid;
						foreach (var item in grid.Children)
						{
							if (item is Viewbox)
							{
								viewbox = item as Viewbox;
								viewbox.Child = FloorUserControl;							
								reslut = true;
								Console.WriteLine(viewbox.Name);
								break;
							}
						}
					}
					
					if (reslut)//跳出while循环体
					{
						break;
					}
					parent = VisualTreeHelper.GetParent(parent);
				}
			

		}


		private UserControl _areaFloorControl;

		public UserControl AreaFloorControl
		{
			get { return _areaFloorControl; }
			set { _areaFloorControl = value; }
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			InitFloor();

			if (IsDefalutFloor)
			{
				AddFloorToArea();
			}
		}

		private void InitFloor()
		{
			//if (FloorUserControl!=null)
			//{
			//	AreaFloorControl = FloorUserControl;
			//	return;
			//}


			if (string.IsNullOrEmpty(FloorClassName) )
			{
				return;
			}
			if (FloorUserControl == null)
			{				
			    var floor=	FloorControlList.Instance.FloorBeanList.Where(x => x.FloorClassName == FloorClassName).FirstOrDefault();
				if (floor!=null)
				{
					FloorUserControl = floor.FloorUserControl;
				}
				
			}

		}

		private UserControl _floorUserControl;

		public UserControl FloorUserControl
		{
			get { return _floorUserControl; }
			set { _floorUserControl = value; }
		}
		

	}
}
