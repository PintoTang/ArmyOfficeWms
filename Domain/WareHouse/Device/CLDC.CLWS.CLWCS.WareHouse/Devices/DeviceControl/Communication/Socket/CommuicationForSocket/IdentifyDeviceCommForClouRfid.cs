using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ClouRFID.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;
using CLDC.Infrastructrue.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.Framework.OPCClientAbsPckg;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{

    /// <summary>
    /// Socket通讯的识别设备
    /// </summary>
    public class IdentifyDeviceCommForClouRfid : IIdentifyDeviceCom<List<string>>
    {
        private bool _isConnected = true;

        public MessageReportDelegate MessageReportEvent { get; set; }

        public void MessageReport(string message,EnumLogLevel messageLevel)
        {
            if (MessageReportEvent != null)
            {
                MessageReportEvent(message, messageLevel);
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            private set { _isConnected = value; }
        }

        public DeviceName DeviceName { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }

        public event BarcodeCallback<List<string>> OnReceiveBarcode;

        private string remoteIpAddr { get; set; }
        private int remotePort { get; set; }

        public RFIDForClou RfidForClou { get; set; }
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            OperateResult opResult = InitConfig();
            if (opResult.IsSuccess)
            {
                string strRfidInfo = remoteIpAddr + ":" + remotePort.ToString();
                RfidForClou = new RFIDForClou(strRfidInfo, ConvertMode.Hex);
                RfidForClou.ReceiveBarcodeHandler += RfidForClou_ReceiveBarcodeHandler;
                this.OpcClient = DependencyHelper.GetService<OPCClientAbstract>();
            }
            else
            {
                opResult.IsSuccess = false;
                opResult.Message = "加载配置文件，初始化数据失败";
            }
            return opResult;
        }

        void RfidForClou_ReceiveBarcodeHandler(List<string> barcodeList, params object[] para)
        {
            if (OnReceiveBarcode != null) OnReceiveBarcode(this.DeviceName, barcodeList, para);
        }


        /// <summary>
        /// 异步读取
        /// </summary>
        /// <param name="para"></param>
        public async void SendCommandAsync(params object[] para)
        {
            await Task.Run(() => RfidForClou.GetBarcode());
        }
       public OperateResult<List<string>> SendCommand(params object[] para)
        {
            OperateResult<List<string>> opResult = new OperateResult<List<string>>();
            RfidForClou.GetBarcode();
            return opResult;
        }

        private OperateResult InitConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication/Config";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                OperateResult initializeResult = InitializeSocketConfig(xmlNode);
                if (!initializeResult.IsSuccess)
                {
                    return initializeResult;
                }
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private OperateResult InitializeSocketConfig(XmlNode xmlNode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            string nodeInText = "";
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                nodeInText = node.InnerText.Trim();

                if (node.Name.Equals("RemoteIp"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        remoteIpAddr = nodeInText.Trim();
                    }
                }
                else if (node.Name.Equals("RemotePort"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        remotePort = int.Parse(nodeInText.Trim());
                    }
                }
            }
            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// OPC的客户端
        /// </summary>
        private OPCClientAbstract OpcClient { get; set; }

    }
}
