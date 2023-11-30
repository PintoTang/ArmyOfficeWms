using CL.Framework.CmdDataModelPckg;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WHSE.Monitor.Client.UserControls;
//using WHSE.Monitor.Framework.UserControls.Running.Abs;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// StackingCrane.xaml 的交互逻辑
	/// </summary>
	public partial class StackingCraneBase : UserControl, ILogisticsDeviceInfo
	{
		//public StackingCraneOperationAbs stackingCraneOperation;
		protected StackingCraneViewModel _viewModel=new StackingCraneViewModel ();
		public StackingCraneBase()
		{
			InitializeComponent();
			DataContext=_viewModel;
						
		}

		protected const string _headName = "StackingCrane#";
		protected string _userControlName;
		/// <summary>
		/// 控件名
		/// </summary>
		public virtual string UserControlName
		{
			get { return _userControlName; }
			set
			{
				_userControlName = _headName + value;
				lb_DeviceName.Content = value;

			}
		}



		private double _railWidth;
		/// <summary>
		/// 轨道宽度
		/// </summary>
		public double RailWidth
		{
			get { return _railWidth; }
			set
			{
				_railWidth = value;
				rec_Line.Height = value;
			}
		}
		


		protected bool _isShowDeviceName;
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
					lb_DeviceName.Visibility = Visibility.Visible;
				}
				else
				{
					lb_DeviceName.Visibility = Visibility.Hidden;
				}

			}
		}

		private UserControlLocation _location;

		public UserControlLocation UserControlNameLocation
		{
			get {
				return _location; 
			}
			set {
				_location = value;
				switch (value)
				{
					case UserControlLocation.StartingPosition:
						Grid.SetColumn(grid_Name, 0);						
						break;
					case UserControlLocation.EndPosition:
						Grid.SetColumn(grid_Name, 2);
						break;
					default:
						break;
				}
			}
		}
		



		public virtual void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
            //if (stackingCraneOperation != null)
            //{
            //    stackingCraneOperation.ShowDetailForm();
			
            //}

		}

		public virtual void btn_Click(object sender, RoutedEventArgs e)
		{

		}



		public virtual DoubleAnimationBase getPath(DeviceName from, DeviceName to)
		{
			throw new NotImplementedException();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			TransformHelper.ResetAngleUp(this, lb_DeviceName);
		}

		


	private bool _isEndDevice;

	public bool IsEndDevice
	{
		get { return _isEndDevice; }
		set
		{
			_isEndDevice = value;
			if (_viewModel != null)
			{
				_viewModel.IsEndDevice = value;
			}

		}
	}

	}
	public enum UserControlLocation
	{
		/// <summary>
		/// 原点位置
		/// </summary>
		StartingPosition,
		/// <summary>
		/// 反原点位置
		/// </summary>
		EndPosition

	}

}
