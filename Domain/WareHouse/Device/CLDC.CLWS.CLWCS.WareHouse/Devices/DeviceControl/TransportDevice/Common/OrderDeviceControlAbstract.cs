using System;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common
{
	/// <summary>
	/// 具有状态变化的设备控制虚类
	/// </summary>
	public abstract class OrderDeviceControlAbstract : DeviceControlBaseAbstract
	{
        public Action<TaskExcuteStatusType> NotifyDeviceExceptionEvent;
		public abstract bool IsLoad();

		public abstract OperateResult ClearFaultCode();

		public virtual bool IsConnected()
		{
			return true;
		}

		public abstract OperateResult SetControlState(ControlStateMode destState);
        public abstract OperateResult SetAbleState(UseStateMode destState);

        public abstract OperateResult IsCanChangeAbleState(UseStateMode destState);

		public virtual OperateResult ChangeOrderType(OrderTypeEnum type)
		{
			return OperateResult.CreateSuccessResult();
		}

		public virtual OperateResult CanChangeOrderType(OrderTypeEnum type)
		{
			return OperateResult.CreateSuccessResult();
		}


		

		public abstract Package GetCurrentPackage();


	    /// <summary>
	    /// 注册PLC监控的值变化
	    /// </summary>
	    /// <param name="dbBlockEnum"></param>
	    /// <param name="monitervaluechange"></param>
	    /// <returns></returns>
	    public abstract  OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange);

	    /// <summary>
	    /// 注册PLC监控的Bool变化
	    /// </summary>
	    /// <param name="dbBlockEnum"></param>
	    /// <param name="monitervaluechange"></param>
	    /// <returns></returns>
	    public abstract OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum,CallbackContainOpcBoolValue monitervaluechange);

		public abstract OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum,int startValue,int endValue, MonitorSpecifiedOpcValueCallback callbackAction);

		/// <summary>
		/// 下发指令到设备
		/// </summary>
        /// <param name="transMsg"></param>
		/// <returns></returns>
        public abstract OperateResult DoJob(TransportMessage transMsg);


        public abstract OperateResult<int> GetFinishedOrder();
    }
}
