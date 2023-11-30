using System;
using System.IO;
using System.Net;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Windows.Controls;
using System.Xml;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Webservice.Model;
using CLDC.Infrastructrue.Xml;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
    public class WebApiServiceHost : ServiceAbstract
    {

        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public int ApiPort { get; set; }

        public string ApiIp
        {
            get { return _apiIp; }
            set { _apiIp = value; }
        }

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

                WebApiClientProperty webApiConfig = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        webApiConfig =
                            (WebApiClientProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebApiClientProperty));
                    }
                    catch (Exception ex)
                    {
                        initilizeConfigResult.IsSuccess = false;
                        initilizeConfigResult.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (webApiConfig == null)
                {
                    return initilizeConfigResult;
                }

                ApiPort = webApiConfig.ApiPort;
                ApiIp = webApiConfig.ApiIp;
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

        private HttpSelfHostServer _server;
        private string _apiIp = "127.0.0.1";

        protected override OperateResult StartService()
        {
            string http = string.Format(@"http://{0}:{1}", ApiIp, ApiPort);
            OperateResult result = new OperateResult();
            try
            {
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(http);

                config.Routes.MapHttpRoute("API", "api/{controller}/{action}/{cmd}", new { cmd = RouteParameter.Optional });
                _server = new HttpSelfHostServer(config);
                _server.OpenAsync().Wait();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult StopService()
        {
            OperateResult result = OperateResult.CreateSuccessResult();
            try
            {
                if (_server != null)
                {
                    _server.CloseAsync().Wait();
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

        public override System.Windows.Window GetAssistantView()
        {
            throw new NotImplementedException();
        }

        public override UserControl GetDetailView()
        {
            return new UserControl();
        }
    }
}
