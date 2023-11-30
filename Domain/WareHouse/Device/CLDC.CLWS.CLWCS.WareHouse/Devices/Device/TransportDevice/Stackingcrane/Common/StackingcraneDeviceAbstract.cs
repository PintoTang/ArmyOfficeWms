using System.Linq;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.Common
{
    public abstract class StackingcraneDeviceAbstract : TransportDeviceBaseAbstract
    {
        private int _curColumn = 5;

        public int CurColumn
        {
            get { return _curColumn; }
            set
            {
                _curColumn = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 当前执行的指令
        /// </summary>
        public TransportMessage CurTransport
        {
            get
            {
                return UnFinishedTask.DataPool.FirstOrDefault(t =>t.TransportOrder!=null && t.TransportOrder.Status!=CL.WCS.DataModelPckg.StatusEnum.Discard && t.TransportOrder.Status!=CL.WCS.DataModelPckg.StatusEnum.Cancle && t.TransportStatus.Equals(TransportResultEnum.UnFinish));
            }
        }

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.OrderDatabaseHandler = DependencyHelper.GetService<OrderDataAbstract>();
            this.DeviceLiveDataHandler = DependencyHelper.GetService<LiveStatusAbstract>();
            TransportManageHandler = DependencyHelper.GetService<ITransportManage>();
            DeviceBusiness = business as RobotTecStackingcraneBusiness;
            DeviceControl = control as RobotTecStackingcraneControl;
            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "RobotTecStackingcraneBusiness", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "RobotTecStackingcraneControl", control.GetType().FullName);

                return OperateResult.CreateFailedResult(msg, 1);
            }

            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Random rd = new Random();
            //        CurColumn = rd.Next(0, 19);
            //        Thread.Sleep(5000);
            //    }
            //}).Start();

            return OperateResult.CreateSuccessResult();
        }



        public abstract OperateResult ClearFaultCode();

        internal void DeviceExceptionHandle(TaskExcuteStatusType type)
        {
            //if (type.Equals(TaskExcuteStatusType.UnknowException))
            //{
            //    LogMessage("设备上报未知异常，无法手动恢复，请人工处理", EnumLogLevel.Error, true);
            //    return;
            //}
            TransportMessage exceptionTransport = CurTransport;
            if (exceptionTransport == null)
            {
                LogMessage(string.Format("设备底层上报了异常：{0} 但是没有找到异常执行指令信息", type), EnumLogLevel.Warning, true);
                return;
            }

            TaskExcuteMessage<TransportMessage> exceptionMsg = new TaskExcuteMessage<TransportMessage>(this, exceptionTransport, type,TaskExcuteStepStatusEnum.Finished);
            if (NotifyDeviceExceptionEvent == null)
            {
                LogMessage(string.Format("设备上报异常：{0} 但是NotifyDeviceExceptionEvent未注册", type), EnumLogLevel.Error, true);
                return;
            }


            OperateResult notifyResult = NotifyDeviceExceptionEvent(exceptionMsg);
            if (!notifyResult.IsSuccess)
            {
                LogMessage(string.Format("设备上报异常：{0} 异常处理的指令编号：{1}，上报WMS失败，失败原因：{2}", type, exceptionTransport.TransportOrderId, notifyResult.Message), EnumLogLevel.Info, true);
                return;
            }

            //1.清除设备异常代码
            OperateResult result = ClearFaultCode();
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("清除设备的异常代码失败，原因：{0} \r\n请人工清除", result.Message), EnumLogLevel.Error, true);
                return;
            }
            LogMessage(string.Format("设备异常：{0}  处理成功", type), EnumLogLevel.Info, true);
        }
        public override OperateResult ParticularStart()
        {

            OperateResult registerAll = RegisterValueChange();
            if (!registerAll.IsSuccess)
            {
                return registerAll;
            }

            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 注册设备值变化
        /// </summary>
        protected abstract OperateResult RegisterValueChange();

        /// <summary>
        /// 处理设备状态的业务逻辑
        /// </summary>
        /// <param name="newValue"></param>
        protected abstract void HandleDeviceWorkStatusChange(int newValue);


        /// <summary>
        /// 处理设备当前位置变化的业务逻辑
        /// </summary>
        /// <param name="newValue"></param>
        protected abstract void HandleDeviceCurColumnChange(int newValue);

        public abstract void HandleInProcessOrderValueChange();

    }
}
