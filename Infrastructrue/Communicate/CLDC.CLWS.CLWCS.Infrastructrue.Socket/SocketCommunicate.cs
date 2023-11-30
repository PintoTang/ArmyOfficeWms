using System;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets
{
    /// <summary>
    /// Socket通讯协议接口实现
    /// </summary>
    public class SocketCommunicate : ComBase
    {
        /// <summary>
        /// tcp对象(客户端、服务端)
        /// </summary>
        public TcpCom tcpComm { get; set; }

        public SocketCommunicate()
        {
            this.tcpComm = new TcpCom();
        }
        public int DeviceId { get; set; }

        public DeviceName DeviceName { get; set; }
        /// <summary>
        /// SocketType 通讯类型(客户端、服务端)
        /// </summary>
        public string SocketType { get; set; }
        public OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            OperateResult initConfig = InitConfig();
            if (!initConfig.IsSuccess)
            {
                return initConfig;
            }
            CreateConn();

            return OperateResult.CreateSuccessResult();
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
                if (xmlNode.Name.Equals("Config"))//得到socket类型 
                {
                    SocketType = xmlNode.Attributes["Type"].Value.Trim();
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
                //result = OperateResult.CreateFailedResult();//此行代码 待删除 屏蔽了具体错误
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
                if (node.Name.Equals("IsServer"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        tcpComm.IsSever = (bool)(nodeInText.ToUpper() == "TRUE");
                    }
                }
                else if (node.Name.Equals("LocalIp"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        tcpComm.LocalIp = System.Net.IPAddress.Parse(nodeInText);
                    }
                }
                else if (node.Name.Equals("LocalPort"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        tcpComm.LocalIpPort = int.Parse(nodeInText);
                    }
                }
                else if (node.Name.Equals("RemoteIp"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        tcpComm.RemoteIp = System.Net.IPAddress.Parse(nodeInText);
                    }
                }
                else if (node.Name.Equals("RemotePort"))
                {
                    if (!string.IsNullOrEmpty(nodeInText))
                    {
                        tcpComm.RemoteIpPort = int.Parse(nodeInText);
                    }
                }
            }
            result.IsSuccess = true;
            return result;
        }
       
       /// <summary>
        /// 创建连接
       /// </summary>
        public void CreateConn()
        {
            if (SocketType == "Socket")
            {
                this.tcpComm.Connect();
            }
            else
            {
                //配置有问题 SocketType 
            }
        }


        public override int Send(byte[] bysts)
        {
           return this.tcpComm.Send(bysts);
        }

        public override bool Close()
        {
          return  this.tcpComm.Close();
        }

        public override int Receive(byte[] bytes)
        {
           return this.tcpComm.Receive(bytes);
        }
    }
}
