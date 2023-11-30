using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client
{
    public sealed class SocketAsyncClient : SocketAsyncClientAbstract
    {
        public override OperateResult InitilizeConfig()
        {
            OperateResult initilizeConfigResult = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication/Config";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);

                if (xmlNode == null)
                {
                    initilizeConfigResult.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    initilizeConfigResult.IsSuccess = false;
                    return initilizeConfigResult;
                }

                string businessConfigXml = xmlNode.OuterXml;

                SocketClientProperty socketProperty = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        socketProperty =
                            (SocketClientProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(SocketClientProperty));
                    }
                    catch (Exception ex)
                    {
                        initilizeConfigResult.IsSuccess = false;
                        initilizeConfigResult.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (socketProperty == null)
                {
                    return initilizeConfigResult;
                }

                LocalPort = socketProperty.LocalPort;
                RemotePort = socketProperty.RemotePort;
                RemoteIp = socketProperty.RemoteIp;
                LocalIp = socketProperty.LocalIp;

                ProtocolType = socketProperty.ProtocolType;

                LocalIpAndPort = new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort);
                RemoteIpAndPort = new IPEndPoint(IPAddress.Parse(RemoteIp), RemotePort);

                initilizeConfigResult.IsSuccess = true;
                return initilizeConfigResult;

            }
            catch (Exception ex)
            {
                initilizeConfigResult.IsSuccess = false;
                initilizeConfigResult.Message = OperateResult.ConvertException(ex);
            }
            return initilizeConfigResult;
        }
    }
}
