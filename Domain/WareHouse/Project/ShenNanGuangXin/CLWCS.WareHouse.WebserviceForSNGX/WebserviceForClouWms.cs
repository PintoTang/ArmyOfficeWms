using System;
using System.IO;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Webservice.Model;
using CLDC.Infrastructrue.Xml;

namespace CLWCS.WareHouse.WebserviceForHeFei
{
    /// <summary>
    /// WCS提供HeFei项目科陆Wms接口
    /// </summary>
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    [ServiceBehavior(Namespace = "http://www.szclou.com", InstanceContextMode = InstanceContextMode.Single)]
    public class WebserviceForClouWms : ServiceAbstract
    {
        public string WebserviceUrl { get; set; }
        private WcfServerHost<WebserviceForHeFei> WebserviceHost { get; set; }

        public string WebserviceType = "IWebserviceForHeFei";

        protected override OperateResult InitilizeConfig()
        {
            OperateResult initilizeConfigResult = OperateResult.CreateFailedResult();
            try
            {
                string fileName = "Config/WcsServiceConfig.xml";
                string path = "Config";
                var config = new XmlOperator(fileName);
                XmlElement xmlElement = config.GetXmlElement("WcsService", "Id", WebserviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);

                if (xmlNode == null)
                {
                    initilizeConfigResult.Message = string.Format("通过设备：{0} 获取配置失败", WebserviceId);
                    initilizeConfigResult.IsSuccess = false;
                    return initilizeConfigResult;
                }

                string businessConfigXml = xmlNode.OuterXml;

                WebserviceConfigProperty webserviceConfig = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        webserviceConfig =
                            (WebserviceConfigProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebserviceConfigProperty));
                    }
                    catch (Exception ex)
                    {
                        initilizeConfigResult.IsSuccess = false;
                        initilizeConfigResult.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (webserviceConfig == null)
                {
                    return initilizeConfigResult;
                }

                WebserviceUrl = webserviceConfig.WebserviceUrl;
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

        protected override OperateResult StartService()
        {
            return WebserviceHost.StartService(WebserviceUrl, WebserviceType);
        }

        public override OperateResult StopService()
        {
            return WebserviceHost.StopService();
        }

        public override Window GetAssistantView()
        {
            return new Window();
        }

        public override UserControl GetDetailView()
        {
            ServiceApiView apiServiceView = new ServiceApiView();
            apiServiceView.DataContext = new ServiceApiViewModel(Name);
            return apiServiceView;
        }

        protected override OperateResult ParticularInitlize()
        {
            WebserviceHost = new WcfServerHost<WebserviceForHeFei>();
            return OperateResult.CreateSuccessResult();
        }
    }
}
