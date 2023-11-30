using CL.Framework.CmdDataModelPckg;
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
	public class PackageMoveDataPool : BaseDataPool<string/*packageBarcode*/, PackageMoveInfoBean>
	{
		#region 折叠单例模式的实现代码
		private static PackageMoveDataPool _instance;
		private PackageMoveDataPool()
		{
		}

		public static PackageMoveDataPool Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PackageMoveDataPool();
				}
				return _instance;
			}
		}

		public static void TI_ClearInstance()
		{
			_instance = null;
		}
		#endregion

		IPackageMoveOutput _packageMoveOutputAbs = null;
		PackageMoveInfoOutputDelegate _packageMoveInfoOutputDelegate = null;

        public void SetPackageMoveOutputAbs(IPackageMoveOutput packageMoveOutputAbs)
		{
			this._packageMoveOutputAbs = packageMoveOutputAbs;
			this._packageMoveOutputAbs.PackageMoveInfoOutputEvent += this.PackageMoveInfoInput;
		}

		private bool PackageMoveInfoInput(List<PackageMoveInfoBean> packageMoveInfoBeanList)
		{
			foreach (PackageMoveInfoBean packageMoveInfoBean in packageMoveInfoBeanList)
			{
				string packageBarcode = packageMoveInfoBean.PackageBarcode;

				lock (_dataPoolDict)
				{
					if (_dataPoolDict.ContainsKey(packageMoveInfoBean.PackageBarcode))
					{
						DeviceName prevDevice = _dataPoolDict[packageBarcode].CurrDevice;//缓存的当前设备既为这次的前一个设备
						packageMoveInfoBean.PrevDevice = prevDevice;
						_dataPoolDict[packageBarcode] = packageMoveInfoBean;
					}
					else
					{
						_dataPoolDict.Add(packageMoveInfoBean.PackageBarcode, packageMoveInfoBean);
					}
				}
			}

			if (null != _packageMoveInfoOutputDelegate)
			{
				return _packageMoveInfoOutputDelegate(packageMoveInfoBeanList);
			}
			else
			{
				return true;
			}
		}

		public bool DeletePackageMoveInfoRecord(string packageBarcode)
		{
			return DeleteDataPoolRecord(packageBarcode);
		}

		public void RegisterViewRefreshDelegate(PackageMoveInfoOutputDelegate packageMoveInfoOutputDelegate)
		{
			_packageMoveInfoOutputDelegate = packageMoveInfoOutputDelegate;
		}
	}
}
