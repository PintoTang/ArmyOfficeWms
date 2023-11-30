using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Webservice.Model
{
    public class WcfServerHost<T> : WcfServer<T>
    {
        
        protected override OperateResult ParticularInitConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult StartService(string uri, string type)
        {
            OperateResult startResult = OperateResult.CreateFailedResult();
            try
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                httpBinding.MaxReceivedMessageSize = 2147483647;
                httpBinding.MaxBufferPoolSize = 2147483647;
                httpBinding.MaxBufferSize = 2147483647;
                Type contractType = typeof (T).GetInterface(type);
                this.AddServiceEndpoint(contractType, httpBinding, uri);
                if (this.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
                {
                    ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
                    behavior.HttpGetEnabled = true;
                    behavior.HttpGetUrl = new Uri(uri);
                    this.Description.Behaviors.Add(behavior);
                    this.Open();
                    startResult.IsSuccess = true;
                    return startResult;
                }
            }
            catch (Exception ex)
            {
                startResult.IsSuccess = false;
                startResult.Message = OperateResult.ConvertException(ex);
            }
            return startResult;

        }

        public override OperateResult StopService()
        {
            OperateResult stopResult=OperateResult.CreateFailedResult();
            try
            {
                this.Abort();
                stopResult.IsSuccess = true;
                return stopResult;
            }
            catch (Exception ex)
            {
                stopResult.IsSuccess = true;
                stopResult.Message = OperateResult.ConvertException(ex);
            }
            return stopResult;
        }
    }
}
