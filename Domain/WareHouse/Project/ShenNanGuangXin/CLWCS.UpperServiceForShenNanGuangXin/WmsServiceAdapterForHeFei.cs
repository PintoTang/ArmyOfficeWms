using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.Communicate;
using CLWCS.UpperServiceForHeFei.Target;

namespace CLWCS.UpperServiceForHeFei
{
    public class WmsServiceAdapterForHeFei : MethodExcute, IWmsService, INotifyInstruct
    {
        private UpperServiceForHeFei _upperService;
        public override OperateResult ParticularInitilize()
        {
           OperateResult initCommResult= InitializeCommunicate();
            if (!initCommResult.IsSuccess)
            {
                return initCommResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult InitializeCommunicate()
        {
            _upperService = new UpperServiceForHeFei(WebserviceUrl, TimeOut, CommunicationMode);
            return OperateResult.CreateSuccessResult();
        }

        public string NotifyBarcodeReqNewInstruct(string cmd)
        {
            return _upperService.NotifyBarcodeReqNewInstruct(cmd);
        }

        /// <summary>
        /// 上报盘点结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyStocktakingInstructResult(string cmd)
        {
            return _upperService.NotifyStocktakingInstructResult(cmd);
        }
        /// <summary>
        /// 上报拆垛完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyUnstackFinish(string cmd)
        {
            return _upperService.NotifyUnstackFinish(cmd);
        }
        /// <summary>
        /// 上报码盘完成（整托）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyPalletizerFinish(string cmd)
        {
            return _upperService.NotifyPalletizerFinish(cmd);
        }
        public string NotifyOutFinish(string cmd)
        {
            return _upperService.NotifyOutFinish(cmd);
        }

        
        /// <summary>
        /// 指令完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyInstructFinish(string cmd)
        {
            return _upperService.NotifyInstructFinish(cmd);
        }
        /// <summary>
        /// 指令取消
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyInstructCancel(string cmd)
        {
            return _upperService.NotifyInstructCancel(cmd);
        }
        /// <summary>
        /// 指令异常
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyInstructException(string cmd)
        {
            return _upperService.NotifyInstructException(cmd);
        }

        //public string IsAllowOut(string cmd)
        //{
        //    return _upperService.IsAllowOut(cmd);
        //}

        //public string IsFinished(string cmd)
        //{
        //    return _upperService.IsFinished(cmd);
        //}


        //public string IsAllowIn(string cmd)
        //{
        //    return _upperService.IsAllowIn(cmd);
        //}
      
        /// <summary>
        /// 申请空托盘出库
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ApplyEmptyTrayOut(string cmd)
        {
            return _upperService.ApplyEmptyTrayOut(cmd);
        }
        /// <summary>
        /// 申请空托盘入库
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ApplyEmptyTrayIn(string cmd)
        {
            return _upperService.ApplyEmptyTrayIn(cmd);
        }
        /// <summary>
        /// CS上报包装条码给wms，wms进行验证
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        public string NotifyPackageBarcodeCheck(string cmd)
        {
            return _upperService.NotifyPackageBarcodeCheck(cmd);
        }
        /// <summary>
        /// CS上报箱子放货完成（单次）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyPackagePutFinish(string cmd)
        {
            return _upperService.NotifyPackagePutFinish(cmd);
        }

        public string NotifyPackagePutFinishForOut(string cmd)
        {
            return _upperService.NotifyPackagePutFinishForOut(cmd);
        }


        public string ReportTroubleStatus(string cmd)
        {
            return _upperService.ReportTroubleStatus(cmd);
        }
        public string ReportNodeStatus(string cmd)
        {
            return _upperService.ReportNodeStatus(cmd);
        }


        public string OpenOrCloseDoor(string cmd)
        {
            return _upperService.OpenOrCloseDoor(cmd);
        }


        public string NotifyPackageBarcodeCheckNew(string cmd)
        {
            return _upperService.NotifyPackageBarcodeCheckNew(cmd);
        }

        public string NotifyPackagePutFinishNew(string cmd)
        {
            return _upperService.NotifyPackagePutFinishNew(cmd);
        }
        public string NotifyCrossDoor(string cmd)
        {
            return _upperService.NotifyCrossDoor(cmd);
        }

        public string NotifyAgvCarry(string cmd)
        {
            return _upperService.NotifyAgvCarry(cmd);
        }

        public string NotifyApplyContainer(string cmd)
        {
            return _upperService.NotifyApplyContainer(cmd);
        }
        public string NotifyApplyIn(string cmd)
        {
            return _upperService.NotifyApplyIn(cmd);
        }

        public string NotifyDoorOpenOrCloseFinished(string cmd)
        {
            return _upperService.NotifyDoorOpenOrCloseFinished(cmd);
        }
    }
}
