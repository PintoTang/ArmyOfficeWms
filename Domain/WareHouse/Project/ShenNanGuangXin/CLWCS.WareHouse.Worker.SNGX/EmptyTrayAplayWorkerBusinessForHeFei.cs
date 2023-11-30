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
using System.IO;
using CLDC.CLWS.CLWCS.Framework;
using System.Diagnostics;

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    ///入库 放货处 空托盘申请——>WMS
    /// </summary>
   public class EmptyTrayAplayWorkerBusinessForHeFei : SwitchingWorkerBusinessAbstract
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        private OrderManage _orderManage;
        string wcsWareHouseInFullPath = "";
        protected override OperateResult ParticularInitlize()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            _orderManage = DependencyHelper.GetService<OrderManage>();
            if (string.IsNullOrEmpty(wcsWareHouseInFullPath))
            {
                string strAppPath = Directory.GetCurrentDirectory();
                wcsWareHouseInFullPath = Path.Combine(strAppPath, @"SerialFile\wcsWareHouseInFiles.ini");
            }
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
            OperateResult<string> wmsAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(device.CurAddress.FullName);
            if (!wmsAddress.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("Wcs地址：{0} 转换Wms的地址失败，失败原因：\r\n{1}", device.CurAddress.ToString(), wmsAddress.Message), 1);
            }

            //string readValue = IniHelper.ReadIniData("3003", "IsStartTask", "", wcsWareHouseInFullPath);
            //if (readValue.Equals("false"))
            //{
            //    //不申请 空托盘
            //    return OperateResult.CreateFailedResult("WMS 未发开始任务状态，不处理空托盘业务");
            //}

            //做下判断 防止申请两次
            OperateResult<ExOrder> getOutTaskOrderForEmptyTray = _orderManage.ManagedDataPool.FindData(o => o.DestAddr.FullName.Equals("PutGoodsPort:1_1_1")
            && o.SourceTaskType == SourceTaskEnum.Out
            && (o.Status.Equals(StatusEnum.Waiting) || o.Status.Equals(StatusEnum.CmdSent)
            || o.Status.Equals(StatusEnum.NotifyOPC) || o.Status.Equals(StatusEnum.Processing)));
            if (getOutTaskOrderForEmptyTray.Content != null)
            {
                string msg = string.Format("未完成的任务中已存在空托盘出库到1008 不能触发调用WMS申请空托盘：任务号{0}", getOutTaskOrderForEmptyTray.Content.OrderId);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            ApplayEmptyTrayMode applayEmptyTrayMode = new ApplayEmptyTrayMode { Addr = "PutGoodsPort:1_1_1" };
            string cmdPara = JsonConvert.SerializeObject(applayEmptyTrayMode);
            string interFaceName = "ApplyEmptyTrayOut";
            NotifyElement element = new NotifyElement("", interFaceName, "空托盘出库申请", null, cmdPara);
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            sw2.Stop();
            LogMessage("WCS调用wms ApplyEmptyTrayOut  耗时：" + sw2.ElapsedMilliseconds + "毫秒", EnumLogLevel.Info, false);


            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口:{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            else
            {
                try
                {
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult("调用接口:" + interFaceName + "成功，并且WMS返回成功信息");
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口 " + interFaceName + "成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
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
