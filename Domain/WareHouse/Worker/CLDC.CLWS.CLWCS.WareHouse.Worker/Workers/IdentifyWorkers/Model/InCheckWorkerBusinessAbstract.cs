using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
    public abstract class InCheckWorkerBusinessAbstract : WorkerBusinessAbstract
    {
        public abstract OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> assistantDevices, string barcode, SizeProperties properties);

        /// <summary>
        /// 判断是否需要处理条码
        /// </summary>
        /// <param name="device"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public abstract bool IsNeedHandleBarcode(DeviceBaseAbstract device, string barcode);




    }
}
