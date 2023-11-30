using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common
{
    public abstract class StackRfidControlAbstract : DeviceControlBaseAbstract
    {
        /// <summary>
        /// 下发命令
        /// </summary>
        /// <param name="webApiCmd"></param>
        /// <returns></returns>
        public abstract OperateResult<string> SendCmd<TResponse>(WebApiInvokeCmd webApiCmd) where TResponse:IResponse,new();
        public string Http { get; set; }
    }
}
