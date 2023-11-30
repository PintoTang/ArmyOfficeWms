using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.StackRfid;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor
{
    /// <summary>
    /// 射频门抽象类
    /// </summary>
    public abstract class RfidDoorAbstract : DeviceBaseForTask<StringCharTask>, IDoorDevice, IScanDevice, ITaskDevice<StringCharTask>
    {



        /// <summary>
        /// 下发命令
        /// </summary>
        /// <param name="webApiCmd"></param>
        /// <returns></returns>
        public OperateResult<string> SendCmd(WebApiInvokeCmd webApiCmd)
        {
            return DeviceControl.SendCmd<SyncResReMsg>(webApiCmd);
        }

        protected internal StringCharTaskAbstract StringCharTaskDal;
        protected internal StackRfidBusinessAbstract DeviceBusiness;
        protected internal StackRfidControlAbstract DeviceControl;

        public abstract OperateResult CloseDoor(string cmd);

        public abstract OperateResult OpenDoor(string cmd);

        public abstract OperateResult Scan(string cmd);

        public abstract OperateResult ShowMessage(string cmd);


        public abstract OperateResult FinishTask(StringCharTask task);

        public abstract OperateResult<StringCharTask> FinishTask(string taskCode);

        public abstract OperateResult DoTask(StringCharTask task);

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.DeviceBusiness = business as StackRfidBusinessAbstract;
            this.DeviceControl = control as StackRfidControlAbstract;
            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "StackRfidBusinessAbstract", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "StackRfidControlAbstract", control.GetType().FullName);

                return OperateResult.CreateFailedResult(msg, 1);
            }

            StringCharTaskDal = DependencyHelper.GetService<StringCharTaskAbstract>();

            return OperateResult.CreateSuccessResult();
        }
       
    }
}
