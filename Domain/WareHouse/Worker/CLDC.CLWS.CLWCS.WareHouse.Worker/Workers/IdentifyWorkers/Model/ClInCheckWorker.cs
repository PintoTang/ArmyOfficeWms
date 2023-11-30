using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Station.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class ClInCheckWorker : InCheckWorkerAbstract
    {
       
       
        public OperateResult<SizeProperties> GetGoodsProperties(Addr curAddr)
        {
            List<AssistantDevice> assistants = GetAssistantByDeviceTypeAndAddress(DeviceTypeEnum.LoadDevice, curAddr);
            if (assistants != null && assistants.Count > 0)
            {
                if (assistants.Count > 1)
                {
                    LogMessage(string.Format("找到地址：{0} 类型：{1} 相同的协助者，请检查配置文件", curAddr, DeviceTypeEnum.LoadDevice), EnumLogLevel.Error, true);
                    return OperateResult.CreateFailedResult<SizeProperties>("无数据");
                }
                StationDeviceAbstract getPropertiesDevice = assistants[0].Device as StationDeviceAbstract;
                OperateResult<SizeProperties> propertiesResult = getPropertiesDevice.GetGoodsProperties();
                return propertiesResult;
            }
            else
            {
                return OperateResult.CreateFailedResult<SizeProperties>("无数据");
            }
        }

        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<string> identifyMsg, params object[] para)
        {

            //此处需要判断，条码是否已经上报成功，解决上报成功后没有生成指令，然后重启了WCS
            string barcode = string.Join(",", identifyMsg);
            string barcodeMsg = string.Format("收到条码信息：{0}", string.Join(",", identifyMsg));
            LogMessage(barcodeMsg, EnumLogLevel.Info, true);
            if (!WorkerBusiness.IsNeedHandleBarcode(device, barcode))
            {
                string msg = string.Format("条码：{0} 已上报，不重新上报", barcodeMsg);
                LogMessage(msg, EnumLogLevel.Info, true);
                return OperateResult.CreateSuccessResult(msg);
            }
            SendContentToScreen(barcode);

            OperateResult<SizeProperties> getPropertiesResult = GetGoodsProperties(device.CurAddress);
            if (!getPropertiesResult.IsSuccess)
            {
                string msg = string.Format("获取货物的属性失败：{0}", getPropertiesResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg);
            }

            OperateResult handleBarcode = WorkerBusiness.HandleIdentifyMsg(device, WorkerAssistants, barcode, getPropertiesResult.Content);
            if (handleBarcode.IsSuccess)
            {
                LogMessage(handleBarcode.Message, EnumLogLevel.Info, true);
                if (handleBarcode.ErrorCode.Equals(100))
                {
                    string screemMsg = string.Format("货物回退，回退原因：{0}", handleBarcode.Message);
                    SendContentToScreen(screemMsg);
                    LogMessage(screemMsg, EnumLogLevel.Info, true);
                }
                else
                {
                    string screemMsg = string.Format("条码：{0} 业务处理成功", barcode);
                    SendContentToScreen(screemMsg);
                    LogMessage(screemMsg, EnumLogLevel.Info, true);
                }
            }
            else
            {
                LogMessage(handleBarcode.Message, EnumLogLevel.Error, true);
                SendContentToScreen(string.Format("条码：{0} 业务处理失败", barcode));
            }
            return handleBarcode;

        }
    }
}
