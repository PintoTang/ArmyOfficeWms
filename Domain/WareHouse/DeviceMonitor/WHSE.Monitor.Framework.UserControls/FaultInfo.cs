using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using CL.WCS.Monitor.DataModel;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;
using Framework.LanguageConverter;
using CL.WCS.Monitor.DataPool;

namespace WHSE.Monitor.Framework.UserControls
{
	public class FaultInfo
	{

		#region IntanceCode
		private static FaultInfo instance;
		private FaultInfo()
		{

		}
		public static FaultInfo Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new FaultInfo();
				}
				return instance;
			}

		}
		#endregion



		public void ChangeDeviceFaultState(string device, DeviceStateBean state)
		{

			NotifyFaultTable(device, state);
			NotifyAreaState();			
			
		}

		private void NotifyFaultTable(string device, DeviceStateBean state)
		{			

			if (DataSource.Where(x => x.DeviceName == device).Count() > 0)
			{
				if (state.FaultStateList.Where(x => x > 0).Count() > 0)
				{
					FaultInfoBean bean = DataSource.Where(x => x.DeviceName == device).FirstOrDefault();
					bean.FaultMessage = FaultMessageConvertor.FaultInfoToString(state.FaultStateList);
					bean.FaultLevel = FaultLevelEnum.Level_2;
					bean.FaultTime = state.LastRxFaultTime;
					
				}
				else
				{
					FaultInfoBean bean = DataSource.Where(x => x.DeviceName == device).FirstOrDefault();
					Application.Current.Dispatcher.Invoke(new Action(() =>
					{
						dataSource.Remove(bean);
					}));
				}

			}
			else
			{
				if (state.FaultStateList != null)
				{


					if (state.FaultStateList.Where(x => x > 0).Count() > 0)
					{
						FaultInfoBean bean = new FaultInfoBean();
						bean.DeviceName = device;
						bean.FaultMessage = FaultMessageConvertor.FaultInfoToString(state.FaultStateList);
						bean.FaultLevel = FaultLevelEnum.Level_2;
						bean.FaultTime = state.LastRxFaultTime;
						bean.DeviceName = device;
						Application.Current.Dispatcher.Invoke(new Action(() =>
						{
							DataSource.Add(bean);

						}));
					}
				}
		
			}


		}

	


		private ObservableCollection<FaultInfoBean> dataSource = new ObservableCollection<FaultInfoBean>();

		public ObservableCollection<FaultInfoBean> DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}

		public  delegate void RefreshAreaState(string areaName);

		public event Action ChangeAreaStateEnvent;
		public void NotifyAreaState()
		{
			if (ChangeAreaStateEnvent != null)
			{
				ChangeAreaStateEnvent();
			}
		}

		public DeviceDisplayStateEnum AreaDisplayState(string areaName)
		{

			return DeviceDisplayStateEnum.Normal;

		}




	}




}
