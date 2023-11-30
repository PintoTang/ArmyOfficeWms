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
    public class DoorOpenCloseFinishedWorkerBusiness : SwitchingWorkerBusinessAbstract
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
            //1.AGV请求进入，请求开门
            //2.允许AGV进入，已开门
            //3.AGV进入完成，请求关门
            //4.AGV进入完成，已关门

            //5.AGV请求离开，请求开门
            //6.允许AGV离开，已开门
            //7.AGV离开完成，请求关门
            //8.AGV离开完成，已关门
            if (newValue > 0)
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
                    DoorOpenOrCloseFinishedModel cmd = new DoorOpenOrCloseFinishedModel();
                    if (newValue == 2) //2.允许AGV进入，已开门
                    {
                        cmd.IsIn = true;
                        cmd.IsOpen = true;
                    }else if (newValue == 4) //4.AGV进入完成，已关门
                    {
                        cmd.IsIn = true;
                        cmd.IsOpen = false;
                    }
                    else if (newValue == 6)//6.允许AGV离开，已开门
                    {
                        cmd.IsIn = false;
                        cmd.IsOpen = true;
                    }
                    else if (newValue == 8)//8.AGV离开完成，已关门
                    {
                        cmd.IsIn = false;
                        cmd.IsOpen = false;
                    }
                    else
                    {
                        string msg = string.Format("未知状态：{0}", newValue.ToString());
                        return OperateResult.CreateFailedResult(msg, 1);
                    }
                    string cmdPara = cmd.ToJson();
                    string interFaceName = "NotifyDoorOpenOrCloseFinished";
                    NotifyElement element = new NotifyElement("", interFaceName, "开关门完成上报", null, cmdPara);
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
                    switchDevice.DeviceControl.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                    return OperateResult.CreateSuccessResult("调用接口:" + interFaceName + "成功，并且WMS返回成功信息");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    return OperateResult.CreateFailedResult(OperateResult.ConvertException(ex));
                }
                finally
                {
                }
            }
            return OperateResult.CreateSuccessResult();

        }
    }
}
