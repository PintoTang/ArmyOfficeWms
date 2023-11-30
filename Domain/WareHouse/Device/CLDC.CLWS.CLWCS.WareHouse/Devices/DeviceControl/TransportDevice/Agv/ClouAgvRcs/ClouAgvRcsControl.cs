using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClouAgvRcsControl : OrderDeviceControlAbstract
    {
        public string Http { get; set; }

        public IWebNetInvoke WebNetInvoke;
        public override OperateResult ParticularInitConfig()
        {
            //获取Http 的值
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                string communicationConfigXml = xmlNode.OuterXml;
                WebClientCommunicationProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (WebClientCommunicationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebClientCommunicationProperty));
                        if (communicationProperty == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("获取：{0} 通讯初始化参数失败", this.Name));
                        }
                        WebNetInvoke = (IWebNetInvoke)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                        if (WebNetInvoke == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("初始化：{0} 通讯接口：{1} 失败，命名空间：{2} 类名：{3}", this.Name, "IWebNetInvoke", communicationProperty.NameSpace, communicationProperty.ClassName));
                        }

                        WebNetInvoke.LogDisplayName = communicationProperty.Name;
                        WebNetInvoke.CommunicationMode = communicationProperty.CommunicationMode;
                        WebNetInvoke.TimeOut = communicationProperty.Config.TimeOut;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (communicationProperty == null)
                {
                    return result;
                }

                Http = communicationProperty.Config.Http.Trim();

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
                result = OperateResult.CreateFailedResult();
            }
            return result;
        }

        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetPropertyView()
        {
            WebApiView controlView = new WebApiView { Height = 250 };
            controlView.HttpUrl = this.Http;
            return controlView;
        }

        public override bool IsLoad()
        {
            return false;
        }

        public override OperateResult ClearFaultCode()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override Package GetCurrentPackage()
        {
            return null;
        }


        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum, int startValue, int endValue,
            MonitorSpecifiedOpcValueCallback callbackAction)
        {
            throw new NotImplementedException();
        }

        private int ConvertOrderType(SourceTaskEnum upperTaskType)
        {
            if (upperTaskType== SourceTaskEnum.In)
            {
                return 10;
            }
            else if (upperTaskType == SourceTaskEnum.InventoryIn)
            {
                return 30;
            }
            else if (upperTaskType == SourceTaskEnum.InventoryOut)
            {
                return 30;
            }
            else
            {
                return 20;
            }
        }

        private int ConvertOrderPriority(int sourcePriority)
        {
            if (sourcePriority<10)
            {
                return 0;
            }
            else if(sourcePriority>=10&&sourcePriority<100)
            {
                return 10;
            }
            else
            {
                return 100;
            }
        }
        /// <summary>
        /// 仓位信息转换 RCS
        /// </summary>
        /// <param name="curCell">Cell:1_1_1_0_A01、Cell:1_1_1_1_A01</param>
        /// <returns>Cell:1_1_1_A01、Cell:6_1_1_A01</returns>
        private string HandCellConverNewCell(string curCell)
        {
            //Cell:3_1_22_A01

            //Cell:1_1_1_0_A01  Cell:1_1_1_1_A01
            //Cell:2_1_1_0_A01  Cell:2_1_1_1_A01
            //Cell:3_1_1_0_A01  Cell:3_1_1_1_A01
           //转换成 Cell:1_1_1_A01、Cell:2_1_1_A01、Cell:3_1_1_A01、Cell:4_1_1_A01、Cell:5_1_1_A01、Cell:6_1_1_A01
            string[] cellInfoArr = curCell.Split(':')[1].Split('_');
            if (cellInfoArr.Length != 5)
            {
                return curCell;
            }
            
            int rankNum = int.Parse(cellInfoArr[0]);
            int layNum = int.Parse(cellInfoArr[1]);
            int colNum = int.Parse(cellInfoArr[2]);
            int lightOrDarkNum = int.Parse(cellInfoArr[3]);
            string cell = "";
            if (lightOrDarkNum == 0)
            {
                //浅位
                if (rankNum == 1)
                {
                    cell = string.Format("Cell:{0}_{1}_{2}_A01", rankNum, layNum, colNum);
                }
                else if (rankNum == 2)
                {
                    cell = string.Format("Cell:{0}_{1}_{2}_A01", rankNum + 2, layNum, colNum);
                }
                else if (rankNum == 3)
                {
                    cell = string.Format("Cell:{0}_{1}_{2}_A01", rankNum + 2, layNum, colNum);
                }
            }
            else if (lightOrDarkNum == 1)
            {
                //深位
                if (rankNum == 1)
                {
                    cell = string.Format("Cell:{0}_{1}_{2}_A01", rankNum + 1, layNum, colNum);
                }
                else if (rankNum == 2)
                {
                    cell = string.Format("Cell:{0}_{1}_{2}_A01", rankNum + 1, layNum, colNum);
                }
                else if (rankNum == 3)
                {
                    cell = string.Format("Cell:{0}_{1}_{2}_A01", rankNum + 3, layNum, colNum);
                }
            }
            return cell;

        }


        public override OperateResult DoJob(TransportMessage transMsg)
        {
            OperateResult doJobResult = OperateResult.CreateFailedResult();
            try
            {
                List<OrderAddCmd> orderAddCmdList = new List<OrderAddCmd>();
                OrderAddCmd orderAddCmd = new OrderAddCmd();
                OrderDetail orderDetail = new OrderDetail();
                orderAddCmd.orderCode = transMsg.TransportOrderId.ToString();
                orderAddCmd.orderSource = "ClouWCS";
                orderAddCmd.planBeginTime = DateTime.Now;
                orderAddCmd.planEndTime = DateTime.Now;

                orderAddCmd.priority = ConvertOrderPriority(transMsg.TransportOrder.OrderPriority);

                orderDetail.beginStationCode = transMsg.StartAddr.FullName;
                orderDetail.endStationCode = transMsg.DestAddr.FullName;

                //根据协议进行数据转换  双深位转换成RCS想要的格式
                if (orderDetail.beginStationCode.Contains("Cell"))
                {
                    orderDetail.beginStationCode = HandCellConverNewCell(transMsg.StartAddr.FullName);
                }
                if (orderDetail.endStationCode.Contains("Cell"))
                {
                    orderDetail.endStationCode = HandCellConverNewCell(transMsg.DestAddr.FullName);
                }

                //处理  起始地址
                if (transMsg.StartAddr.FullName.Contains("InAndOutPort:2_1_1"))
                {
                    orderDetail.beginStationCode = "Cell:3_1_22_A01";
                }
                else if (transMsg.StartAddr.FullName.Contains("CheckOutPort:1_1_1"))
                {
                    orderDetail.beginStationCode = "Cell:3_2_22_A01";
                }
                else if (transMsg.StartAddr.FullName.Contains("CheckInPort:1_1_1"))
                {
                    orderDetail.beginStationCode = "Cell:3_3_22_A01";
                }

                //处理  目标地址
                if (transMsg.DestAddr.FullName.Contains("InAndOutPort:2_1_1"))
                {
                    orderDetail.endStationCode = "Cell:3_1_22_A01";
                }
                else if (transMsg.DestAddr.FullName.Contains("CheckOutPort:1_1_1"))
                {
                    orderDetail.endStationCode = "Cell:3_2_22_A01";
                }
                else if (transMsg.DestAddr.FullName.Contains("CheckInPort:1_1_1"))
                {
                    orderDetail.endStationCode = "Cell:3_3_22_A01";
                }


                orderDetail.actionExtra = string.Empty;
                orderDetail.orderType = ConvertOrderType(transMsg.TransportOrder.SourceTaskType.GetValueOrDefault());
                orderDetail.productCode = transMsg.TransportOrder.PileNo;
                orderDetail.qty = transMsg.TransportOrder.Qty;
                orderDetail.productName = "电能表";

                orderAddCmd.details = new List<OrderDetail>() { orderDetail };
                orderAddCmdList.Add(orderAddCmd);
                string orderAddCmdJson = orderAddCmdList.ToJson();
                WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, ClouAgvRcsApiEnum.OrderAdd.ToString(), orderAddCmdJson);
                return SendCmd(apiCmd);

            }
            catch (Exception ex)
            {
                doJobResult.IsSuccess = false;
                doJobResult.Message = OperateResult.ConvertExMessage(ex);
            }
            return doJobResult;
        }

        public override OperateResult<int> GetFinishedOrder()
        {
            int order = 0;
            return OperateResult.CreateFailedResult(order, "无数据");
        }

        private OperateResult SendCmd(WebApiInvokeCmd webApiCmd)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<string> invokeResult = WebNetInvoke.ServiceRequest<ClouRcsResSucMsg>(Http, webApiCmd.MethodName, webApiCmd.InvokeCmd);
                if (!invokeResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = invokeResult.Message;
                    return result;
                }
                ClouRcsResSucMsg responseResult = (ClouRcsResSucMsg)invokeResult.Content;
                if (!responseResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = invokeResult.Content;
                    return result;
                }
                result.IsSuccess = true;
                result.Message = invokeResult.Content;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

    }
}
