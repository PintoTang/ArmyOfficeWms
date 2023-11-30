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
    public class ApplyContainerWorkerBusiness : SwitchingWorkerBusinessAbstract
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
            if (newValue == 1)//等于1才申请容器
            {
                SwitchDeviceAbstract switchDevice=device as SwitchDeviceAbstract;
                try
                {
                    OperateResult<string> wmsAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(device.CurAddress.FullName);
                    if (!wmsAddress.IsSuccess)
                    {
                        return OperateResult.CreateFailedResult(string.Format("Wcs地址：{0} 转换Wms的地址失败，失败原因：\r\n{1}", device.CurAddress.ToString(), wmsAddress.Message), 1);
                    }
                    int containerType = 1;
                    if (switchDevice != null)
                    {
                        var dataBlockResult=switchDevice.DeviceControl.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                        if (dataBlockResult.IsSuccess)
                        {
                            containerType = dataBlockResult.Content;
                        }
                    }
                    ApplyContainerModel cmd = new ApplyContainerModel { CurrAddr = device.CurAddress.FullName, ContainerType = containerType, };
                    string cmdPara = cmd.ToJson();
                    string interFaceName = "NotifyApplyContainer";
                    NotifyElement element = new NotifyElement("", interFaceName, "申请容器出库", null, cmdPara);
                    OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

                    if (!result.IsSuccess)
                    {
                        string msg = string.Format("调用上层接口:{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                        return OperateResult.CreateFailedResult(msg, 1);
                    }
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (!serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口 " + interFaceName + "成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                    return OperateResult.CreateSuccessResult("调用接口:" + interFaceName + "成功，并且WMS返回成功信息");
                }
                catch (Exception ex)
                {
                    return OperateResult.CreateFailedResult(OperateResult.ConvertException(ex));
                }
            }
            return OperateResult.CreateSuccessResult();

        }
    }
}
