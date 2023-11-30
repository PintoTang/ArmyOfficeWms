using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class ClRollerBusiness : OrderWorkerBuinessAbstract
    {
        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }
        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

     

        public override OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult CancleOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult BeforeTransportBusinessHandle(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
