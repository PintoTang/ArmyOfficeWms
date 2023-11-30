using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model;
using CLDC.Infrastructrue.Xml;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CLWCS.WareHouse.Worker.HeFei
{
    class InCheckWorkerBusinessForHeFei : InCheckWorkerBusinessAbstract
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        private LiveDataAbstract liveDataDbHelper { get; set; }

        protected OrderManage orderManage { get; set; }
        protected override OperateResult ParticularInitlize()
        {
            orderManage = DependencyHelper.GetService<OrderManage>();
            liveDataDbHelper = DependencyHelper.GetService<LiveDataAbstract>();
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();

            return OperateResult.CreateSuccessResult();
        }

        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("PassedAssistant"))
                {
                    string content = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        PassedDeviceId = int.Parse(node.InnerText.Trim());
                    }
                    continue;
                }
                if (node.Name.Equals("FailedAssistant"))
                {
                    string content = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        FailedDeviceId = int.Parse(node.InnerText.Trim());
                    }
                    continue;
                }
            }
            result.IsSuccess = true;
            return result;
        }
        protected override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig;
                string path = "BusinessHandle/Config/Routes";
                XmlElement xmlElement = doc.GetXmlElement("Coordination", "Id", WorkerId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", WorkerId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                return InitalizeConfig(xmlNode);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        protected int PassedDeviceId { get; set; }

        protected int FailedDeviceId { get; set; }

        private OperateResult HandleNotifyPutawayReturn(DeviceBaseAbstract device, string barcode, CmdReturnMessageHeFei returnMsg, List<AssistantDevice> assistantDevices)
        {
            if (returnMsg.IsSuccess)//验证通过
            {
                return OperateResult.CreateSuccessResult();
            }
            ////验证不通过
            //DeviceBaseAbstract startDevice = device;
            AssistantDevice destDevice = assistantDevices.FirstOrDefault(d => d.Id.Equals(FailedDeviceId));
            if (destDevice == null)
            {
                string msg = string.Format("协助者中找不到编号为：{0} 的设备", FailedDeviceId);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            //Addr startAddr = startDevice.CurAddress;
            //Addr destAddr = destDevice.Device.CurAddress;
            //Order order = new Order();
            //order.OrderType = OrderTypeEnum.Out;
            //order.CurrAddr = startAddr;
            //order.DestAddr = destAddr;
            //order.Source = TaskSourceEnum.SELF;
            //order.PileNo = barcode;
            //order.IsReport = false;
            //OperateResult<Order> generateResult = orderManage.GenerateOrder(order);
            //if (generateResult.IsSuccess)
            //{
            //    string msg = string.Format("生成回退指令，回退信息：{0}", returnMsg.MESSAGE);
            //    generateResult.ErrorCode = 100;//暂时使用100错误代码，代表回退成功
            //    generateResult.Message = msg;
            //}
            //return generateResult;
            //(device as IdentityDeviceAbstract<List<string>>).DeviceControl.Communicate.Write
            RollerDeviceControl control = (destDevice.Device as TransportDeviceBaseAbstract).DeviceControl as RollerDeviceControl;
            if (control == null)
            {
                return OperateResult.CreateFailedResult("生成回退指令失败");
            }
            control.Communicate.Write(CL.WCS.DataModelPckg.DataBlockNameEnum.WriteDirectionDataBlock, FailedDeviceId);
            control.Communicate.Write(CL.WCS.DataModelPckg.DataBlockNameEnum.WriteOrderIDDataBlock, 1);
            OperateResult result= OperateResult.CreateSuccessResult();
            result.ErrorCode = 100;
            result.Message = returnMsg.MESSAGE;
            return result;

        }
        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> devices, string barcode, SizeProperties properties)
        {
            OperateResult<string> wmsAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(device.CurAddress.FullName);
            if (!wmsAddress.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("Wcs地址：{0} 转换为Wms地址失败，失败原因：\r\n{1}", device.CurAddress.FullName, wmsAddress.Message), 1);
            }
            //NotifyBarcodeReqNewInstructMode notifyBarcodeReqNewInstructMode = new NotifyBarcodeReqNewInstructMode
            //{
            //    CMD_TRIGGER = device.Name,
            //    CMD_NO = ""
            //};

            CmdNotifyBarcodeModel barCodeInstruct = new CmdNotifyBarcodeModel { Addr = wmsAddress.Content, Barcode = barcode };
            if (string.IsNullOrEmpty(barcode) || barcode.Trim().Equals("0"))
            {
                barCodeInstruct.Barcode = "ERROR";
            }

            string cmdPara = barCodeInstruct.ToJson();
            NotifyElement element = new NotifyElement(barCodeInstruct.Barcode, "NotifyBarcodeReqNewInstruct", "请求上架", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess || result.Content == null)
            {
                string msg = string.Format("调用上层接口失败，详情：\r\n {0}", result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            
            try
            {
                CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                OperateResult putawayResult = HandleNotifyPutawayReturn(device, barcode, serviceReturn, devices);
                if (!putawayResult.IsSuccess)
                {
                    result.Message = string.Format("处理上层返回结果的业务失败，详情：\r\n{0}", putawayResult.Message);
                    result.IsSuccess = false;
                    return result;
                }
                if (putawayResult.ErrorCode.Equals(100))
                {
                    OperateResult handleResult = new OperateResult();
                    handleResult.Message = putawayResult.Message;
                    handleResult.ErrorCode = putawayResult.ErrorCode;
                    return handleResult;
                }
                return OperateResult.CreateSuccessResult(string.Format("条码：{0} 上报成功", barcode));
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override bool IsNeedHandleBarcode(DeviceBaseAbstract device, string barcode)
        {
            return true;
        }


    }
}