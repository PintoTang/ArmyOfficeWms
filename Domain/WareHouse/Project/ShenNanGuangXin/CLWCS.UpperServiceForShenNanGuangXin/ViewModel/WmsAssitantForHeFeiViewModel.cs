using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel;
using CLDC.CLWS.CLWCS.UpperService.Communicate;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.UpperServiceForHeFei.Interface.View;
using CLWCS.UpperServiceForHeFei.Interface.ViewModel;
using CLWCS.UpperServiceForHeFei.Target;
using GalaSoft.MvvmLight;

namespace CLWCS.UpperServiceForHeFei.ViewModel
{
    public class WmsAssitantForHeFeiViewModel:ViewModelBase
    {
        public MethodExcute DataModel { get; set; }
        public WmsAssitantForHeFeiViewModel(MethodExcute wmsService)
        {
            DataModel = wmsService;
            InitWebApiViewModel();
        }
        public WebApiViewModel WebApiViewModel { get; set; }
        private void InitWebApiViewModel()
        {
            WebApiViewModel = new WebApiViewModel(DataModel.Name, DataModel.WebserviceUrl, InvokeApi);
            WebApiViewModel.DicApiNameList = DicApiNameList;
            WebApiViewModel.DicMethodCmd = DicMethodCmd;
        }

        private string InvokeApi(WebApiInvokeCmd cmd)
        {
            OperateResult<object> opResult = ExecInvokeMethod(cmd);
            if (opResult.IsSuccess)
            {
                //返回数据
                return opResult.Content.ToString();
            }
            else
            {
                return opResult.Message;
            }

        }

        public OperateResult<object> ExecInvokeMethod(WebApiInvokeCmd cmd)
        {
            OperateResult<object> opResult = new OperateResult<object>();
            try
            {
                NotifyElement notify = new NotifyElement("Barcode", cmd.MethodName, cmd.MethodName, null, cmd.InvokeCmd);
                //组Json
                opResult = DataModel.Invoke(notify);
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Content = OperateResult.ConvertException(ex);
            }
            return opResult;
        }



        private Dictionary<string, object> _dicMethodCmd = new Dictionary<string, object>();
        public Dictionary<string, object> DicMethodCmd
        {
            get
            {
                if (_dicMethodCmd.Count == 0)
                {
                    InitMethodAndCmd();
                }
                return _dicMethodCmd;
            }
        }

        private void InitMethodAndCmd()
        {
            //处理问题：判断条件，调用线程必须为STA，因为许多UI组件都需要
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyPackageBarcodeCheck.ToString(), new NotifyPageSkuBindBarcodeCmdView(DemoNotifyPackageSkuBindBarcodeCmd));
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyInstructCancel.ToString(), new NotifyInstructCancleView(DemoNotifyInstructCancleCmdMode));
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyInstructException.ToString(), new NotifyInstructExceptionView(DemoNotifyInstructExceptionCmd));
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyInstructFinish.ToString(), new NotifyInstructFinishView(DemoNotifyInstructFinishMode));
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyPalletizerFinish.ToString(), new NotifyPalletizerFinishView(DemoNotifyPalletizerFinishCmd));
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyStocktakingInstructResult.ToString(), new NotifyStocktakingResultCmdView(DemoNotifyStocktakingInstructResultCmd));
                    _dicMethodCmd.Add(WmsServiceForHeFeiEnum.NotifyUnstackFinish.ToString(), new NotifyUnstackFinishCmdView(DemoNotifyUnstackFinishCmdMode));
                });
        }

        public NotifyPackageSkuBindBarcodeCmdMode DemoNotifyPackageSkuBindBarcodeCmd
        {
            get { return new NotifyPackageSkuBindBarcodeCmdMode(); }
        }

        public NotifyInstructCancleCmdMode DemoNotifyInstructCancleCmdMode
        {
            get { return new NotifyInstructCancleCmdMode();}
        }

        public NotifyPalletizerFinishCmdMode DemoNotifyPalletizerFinishCmd
        {
            get { return new NotifyPalletizerFinishCmdMode(); }
        }

        public NotifyStocktakingResultCmdMode DemoNotifyStocktakingInstructResultCmd
        {
            get { return new NotifyStocktakingResultCmdMode(); }
        }


        public NotifyInstructFinishCmdMode DemoNotifyInstructFinishMode
        {
            get { return new NotifyInstructFinishCmdMode(); }
        }

        private NotifyInstructExceptionMode DemoNotifyInstructExceptionCmd
        {
            get { return new NotifyInstructExceptionMode { CURRENT_ADDR = "Addr", EXCEPTION_CODE = "ExpCode", INSTRUCTION_CODE = "Code" }; }
        }

        private NotifyUnstackFinishCmdMode DemoNotifyUnstackFinishCmdMode
        {
            get { return new NotifyUnstackFinishCmdMode();}
        }

        private Dictionary<string, string> _dicApiNameList = new Dictionary<string, string>();
        /// <summary>
        /// 接口方法列表
        /// </summary>
        public Dictionary<string, string> DicApiNameList
        {
            get
            {
                if (_dicApiNameList.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(WmsServiceForHeFeiEnum)))
                    {
                        WmsServiceForHeFeiEnum em = (WmsServiceForHeFeiEnum)value;
                        _dicApiNameList.Add(em.ToString(), em.GetDescription());
                    }
                }
                return _dicApiNameList;
            }
            set
            {
                _dicApiNameList = value;
                RaisePropertyChanged("DicApiNameList");
            }
        }

    }
}
