using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;


namespace WHSE.Monitor.Client.Model.DataPool
{
	public delegate void SysStateViewRefreshDelegate(SysStateEnum state);

	public class SysStateDataAndDelegateInfo
	{
		private SysStateViewRefreshDelegate _sysStateViewRefreshDelegate = null;
		public SysStateViewRefreshDelegate SysStateViewRefreshDelegate
		{
			set
			{
				lock (this)
				{
					if (value != _sysStateViewRefreshDelegate)
					{
						_sysStateViewRefreshDelegate = value;
					}
				}
			}
		}

		private SysStateEnum _sysState = SysStateEnum.OFFLINE;
		public SysStateEnum SysState
		{
			get
			{
				return _sysState;
			}

			set
			{
				lock (this)
				{
					if (value != _sysState)
					{
						_sysState = value;
						if (null != _sysStateViewRefreshDelegate)
						{
							_sysStateViewRefreshDelegate(_sysState);
						}
					}
				}
			}
		}
	}
}
