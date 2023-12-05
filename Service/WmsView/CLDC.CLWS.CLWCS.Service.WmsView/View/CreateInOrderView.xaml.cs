using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.UserCtrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ClouRFID.Model;
using Infrastructrue.Ioc.DependencyFactory;
using System.Xml;
using CLDC.Infrastructrue.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;
using CL.Framework.CmdDataModelPckg;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateInOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateInOrderView : UserControl
    {
        public RFIDForClou HopelandRfid { get; set; }

        private string remoteIpAddr { get; set; }
        private int remotePort { get; set; }
        public int DeviceId { get; set; }
        public event BarcodeCallback<List<string>> OnReceiveBarcode;
        public DeviceName DeviceName { get; set; }


        public CreateInOrderView()
        {
            InitializeComponent();
            Initialize();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnScanRfid_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("test");
        }

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public OperateResult Initialize()
        {
            OperateResult opResult = new OperateResult();//InitConfig();
            opResult.IsSuccess = true;
            if (opResult.IsSuccess)
            {
                string strRfidInfo = "192.168.1.116" + ":" + "9090".ToString();
                HopelandRfid = new RFIDForClou(strRfidInfo, ConvertMode.Hex);
                HopelandRfid.ReceiveBarcodeHandler += HopelandRfid_ReceiveBarcodeHandler;
            }
            else
            {
                opResult.IsSuccess = false;
                opResult.Message = "加载配置文件，初始化数据失败";
            }
            return opResult;
        }

        void HopelandRfid_ReceiveBarcodeHandler(List<string> barcodeList, params object[] para)
        {
            if (OnReceiveBarcode != null) OnReceiveBarcode(this.DeviceName, barcodeList, para);
        }

        /// <summary>
        /// 异步读取
        /// </summary>
        /// <param name="para"></param>
        public async void SendCommandAsync(params object[] para)
        {
            await Task.Run(() => HopelandRfid.GetBarcode());
        }

        public OperateResult<List<string>> SendCommand(params object[] para)
        {
            OperateResult<List<string>> opResult = new OperateResult<List<string>>();
            HopelandRfid.GetBarcode();
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


    }
}
