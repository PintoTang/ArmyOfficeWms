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

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// HoisterBase.xaml 的交互逻辑
	/// </summary>
	public partial class HoisterBase : UserControl
	{
		protected const string _headName = "PLC#";
		protected HoisterViewModel _viewmodel=new HoisterViewModel();
		public HoisterBase()
		{
			InitializeComponent();
			DataContext = _viewmodel;
		}




		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			TransformHelper.ResetAngle(this, lb_DeviceName);			
		}

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

		protected virtual void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{

		}
		

		


	}
}
