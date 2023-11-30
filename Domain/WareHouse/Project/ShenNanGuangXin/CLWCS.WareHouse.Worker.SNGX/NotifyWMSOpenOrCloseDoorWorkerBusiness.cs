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

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    ///plc开关门通知wms
    /// </summary>
    public class NotifyWMSOpenOrCloseDoorWorkerBusiness : SwitchingWorkerBusinessAbstract
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
            DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(3010);
            TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
            RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
            if (device.Id == 202003)
            {
                OperateResult<int> opcResult2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OpcDoorOpen3);
                if (opcResult2020.IsSuccess)
                {
                    //通知wms
                    return NotifyWmsDoorStatusChange(3, opcResult2020.Content);
                }
                else
                {
                    LogMessage("读取2020 指令ID OPC OpcDoorOpen3 失败！+" + opcResult2020.Message, EnumLogLevel.Error, true);
                }
            }
            else if (device.Id == 202004)
            {
                OperateResult<int> opcResult2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OpcDoorOpen4);
                if (opcResult2020.IsSuccess)
                {
                    //通知wms
                    return NotifyWmsDoorStatusChange(4, opcResult2020.Content);
                }
                else
                {
                    LogMessage("读取2020 指令ID OPC OpcDoorOpen4 失败！+" + opcResult2020.Message, EnumLogLevel.Error, true);
                }
            }
            else if (device.Id == 202005)
            {
                OperateResult<int> opcResult2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OpcDoorOpen5);
                if (opcResult2020.IsSuccess)
                {
                    //通知wms
                    return NotifyWmsDoorStatusChange(5, opcResult2020.Content);
                }
                else
                {
                    LogMessage("读取2020 指令ID OPC OpcDoorOpen5 失败！+" + opcResult2020.Message, EnumLogLevel.Error, true);
                }
            }
            else if (device.Id == 202006)
            {
                OperateResult<int> opcResult2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OpcDoorOpen6);
                if (opcResult2020.IsSuccess)
                {
                    //通知wms
                    return NotifyWmsDoorStatusChange(6, opcResult2020.Content);
                }
                else
                {
                    LogMessage("读取2020 指令ID OPC OpcDoorOpen6 失败！+" + opcResult2020.Message, EnumLogLevel.Error, true);
                }
            }

            return OperateResult.CreateSuccessResult("成功1234");
        }

        /// <summary>
        /// 通知wms开关门 
        /// </summary>
        /// <param name="postionNum">刷卡位置 1-6</param>
        /// <param name="opValue">1开， 2关</param>
        /// <returns>通知结果</returns>
        private OperateResult NotifyWmsDoorStatusChange(int postionNum, int opValue)
        {
            OpenOrCloseDoorCmd openOrCloseDoorCmd = new OpenOrCloseDoorCmd { PostionNum = postionNum, ActionNum = opValue };
            string cmdPara = JsonConvert.SerializeObject(openOrCloseDoorCmd);
            string interFaceName = "OpenOrCloseDoor";
            NotifyElement element = new NotifyElement("", interFaceName, "开关门通知wms", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            else
            {
                try
                {
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult("调用接口 " + interFaceName + "成功，并且WMS返回成功信息");
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口  " + interFaceName + " 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            return result;
        }
    }
}
