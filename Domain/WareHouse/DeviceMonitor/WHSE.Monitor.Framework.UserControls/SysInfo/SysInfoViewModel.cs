using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.Monitor.DataModel;
//using WHSE.Monitor.Client.Model.DataPool;

namespace WHSE.Monitor.Framework.UserControls
{
	public class SysInfoViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 后台刷新状态的委托
		/// <para>SysName:系统名称</para>
		/// <para>SysStateEnum:系统状态</para>
		/// </summary>
		public Action<string, SysStateEnum> ChangeSysinfoState;
	

		private SysStateEnum _sysState = SysStateEnum.OFFLINE;
		public SysStateEnum SysState
		{
			get { return _sysState; }
			set
			{
				if (value != _sysState)
				{
					_sysState = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("SysState"));
						if (ChangeSysinfoState != null)
						{
							ChangeSysinfoState.Invoke(SysName, value);

						}

					}

				}
			}
		}

		private string _latestHeartbeatTime = "";
		public string LatestHeartbeatTime
		{
			get { return _latestHeartbeatTime; }
			set
			{
				if (value != _latestHeartbeatTime)
				{
					_latestHeartbeatTime = value;
					PropertyChanged(this, new PropertyChangedEventArgs("LatestHeartbeatTime"));

				}

			}
		}

		private string _ipAddress = "";
		/// <summary>
		/// IP地址
		/// </summary>
		public string IpAddress
		{
			get { return _ipAddress; }
			set
			{
				if (value != _ipAddress)
				{
					_ipAddress = value;
					PropertyChanged(this, new PropertyChangedEventArgs("IpAddress"));
				}
			}
		}

		private string _sysName;

		public string SysName
		{
			get { return _sysName; }
			set { _sysName = value; }
		}



	}
}
