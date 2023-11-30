using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class BoxAndMeterCheckBusiness : InCheckWorkerBusinessAbstract
    {
        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> assistantDevices, string barcode,
            SizeProperties properties)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool IsNeedHandleBarcode(DeviceBaseAbstract device, string barcode)
        {
            return true;
        }




    }
}
