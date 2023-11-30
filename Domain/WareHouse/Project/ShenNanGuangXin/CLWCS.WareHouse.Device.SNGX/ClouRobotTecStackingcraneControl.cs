using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.ThreadHandle;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Stackingcrane;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.stackingcrane.OpcStackingcrane;
using CLDC.Infrastructrue.Xml;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;

namespace CLWCS.WareHouse.Device.HeFei
{
    /// <summary>
    /// HeFei堆垛机控制
    /// </summary>
    public sealed class ClouRobotTecStackingcraneControl : OrderDeviceControlAbstract
    {

        StackingcraneProtocolAbstract protocolAnalysis;


        /// <summary>
        /// 处理异常代码的业务逻辑
        /// </summary>
        /// <param name="newValue"></param>
        private void HandleFaultCodeChange(int newValue)
        {
            bool isNeedHandle = IsNeedHandleFaultCode(newValue);
            if (!isNeedHandle)
            {
                return;
            }

 
            if (NotifyDeviceExceptionEvent == null) return;

            //解注册堆垛机的完成信号

            TaskExcuteStatusType exceptionType = protocolAnalysis.FaultCodeChangeToExceptionType(newValue);
            NotifyDeviceExceptionEvent(exceptionType);
        }
        
        private bool IsNeedHandleFaultCode(int newValue)
        {
            if (newValue < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
       

        internal Action<ConnectState> NotifyConnectStatusChange;

        private readonly ManualResetEvent _enableEvent = new ManualResetEvent(true);

        private ThreadHandleProcess _heartBeat;
        private void DeviceHeartBeat()
        {
            while (_heartBeat.IsContinuous)
            {
                _enableEvent.WaitOne();
                bool change = ChangeHeartBeat();
                if (change)
                {
                    Thread.Sleep(1 * 1000);
                    if (NotifyConnectStatusChange != null)
                    {
                        bool isChange = IsHeartBeatChange();
                        NotifyConnectStatusChange(isChange ? ConnectState.Online : ConnectState.Offline);
                    }
                }
                else
                {
                    if (NotifyConnectStatusChange != null)
                    {
                        NotifyConnectStatusChange(ConnectState.Offline);
                    }
                }
                Thread.Sleep(500);
            }
        }

        public IPlcDeviceCom Communicate { get; set; }

        private bool IsHeartBeatChange()
        {
            OperateResult<int> read = Communicate.ReadInt(DataBlockNameEnum.DeviceConnectStatus);
            if (read.IsSuccess)
            {
                return read.Content == 1;
            }
            else
            {
                return false;
            }
        }

        private bool ChangeHeartBeat()
        {
            OperateResult write = Communicate.Write(DataBlockNameEnum.DeviceConnectStatus, 2);
            if (write.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Package GetCurrentPackage()
        {
            Package currentPackage = new Package();
            OperateResult<int> readOrderResult = Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            if (readOrderResult.IsSuccess)
            {
                currentPackage.PackageId = readOrderResult.Content;
            }
            return currentPackage;
        }

        public override OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            if (!destState.Equals(UseStateMode.Disable))
            {
                return OperateResult.CreateSuccessResult();
            }

            if (IsLoad())
            {
                return OperateResult.CreateFailedResult("设备处于搬运货物状态，不可禁用", 1);
            }
            else
            {
                return OperateResult.CreateSuccessResult();
            }
        }

        public override bool IsLoad()
        {
            OperateResult<int> loaded = Communicate.ReadInt(DataBlockNameEnum.InProgressOrder);
            if (loaded.IsSuccess)
            {
                if (loaded.Content > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
        }

        public override OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum, int startValue, int endValue, MonitorSpecifiedOpcValueCallback callbackAction)
        {
            return Communicate.RegisterFromStartToEndStatus(dbBlockEnum, startValue, endValue, callbackAction);
        }




        public override OperateResult SetAbleState(UseStateMode destState)
        {
            if (destState.Equals(UseStateMode.Disable))
            {
                _enableEvent.Reset();
            }
            else
            {
                _enableEvent.Set();
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult InitProtocolAnalysis()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/ProtocolTranslation";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);


                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    result.IsSuccess = false;
                    return result;
                }

                string communicationConfigXml = xmlNode.OuterXml;
                StackingcraneProtocolTranslationProperty protocolTranslationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        protocolTranslationProperty =
                            (StackingcraneProtocolTranslationProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(StackingcraneProtocolTranslationProperty));
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (protocolTranslationProperty == null)
                {
                    return result;
                }


                protocolAnalysis = (StackingcraneProtocolAbstract)Assembly.Load(protocolTranslationProperty.NameSpace)
                    .CreateInstance(protocolTranslationProperty.NameSpace + "." + protocolTranslationProperty.ClassName);

                if (protocolAnalysis == null)
                {
                    return
                        OperateResult.CreateFailedResult(
                            string.Format("协议分析类反射失败,类名：{0} 命名空间：{1}", protocolTranslationProperty.ClassName, protocolTranslationProperty.NameSpace), 1);
                }

                OperateResult protocolResult = protocolAnalysis.Initialize(DeviceId, DeviceName);
                if (!protocolResult.IsSuccess)
                {
                    return protocolResult;
                }
                result = OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        private OperateResult InitCommunicate()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    result.IsSuccess = false;
                    return result;
                }

                string communicationConfigXml = xmlNode.OuterXml;
                OpcCommunicationProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (OpcCommunicationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(OpcCommunicationProperty));
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

                Communicate = (IPlcDeviceCom)Assembly.Load(communicationProperty.NameSpace)
                                .CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                if (Communicate == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("协议通讯类反射失败,类名：{0} 命名空间：{1}", communicationProperty.ClassName, communicationProperty.NameSpace), 1);
                }
                OperateResult cmmInitResult = Communicate.Initialize(DeviceId, DeviceName);
                if (!cmmInitResult.IsSuccess)
                {
                    return cmmInitResult;
                }
                Communicate.Name = communicationProperty.Name;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }
        public override OperateResult ParticularInitConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult initProtocolAnalysis = InitProtocolAnalysis();
                if (!initProtocolAnalysis.IsSuccess)
                {
                    return initProtocolAnalysis;
                }

                OperateResult initCommunicate = InitCommunicate();
                if (!initCommunicate.IsSuccess)
                {
                    return initCommunicate;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }

            return result;

        }

        private Dictionary<DataBlockNameEnum, int> ComposeCmd(StackingcraneCmdElement cmdElememnt)
        {
            Dictionary<DataBlockNameEnum, int> cmdDic = new Dictionary<DataBlockNameEnum, int>();
            cmdDic.Add(DataBlockNameEnum.CommadType, (int)cmdElememnt.CmdType);
            cmdDic.Add(DataBlockNameEnum.SourceStationNum, cmdElememnt.SourceStationNum);
            cmdDic.Add(DataBlockNameEnum.DestStationNum, cmdElememnt.DestStationNum);

            cmdDic.Add(DataBlockNameEnum.SourceStationColumn, cmdElememnt.SourceStationColumn);
            cmdDic.Add(DataBlockNameEnum.SourceStationLayer, cmdElememnt.SourceStationLayer);
            cmdDic.Add(DataBlockNameEnum.SourceStationRow, cmdElememnt.SourceStationRow);

            cmdDic.Add(DataBlockNameEnum.SourceCellColumn, cmdElememnt.SourceCellColumn);
            cmdDic.Add(DataBlockNameEnum.SourceCellLayer, cmdElememnt.SourceCellLayer);
            cmdDic.Add(DataBlockNameEnum.SourceCellRow, cmdElememnt.SourceCellRow);

            //cmdDic.Add(DataBlockNameEnum.DestCellColumn, cmdElememnt.DestCellColumn);
            //cmdDic.Add(DataBlockNameEnum.DestCellLayer, cmdElememnt.DestCellLayer);
            //cmdDic.Add(DataBlockNameEnum.DestCellRow, cmdElememnt.DestCellRow);
            //cmdDic.Add(DataBlockNameEnum.DestStationColumn, cmdElememnt.DestStationColumn);
            //cmdDic.Add(DataBlockNameEnum.DestStationLayer, cmdElememnt.DestStationLayer);
            //cmdDic.Add(DataBlockNameEnum.DestStationRow, cmdElememnt.DestStationRow);

            cmdDic.Add(DataBlockNameEnum.VerificationCode, cmdElememnt.VerificationCode);
            cmdDic.Add(DataBlockNameEnum.OPCOrderIdDataBlock, cmdElememnt.CmdNum);
            cmdDic.Add(DataBlockNameEnum.SendCmdCode, cmdElememnt.SendCmdCode);
            cmdDic.Add(DataBlockNameEnum.TrayType, cmdElememnt.TrayType);
    
            return cmdDic;
        }

        public override OperateResult DoJob(TransportMessage transMsg)
        {
            //1.通过输入TransportMessage，经过协议解析类返回StatckingcraneCmdElement数据
            //2.得到StatckingcraneCmdElement数据后转换为设备协议报文
            //3.给设备下发协议报文

            OperateResult notifyResult = new OperateResult();

            try
            {
                OperateResult<StackingcraneCmdElement> result = protocolAnalysis.TransportChangeToCmdElement(transMsg);

                if (!result.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("根据指令：{0} \r\n协议解析出错，出错原因：\r\n{1}", transMsg.TransportOrder, result.Message), 1);
                }

                Dictionary<DataBlockNameEnum, int> stackingcraneCmdDic = ComposeCmd(result.Content);

                OperateResult writeResult = Communicate.Write(stackingcraneCmdDic);
                if (!writeResult.IsSuccess)
                {
                    return writeResult;
                }
                notifyResult.IsSuccess = true;
                return notifyResult;
            }
            catch (Exception ex)
            {
                notifyResult.IsSuccess = false;
                notifyResult.Message = OperateResult.ConvertException(ex);


            }
            return notifyResult;
        }

        public override OperateResult<int> GetFinishedOrder()
        {
            OperateResult<int> finishedOrder = Communicate.ReadInt(DataBlockNameEnum.DeviceFinishedOrder);
            return finishedOrder;
        }

        /// <summary>
        /// 读取堆垛机设备状态 
        /// </summary>
        /// <returns></returns>
        public OperateResult<int> GetDeviceWorkStatus()
        {
            OperateResult<int> deviceStatus = Communicate.ReadInt(DataBlockNameEnum.DeviceWorkStatus);
            return deviceStatus;
        }

        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        private const string HeartBeatThreadName = "_心跳";
        public override OperateResult Start()
        {
            _heartBeat = ThreadHandleManage.CreateNewThreadHandle(this.Name + HeartBeatThreadName, DeviceHeartBeat);
            OperateResult result = _heartBeat.Start();
            RegisterValueChange(DataBlockNameEnum.DeviceFaultCode, HandleFaultCodeChange);
            return result;
        }

        public override UserControl GetPropertyView()
        {
            return null;
        }

        public override OperateResult ClearFaultCode()
        {
            return Communicate.Write(DataBlockNameEnum.DeviceFaultCode, 0);
        }
    }
}
