using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace WHSE.Monitor.Framework.UserControls
{
	public class StackingCrane : StackingCraneFacadeChooseBase
	{

		public StackingCrane()
		{
			//StackingCraneOperation = new StackingCraneOperationImp();
		}
		public override DoubleAnimationBase getPath(DeviceName from, DeviceName to)
		{
			return base.getPath(from, to);
		}

		public override string UserControlName
		{
			get
			{
				return base.UserControlName;
			}
			set
			{
				string name;
				_userControlName = value;
				lb_DeviceName.Content = value;
				if (value.Contains(_headName))
				{
					name = value;
				}
				else
				{
					name = _headName + value;
				}

				if (!string.IsNullOrEmpty(value))
				{

					if (StackingCraneViewModelList.Instance.ViewModelList.Where(x => x.DeviceName == name).Count() > 0)
					{
						this.DataContext = StackingCraneViewModelList.Instance.ViewModelList.Where(x => x.DeviceName == name).FirstOrDefault();
					}
					else
					{
						//StackingCraneViewModel viewModel = new StackingCraneViewModel();
						_viewModel.DeviceName = name;
						this.DataContext = _viewModel;
						StackingCraneViewModelList.Instance.ViewModelList.Add(_viewModel);
					}

				}


			}
		}
		public override void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ShowDetails();
		}

		private void ShowDetails()
		{
			DeviceInfoForm form = new DeviceInfoForm(DataContext as DeviceBase);
			form.Topmost = true;
			form.ShowDialog();
			
		}

		public override void btn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowDetails();
		}
		

	}
}
