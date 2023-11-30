using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class TransportWorkerBusiness : OrderWorkerBuinessAbstract
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

        public override OperateResult AfterTransportBusinessHandle(TransportMessage transport)
        {
            //1.清除开始设备对应地址的所有设备的相关信息
            DeviceBaseAbstract startDevice = transport.StartDevice;
            if (startDevice == null)
            {
                return OperateResult.CreateFailedResult(string.Format("搬运信息 {0} 开始设备为空", transport.UniqueCode));
            }
            OperateResult clearResult = startDevice.ClearUpRunMessage(transport);
            return clearResult;
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

        public override OperateResult DeviceExceptionHandle(DeviceExceptionMessage exceptionMsg)
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
