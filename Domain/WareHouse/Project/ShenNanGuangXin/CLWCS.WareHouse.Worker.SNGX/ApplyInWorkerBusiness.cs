using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.Model;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.Framework;
using System.Diagnostics;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.Model;

namespace CLWCS.WareHouse.Worker.HeFei
{
    public class ApplyInWorkerBusiness : SwitchingWorkerBusinessAbstract
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        private OrderManage _orderManage;
        protected override OperateResult ParticularInitlize()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            _orderManage = DependencyHelper.GetService<OrderManage>();
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> workerAssistants, string barcode)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult HandleValueChange(DeviceBaseAbstract device, int newValue)
        {
            if (newValue == 1)//等于1才请求入库
            {
                bool isSuccess = true;
                SwitchDeviceAbstract switchDevice=device as SwitchDeviceAbstract;
                try
                {
                    OperateResult<string> wmsAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(device.CurAddress.FullName);
                    if (!wmsAddress.IsSuccess)
                    {
                        isSuccess = false;
                        return OperateResult.CreateFailedResult(string.Format("Wcs地址：{0} 转换Wms的地址失败，失败原因：\r\n{1}", device.CurAddress.ToString(), wmsAddress.Message), 1);
                    }
                    ApplyInModel cmd = new ApplyInModel { CurrAddr = device.CurAddress.FullName };
                    string cmdPara = cmd.ToJson();
                    string interFaceName = "NotifyApplyIn";
                    NotifyElement element = new NotifyElement("", interFaceName, "请求入库", null, cmdPara);
                    OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

                    if (!result.IsSuccess)
                    {
                        isSuccess = false;
                        string msg = string.Format("调用上层接口:{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                        return OperateResult.CreateFailedResult(msg, 1);
                    }
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (!serviceReturn.IsSuccess)
                    {
                        isSuccess = false;
                        return OperateResult.CreateSuccessResult(string.Format("调用接口 " + interFaceName + "成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                    return OperateResult.CreateSuccessResult("调用接口:" + interFaceName + "成功，并且WMS返回成功信息");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    return OperateResult.CreateFailedResult(OperateResult.ConvertException(ex));
                }
                finally
                {
                    if (!isSuccess)
                    {
                        switchDevice.DeviceControl.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                    }
                    else
                    {
                        switchDevice.DeviceControl.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 2);
                    }
                }
            }
            return OperateResult.CreateSuccessResult();

        }
    }
}
