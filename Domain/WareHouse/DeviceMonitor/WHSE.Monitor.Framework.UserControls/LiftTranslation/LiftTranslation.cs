using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls.LiftTranslation
{
	public class LiftTranslation : LiftTranslationFacadeChooseBase
	{
		public LiftTranslation()
		{

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
					_viewmodel.DeviceName = name;
					DataContext = _viewmodel;
					//if (HoisterViewModelList.Instance.ViewModelList.Where(x => x.DeviceName == name).Count() > 0)
					//{
					//	this.DataContext = HoisterViewModelList.Instance.ViewModelList.Where(x => x.DeviceName == name).FirstOrDefault();
					//}
					//else
					//{
					//	_viewmodel = new LiftTranslationViewModel();
					//	_viewmodel. = name;
					//	this.DataContext = _viewmodel;
					//	HoisterViewModelList.Instance.ViewModelList.Add(_viewmodel);
					//}

				}


			}
		}


		protected override void UserControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DeviceInfoHelper.ShowDetails(DataContext as DeviceBase);

		}
		
	}
}
