using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
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
using WHSE.Monitor.Client.Model.DataPool;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// Roller02.xaml 的交互逻辑
	/// </summary>
	public partial class Roller_Base : DeviceSimulation
	{
		#region 字段

		private bool _isRepeat;
		protected RollerViewModel _viewModel = null;

		#endregion
		public Roller_Base()
		{
			InitializeComponent();
			_viewModel = new RollerViewModel();
			DataContext = _viewModel;
		}

		#region 属性-设备名称
		private string _userControlName;
		/// <summary>
		/// 控件名
		/// 自动补全PLC#
		/// </summary>		
		public string UserControlName
		{
			get { return _userControlName; }
			set
			{
				_userControlName = value;
				lb_DeviceName.Content = value;
				if (!string.IsNullOrEmpty(value))
				{
					if (value.Substring(0, 1) == "[" && value.Substring(value.Length - 1, 1) == "]")
					{
						FullDeviceName = DeviceType + value.Substring(1, value.Length - 2);
						_isRepeat = true;
					}
					else
					{
						FullDeviceName = DeviceType + value;
					}
					CreateViewModel();
				}
			}
		}

		#endregion		
		

		#region 属性-完整名称
		private string fullDeviceName;
		/// <summary>
		/// 完整的设备名称
		/// </summary>
		public string FullDeviceName
		{
			get { return fullDeviceName; }
			set { fullDeviceName = value; }
		}

		#endregion

		#region 属性-是否显示设备名称

		private bool _isShowDeviceName;
		/// <summary>
		/// 是否显示设备名
		/// </summary>
		public bool IsShowDeveiceName
		{
			get { return _isShowDeviceName; }
			set
			{
				_isShowDeviceName = value;
				if (value)
				{					
					grid_label.Visibility = Visibility.Visible;
				}
				else
				{
					grid_label.Visibility = Visibility.Hidden;
				}

			}
		}
		#endregion

		#region 属性-辊筒设备宽度


		private double _borderWidth = 3;

		/// <summary>
		/// 辊筒设备边框宽度
		/// </summary>
		public double BorderWidth
		{
			get
			{
				return _borderWidth;
			}
			set
			{
				_borderWidth = value;
				//rcBorder.StrokeThickness = value;
			}
		}

		#endregion

		#region 方法
		private void CreateViewModel()
		{

			if (_isRepeat == true)
			{
				if ((RollerViewModelList.Instance.ViewModelList.Where(x => x.DeviceName == FullDeviceName).Count()) > 0)
				{
					_viewModel = RollerViewModelList.Instance.ViewModelList.Where(x => x.DeviceName == FullDeviceName).FirstOrDefault();
					DataContext = _viewModel;

				}
				else
				{
					_viewModel = NewViewModel();
					RollerViewModelList.Instance.ViewModelList.Add(_viewModel);
				}
			}
			else
			{
				_viewModel = NewViewModel();
			}
		}

		private RollerViewModel NewViewModel()
		{
			RollerViewModel viewModel = new RollerViewModel();
			DataContext = viewModel;
			viewModel.DeviceName = FullDeviceName;

			return viewModel;
		}
		private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ShowDetailForm();
		
		}

		private void ShowDetailForm()
		{
			if (!string.IsNullOrEmpty(UserControlName))
			{
				DeviceInfoForm deviceInfoForm = new DeviceInfoForm(DataContext as DeviceBase);
				deviceInfoForm.Topmost = true;
				deviceInfoForm.ShowDialog();
			}
		}

		
		private const string DeviceType = "PLC#";

		

		private void btn_Task_Click(object sender, RoutedEventArgs e)
		{
			ShowDetailForm();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			TransformHelper.ResetAngle(this, lb_DeviceName);

			CreateRollerLines();
		
		}

		private void CreateRollerLines()
		{
			double x = this.Width;
			int count = (int)(x / (sRec.Width + sRec.Width / 2));
			for (int i = 0; i < count; i++)
			{
				string xaml = System.Windows.Markup.XamlWriter.Save(sRec);
				Rectangle rec = System.Windows.Markup.XamlReader.Parse(xaml) as Rectangle;
				rec.SnapsToDevicePixels = true;
				sPanel.Children.Add(rec);
			}
		}
		
		#endregion
	}

	
}
