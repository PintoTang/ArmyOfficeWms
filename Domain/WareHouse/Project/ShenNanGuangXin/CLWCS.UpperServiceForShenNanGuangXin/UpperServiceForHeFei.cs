using System;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLWCS.UpperServiceForHeFei.Target;

namespace CLWCS.UpperServiceForHeFei
{
    /// <summary>
    /// 上层接口
    /// </summary>
    public class UpperServiceForHeFei : IWmsService, INotifyInstruct
    {

        private readonly IWebNetInvoke _webNetInvoke;
        private string Url = string.Empty;
        private int TimeOut = 10;
        public UpperServiceForHeFei(string url, int timeOut, CommunicationModeEnum communicationMode)
        {
            _webNetInvoke = new WebApiInvoke();// WebApi
            _webNetInvoke.LogDisplayName = "WMS接口服务调用";
            _webNetInvoke.CommunicationMode = communicationMode;
            this.Url = url;
            this.TimeOut = timeOut * 1000;
        }

        public string NotifyInstructCancel(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyInstructCancel.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyInstructException(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyInstructException.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyInstructFinish(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyInstructFinish.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyStocktakingInstructResult(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyStocktakingInstructResult.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyUnstackFinish(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyUnstackFinish.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyPalletizerFinish(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyPalletizerFinish.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string NotifyOutFinish(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, "NotifyOutFinish", cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ApplyEmptyTrayOut(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.ApplyEmptyTrayOut.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ApplyEmptyTrayIn(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.ApplyEmptyTrayIn.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string NotifyPackageBarcodeCheck(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyPackageBarcodeCheck.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyPackagePutFinish(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyPackagePutFinish.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyPackagePutFinishForOut(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyPackagePutFinishForOut.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReportTroubleStatus(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.ReportTroubleStatus.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ReportNodeStatus(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.ReportNodeStatus.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string OpenOrCloseDoor(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.OpenOrCloseDoor.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string NotifyPackageBarcodeCheckNew(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyPackageBarcodeCheckNew.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyPackagePutFinishNew(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyPackagePutFinishNew.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyCrossDoor(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyCrossDoor.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyAgvCarry(string cmd)
        {

            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyAgvCarry.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyBarcodeReqNewInstruct(string cmd)
        {

            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyBarcodeReqNewInstruct.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string NotifyApplyContainer(string cmd)
        {

            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyApplyContainer.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string NotifyApplyIn(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyApplyIn.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NotifyDoorOpenOrCloseFinished(string cmd)
        {
            try
            {
                OperateResult<string> responseResult = _webNetInvoke.ServiceRequest<SyncResReErr>(Url, WmsServiceForHeFeiEnum.NotifyDoorOpenOrCloseFinished.ToString(), cmd);
                return responseResult.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}