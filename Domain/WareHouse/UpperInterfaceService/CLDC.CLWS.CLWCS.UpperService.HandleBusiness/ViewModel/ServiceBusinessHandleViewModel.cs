using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness.ViewModel
{
    public class ServiceBusinessHandleViewModel : ViewModelBase
    {
        public ServiceBusinessHandleViewModel(UpperServiceBusinessAbstract dataModel)
        {
            _upperInterfaceDataAbstract = DependencyHelper.GetService<UpperInterfaceDataAbstract>();
            _pageViewModel = new UcSplitPagerViewModel();
            _pageViewModel.PageChange += UpdateData;
            UpdateData();
            ServiceBusinessHandle = dataModel;
            ServiceBusinessHandle.NotifyMsgToUiEvent += ShowLogMessage;
        }
        private List<UpperInterfaceInvoke> _currentShowlist = new List<UpperInterfaceInvoke>();
        private readonly UpperInterfaceDataAbstract _upperInterfaceDataAbstract;
        public UpperServiceBusinessAbstract ServiceBusinessHandle { get; set; }

        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }



        /// <summary>
        /// 当前显示的数据
        /// </summary>
        public List<UpperInterfaceInvoke> CurrentShowList
        {
            get { return _currentShowlist; }
            set
            {
                _currentShowlist = value;
                RaisePropertyChanged();
            }

        }
        private UcSplitPagerViewModel _pageViewModel;

        public UcSplitPagerViewModel PageViewModel
        {
            get { return _pageViewModel; }
            set
            {
                _pageViewModel = value;
                RaisePropertyChanged();
            }
        }

        private readonly Dictionary<string, string> _dicHandleStatusEnum = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> DicHandleStatusEnum
        {
            get
            {
                if (_dicHandleStatusEnum.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(InvokeStatusMode)))
                    {
                        InvokeStatusMode em = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), strName);
                        _dicHandleStatusEnum.Add(((int)em).ToString(), em.GetDescription());
                    }
                }
                return _dicHandleStatusEnum;
            }
        }

        protected void ShowLogMessage(string msg, EnumLogLevel level)
        {

            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(new Action<string, EnumLogLevel>((m, l) =>
                {
                    ClearLogMsg();
                    UcStationLogInfoModel logMsg = new UcStationLogInfoModel
                    {
                        DateTime = DateTime.Now.ToString("MM/dd HH:mm:ss"),
                        Content = m,
                        Level = level
                    };
                    LogInfoModelLst.Add(logMsg);
                }), DispatcherPriority.Background, msg, level);
        }
        private ObservableCollection<UcStationLogInfoModel> _logInfoModelLst = new ObservableCollection<UcStationLogInfoModel>();
        public ObservableCollection<UcStationLogInfoModel> LogInfoModelLst
        {
            get { return _logInfoModelLst; }
            set
            {
                if (_logInfoModelLst != null && _logInfoModelLst != value)
                {
                    _logInfoModelLst = value;
                }
            }
        }
        private void ClearLogMsg()
        {
            lock (LogInfoModelLst)
            {
                if (LogInfoModelLst.Count >= 30)//100改30 不用太多日志
                {
                    LogInfoModelLst.RemoveAt(0);
                }
            }
        }

        private UpperInterfaceInvokeFilter CombineFilter(long pagIndex, long pageSize)
        {
            UpperInterfaceInvokeFilter filterModel = new UpperInterfaceInvokeFilter();
            filterModel.HandleFromTime = this.HandleFromTime;
            filterModel.HandleStatus = HandleStatus;
            filterModel.Barcode = this.Barcode;
            filterModel.HandleResult = this.HandleResult;
            filterModel.HandleToTime = this.HandleToTime;
            filterModel.MethodName = this.MethodName;
            filterModel.PageIndex = pagIndex;
            filterModel.PageSize = pageSize;
            return filterModel;
        }

        public string HandleResult
        {
            get { return _handleResult; }
            set { _handleResult = value; }
        }

        public string Barcode
        {
            get { return _barcode; }
            set { _barcode = value; }
        }

        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }

        public string HandleToTime
        {
            get { return _handleToTime; }
            set { _handleToTime = value; }
        }

        public string HandleStatus
        {
            get { return _handleStatus; }
            set { _handleStatus = value; }
        }

        public string HandleFromTime
        {
            get { return _handleFromTime; }
            set { _handleFromTime = value; }
        }

        //private void InitPage()
        //{
        //    long totalCount = _upperInterfaceDataAbstract.GetTotalCount();
        //    PageViewModel.TotalCount = totalCount;
        //}

        private void UpdateData()
        {
            //InitPage();
            UpperInterfaceInvokeFilter filterModel = CombineFilter(PageViewModel.PageIndex, PageViewModel.PageSize);
            if (!string.IsNullOrEmpty(filterModel.MethodName))
            {
                if (filterModel.MethodName.Length > 20)
                {
                    MessageBoxEx.Show("输入接口名称 数据过长，请重新输入！");
                    return;
                }
                else
                {
                    //验证特殊字符
                    bool isContainSpecialCharacters = ValidData.CheckSpecialCharacters(filterModel.MethodName);
                    if (isContainSpecialCharacters)
                    {
                        MessageBoxEx.Show("输入接口名称 包含特殊字符，请重新输入！");
                        return;
                    }
                }
            }

            CurrentShowList = _upperInterfaceDataAbstract.SelectData(filterModel,out int totalCount);

            PageViewModel.TotalCount = totalCount;
        }

        private RelayCommand _searchCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(UpdateData);
                }
                return _searchCommand;
            }
        }

        private MyCommand _reInvokeCommand;
        public MyCommand ReInvokeCommand
        {
            get
            {
                if (_reInvokeCommand == null)
                {
                    _reInvokeCommand = new MyCommand(ReInvoke);
                }
                return _reInvokeCommand;
            }
        }

        private void ReInvoke(object obj)
        {
            if (obj is UpperInterfaceInvoke)
            {
                UpperInterfaceInvoke invoke = obj as UpperInterfaceInvoke;
                NotifyElement notifyElement = new NotifyElement(invoke.Barcode, invoke.MethodName, invoke.BusinessName, null, invoke.Parameters);
                OperateResult invokeResult = ServiceBusinessHandle.Invoke(notifyElement);
                string msg = invokeResult.IsSuccess ? "调用成功" : string.Format("调用失败：{0}", invokeResult.Message);
                string recordMsg = string.Format("手动调用上层业务接口：{0} 参数：{1} 调用结果：{2}", notifyElement.MethodName, notifyElement.ParameterToString(), msg);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要重新调用的数据");
            }
        }

        private RelayCommand _pauseCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                {
                    _pauseCommand = new RelayCommand(PauseAndRun);
                }
                return _pauseCommand;
            }
        }
        private void PauseAndRun()
        {
            if (ServiceBusinessHandle.CurRunState.Equals(RunStateMode.Run))
            {
                string recordMsg = "暂停上层业务接口异步接口池调用";
                OperateLogHelper.Record(recordMsg);
                ServiceBusinessHandle.Pause();    
            }
            else if (ServiceBusinessHandle.CurRunState.Equals(RunStateMode.Pause))
            {
                string recordMsg = "开始上层业务接口异步接口池调用";
                OperateLogHelper.Record(recordMsg);
                ServiceBusinessHandle.Run();
            }
        }

        private RelayCommand _resetCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(Reset);
                }
                return _resetCommand;
            }
        }

        private void Reset()
        {
            OperateResult resetResult = ServiceBusinessHandle.Reset();
            if (resetResult.IsSuccess)
            {
                string recordMsg = "复位上层业务接口异步接口池调用成功";
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue("复位成功");
            }
            else
            {
                string recordMsg = "复位上层业务接口异步接口池调用失败";
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue(string.Format("复位失败:{0}", resetResult.Message));
            }
        }



        

        private RelayCommand _openAssistantCommand;

        public RelayCommand OpenAssistantCommand
        {
            get
            {
                if (_openAssistantCommand == null)
                {
                    _openAssistantCommand = new RelayCommand(OpenAssistantView);
                }
                return _openAssistantCommand;
            }
        }

        private void OpenAssistantView()
        {
            Window assistantView = ServiceBusinessHandle.GetAssistantView();
            if (assistantView == null)
            {
                return;
            }
            assistantView.ShowDialog();
        }



        private RelayCommand _invokeApiCommand;
        public RelayCommand InvokeApiCommand
        {
            get
            {
                if (_invokeApiCommand == null)
                {
                    _invokeApiCommand = new RelayCommand(InvokeApi);
                }
                return _invokeApiCommand;
            }
        }

        private void InvokeApi()
        {
            try
            {
                string methodName = SelectedApiName;
                string businessName = DicApiNameList[methodName];
                object[] para = { RequestValue };
                NotifyElement notifyElement = new NotifyElement("Barcode", methodName, businessName, null, para);
                OperateResult<object> reponseResult = ServiceBusinessHandle.Invoke(notifyElement);
                if (reponseResult.IsSuccess)
                {
                    string recordMsg = string.Format("调用上层接口：{0} 参数：{1} 成功",notifyElement.MethodName,notifyElement.ParameterToString());
                    OperateLogHelper.Record(recordMsg);

                    ReponseValue = reponseResult.Content.ToString();
                }
                else
                {
                    string recordMsg = string.Format("调用上层接口：{0} 参数：{1} 失败", notifyElement.MethodName, notifyElement.ParameterToString());
                    OperateLogHelper.Record(recordMsg);
                    ReponseValue = reponseResult.Message;
                }

            }
            catch (Exception ex)
            {
                string msg = OperateResult.ConvertException(ex);
                ReponseValue = msg;
            }
        }

        private string _requestValue;
        //输入参数
        public string RequestValue
        {
            get { return _requestValue; }
            set
            {
                if (_requestValue != value)
                {
                    _requestValue = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _reponseValue;
        //输出参数
        public string ReponseValue
        {
            get { return _reponseValue; }
            set
            {
                if (_reponseValue != value)
                {
                    _reponseValue = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _selectedApiName;
        /// <summary>
        /// 当前选择的接口名称
        /// </summary>
        public string SelectedApiName
        {
            get { return _selectedApiName; }
            set
            {
                if (_selectedApiName != null && _selectedApiName != value)
                {
                    _selectedApiName = value;
                }
                RaisePropertyChanged();
            }
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
                    _dicApiNameList.Add("NotifyPutaway", "请求上架");
                    _dicApiNameList.Add("NotifyInstructFinish", "上报指令完成");
                    _dicApiNameList.Add("NotifyInstructException", "上报指令异常");
                    _dicApiNameList.Add("NotifyInstructCancel", "上报指令取消");
                    _dicApiNameList.Add("NotifyPalletsOutbound", "请求空托盘");
                }
                return _dicApiNameList;
            }
            set
            {
                _dicApiNameList = value;
                RaisePropertyChanged();
            }
        }


    
      

        private MyCommand _openLogCommand;
        private string _handleResult=string.Empty;
        private string _barcode=string.Empty;
        private string _methodName = string.Empty;
        private string _handleToTime = string.Empty;
        private string _handleStatus = string.Empty;
        private string _handleFromTime = string.Empty;

        /// <summary>
        /// 打开当前日志
        /// </summary>
        public MyCommand OpenLogCommand
        {
            get
            {
                if (_openLogCommand == null)
                    _openLogCommand = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              LogManageHelper.Instance.OpenLogFile(ServiceBusinessHandle.Name);
                          }
                        ));
                return _openLogCommand;
            }
        }
    }
}
