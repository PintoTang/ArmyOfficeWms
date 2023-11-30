using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Client.BizService.Abs;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Client.Model.DataPool
{
	public class SysStateDataPool : BaseDataPool<string/*sysName*/, SysStateDataAndDelegateInfo>
	{
		#region 折叠单例模式的实现代码
		private static SysStateDataPool _instance;
		private SysStateDataPool()
		{
		}

		public static SysStateDataPool Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SysStateDataPool();
				}
				return _instance;
			}
		}

		public static void TI_ClearInstance()
		{
			_instance = null;
		}
		#endregion

		ISysStateOutput _sysStateOutputAbs = null;

        public void SetSysStateOutputAbs(ISysStateOutput sysStateOutputAbs)
		{
			this._sysStateOutputAbs = sysStateOutputAbs;
			this._sysStateOutputAbs.SysStateOutputEvent += SysStateDataInput;
		}

		private bool SysStateDataInput(string sysName, SysStateEnum state)
		{
			CreateNewValueIfNotInDict(sysName);
			_dataPoolDict[sysName].SysState = state;//当界面控件还打开时，协议就通知系统状态。此时先将系统状态暂存到数据池中，等后期界面使用。
			return true;
		}

		public void RegisterViewRefreshDelegate(string sysName, SysStateViewRefreshDelegate sysStateViewRefreshDelegate)
		{
			if(string.IsNullOrEmpty(sysName))
			{
				string msg = "界面层尝试注册一个空系统名的回调委托，说明界面层有严重BUG。需要界面层将该方法的调用迁移到已经能获取到sysName的位置调用。";
				Log.getDebugFile().Debug(msg);
				throw new Exception(msg);
			}

			CreateNewValueIfNotInDict(sysName);
			_dataPoolDict[sysName].SysStateViewRefreshDelegate = sysStateViewRefreshDelegate;
		}

		public DateTime GetLatestHeartbeatTime(string sysName)
		{
			return _sysStateOutputAbs.GetLatestHeartbeatTime(sysName);
		}

		public SysStateEnum GetSysState(string sysName)
		{
			SysStateDataAndDelegateInfo sysStateDataAndDelegateBean = GetDataPoolRecord(sysName);
			
			if (null != sysStateDataAndDelegateBean)
			{
				return sysStateDataAndDelegateBean.SysState;
			}
			else
			{
				return SysStateEnum.OFFLINE;
			}
		}
	}
}
