using System;
using System.IO;
using System.Reflection;
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
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 普罗格四向车业务控制
    /// </summary>
    public class FourWayVehicleRcsControl : OrderDeviceControlAbstract
    {
        /// <summary>
        /// http webApi链接  
        /// </summary>
        public string Http { get; set; }

        /// <summary>
        /// 通讯接口
        /// </summary>
        public IWebNetInvoke WebNetInvoke;
        /// <summary>
        /// 初始化配置加载
        /// </summary>
        /// <returns></returns>
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
                        if (communicationProperty==null)
                        {
                            return  OperateResult.CreateFailedResult(string.Format("获取：{0} 通讯初始化参数失败",this.Name));
                        }
                        WebNetInvoke =(IWebNetInvoke)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

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
        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 获取WebApiView
        /// </summary>
        /// <returns></returns>
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
       
        /// <summary>
        /// 搬运任务业务处理
        /// </summary>
        /// <param name="transMsg">搬运信息</param>
        /// <returns>OperateResult</returns>
        public override OperateResult DoJob(TransportMessage transMsg)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                SendTaskCmd cmd = new SendTaskCmd()
                {
                    END_ADDR = transMsg.DestAddr.FullName,
                    PACKAGE_BARCODE = transMsg.PileNo,
                    PRI = transMsg.TransportOrder.OrderPriority,
                    START_ADDR = transMsg.StartAddr.FullName,
                    TASK_NO = transMsg.TransportOrder.OrderId.ToString(),
                    TASK_TYPE = GetRcsTaskTypeForUpperTaskType(transMsg.TransportOrder.SourceTaskType.GetValueOrDefault()),
                    EXT1 = "",
                    EXT2 = ""
                };

                //对起始地址 和目标地址进行转换 wms-转换成-RCS坐标
                if (cmd.TASK_TYPE == 2)
                {
                    //入库
                    if (cmd.START_ADDR.Contains("DetectionPort:1_1_1"))
                    {
                        cmd.START_ADDR = "0100450017";//1003 对应RCS 1004
                    }
                    else if (cmd.START_ADDR.Contains("DetectionPort:2_1_1"))
                    {
                        cmd.START_ADDR = "0100450022";//1015  对应RCS 2004
                    }
                    cmd.END_ADDR =
                      PuLuoGeAgvCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(cmd.END_ADDR);
                }
                else if (cmd.TASK_TYPE == 3)
                {
                    //出库
                    cmd.START_ADDR =
                        PuLuoGeAgvCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(cmd.START_ADDR);
                    if (cmd.END_ADDR.Contains("DetectionPort:1_1_1"))
                    {
                        cmd.END_ADDR = "0100450017";//1005 对应RCS 1004
                    }
                    else if (cmd.END_ADDR.Contains("DetectionPort:2_1_1"))
                    {
                        cmd.END_ADDR = "0100450022";//1014 对应RCS 2004
                    }
                }
                else if (cmd.TASK_TYPE == 4)
                {
                    //移库
                    cmd.START_ADDR =
                       PuLuoGeAgvCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(cmd.START_ADDR);
                    cmd.END_ADDR =
                   PuLuoGeAgvCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(cmd.END_ADDR);
                }
                else
                {
                    //其它类型 不做处理
                    result.Message = string.Format("任务类型不为2、3、4! 原搬运任务类型为：{0} 实际参数为：{1}",
                        transMsg.TransportOrder.SourceTaskType.ToString(), cmd.TASK_TYPE);
                    result.IsSuccess = false;
                    return result;
                }

                if (string.IsNullOrEmpty(cmd.START_ADDR) || string.IsNullOrEmpty(cmd.END_ADDR))
                {
                    result.Message = "起始地址或目标地址不能为空!";
                    result.IsSuccess = false;
                    return result;
                }
                return SendTask(cmd);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult<int> GetFinishedOrder()
        {
            int order = 0;
            return OperateResult.CreateFailedResult(order, "无数据");
        }

        /// <summary>
        /// 发送指令 
        /// </summary>
        /// <param name="webApiCmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult SendCmd(WebApiInvokeCmd webApiCmd)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<string> invokeResult = WebNetInvoke.ServiceRequest<SyncResReMsg>(Http, webApiCmd.MethodName, webApiCmd.InvokeCmd);
                if (!invokeResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = invokeResult.Message;
                    return result;
                }
                SyncResReMsg responseResult = (SyncResReMsg)invokeResult.Content;
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
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        #region 调用RCS接口方法
        /// <summary>
        /// 下发任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult SendTask(SendTaskCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.SendTask.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// 下发换层任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult SendChangeTask(SendChangeTaskCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.SendChangeTask.ToString(),
              cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// 任务查询
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult SelectTaskInfo(SelectTaskInfoCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.SelectTaskInfo.ToString(),
              cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult DeleteTask(DeleteTaskCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.DeleteTask.ToString(),
              cmd.ToJson());
            return SendCmd(apiCmd);
        }

        /// <summary>
        /// 请求货物同行
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult RequestRCSPermit(RequestRCSPermitCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.RequestRCSPermit.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// 通知RCS货物通行完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult ReportRCSPermitFinish(ReportRCSPermitFinishCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.ReportRCSPermitFinish.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }

        /// <summary>
        /// 模式切换
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult SendModeSwitch(SendModeSwitchCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.SendModeSwitch.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        #endregion

        /// <summary>
        /// 转换成RCS协议类型 2入库、3出库、4移库
        /// </summary>
        /// <param name="sourceTaskType">任务类型</param>
        /// <returns></returns>
        private int GetRcsTaskTypeForUpperTaskType(SourceTaskEnum sourceTaskType)
        {
            int rtValue = 0;
            switch (sourceTaskType)
            {
                case SourceTaskEnum.In:
                case SourceTaskEnum.InventoryIn:
                case SourceTaskEnum.PickIn:
                case SourceTaskEnum.HandUpLoad:
                    rtValue = 2;
                    break;
                case SourceTaskEnum.Out:
                case SourceTaskEnum.InventoryOut:
                case SourceTaskEnum.PickOut:
                case SourceTaskEnum.HandDownLoad:
                    rtValue = 3;
                    break;
                case SourceTaskEnum.Move:
                    rtValue = 4;
                    break;
                default:
                    break;
            }
            return rtValue;
        }


    }
}
