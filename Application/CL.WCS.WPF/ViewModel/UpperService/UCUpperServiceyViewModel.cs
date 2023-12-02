using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.UpperService.Communicate;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RelayCommand = CLDC.CLWS.CLWCS.Infrastructrue.DataModel.RelayCommand;

namespace CL.WCS.WPF
{
    public class UCUpperServiceyViewModel : WareHouseViewModelBase
    {
        private MethodExcute methodExcute;
        private UpperInterfaceDataAbstract upperInterfaceDataAbstract;
        private UcSplitPagerViewModel pageViewModel;
        public UcSplitPagerViewModel PageViewModel
        {
            get { return pageViewModel; }
            set
            {
                pageViewModel = value;
                RaisePropertyChanged("PageViewModel");
            }
        }
       
        public UCUpperServiceyViewModel(MethodExcute tempMethodExcute)
        {
            this.methodExcute = tempMethodExcute;
            this.upperInterfaceDataAbstract = DependencyHelper.GetService<UpperInterfaceDataAbstract>();
            PageViewModel = new UcSplitPagerViewModel();
            PageViewModel.PageChange += InitDataList;
        }


        #region 属性
        private string s_WebServiceUrl;
        //WebServiceUrl地址
        public string S_WebServiceUrl
        {
            get
            {
                if (methodExcute != null)
                {
                    s_WebServiceUrl = methodExcute.WebserviceUrl;
                }
                return s_WebServiceUrl;
            }
            set
            {
                if (s_WebServiceUrl != value)
                {
                    s_WebServiceUrl = value;
                    RaisePropertyChanged("S_WebServiceUrl");
                }
            }
        }
        private string s_TimeOut;
        //超时时间
        public string S_TimeOut
        {
            get
            {
                if (methodExcute != null)
                {
                    s_TimeOut = methodExcute.TimeOut.ToString();
                }
                return s_TimeOut;
            }
            set
            {
                if (s_TimeOut != value)
                {
                    s_TimeOut = value;
                    RaisePropertyChanged("S_TimeOut");
                }
            }
        }



        private string s_InputParms;
        //输入参数
        public string S_InputParms
        {
            get { return s_InputParms; }
            set
            {
                if (s_InputParms != value)
                {
                    s_InputParms = value;
                    RaisePropertyChanged("S_InputParms");
                }
            }
        }
        private string s_OutputParms;
        //输出参数
        public string S_OutputParms
        {
            get { return s_OutputParms; }
            set
            {
                if (s_OutputParms != value)
                {
                    s_OutputParms = value;
                    RaisePropertyChanged("S_OutputParms");
                }
            }
        }
        private string s_DisPatchInterefaceMethodName;
        //接口方法名称（调用 使用）
        public string S_DisPatchInterefaceMethodName
        {
            get { return s_DisPatchInterefaceMethodName; }
            set
            {
                if (s_DisPatchInterefaceMethodName != value)
                {
                    s_DisPatchInterefaceMethodName = value;
                    RaisePropertyChanged("S_DisPatchInterefaceMethodName");
                }
            }
        }

        //********************************************
        ///查询参数
        private string s_MethodName;
        public string S_MethodName
        {
            get { return s_MethodName; }
            set
            {
                s_MethodName = value;
                RaisePropertyChanged("S_MethodName");
            }
        }
        private string s_BusinessName;
        public string S_BusinessName
        {
            get { return s_BusinessName; }
            set
            {
                s_BusinessName = value;
                RaisePropertyChanged("S_BusinessName");
            }
        }
        private string s_InvokeStatus;
        public string S_InvokeStatus
        {
            get { return s_InvokeStatus; }
            set
            {
                s_InvokeStatus = value;
                RaisePropertyChanged("S_InvokeStatus");
            }
        }
        private string s_Result;
        public string S_Result
        {
            get { return s_Result; }
            set
            {
                s_Result = value;
                RaisePropertyChanged("S_Result");
            }
        }
        private string s_CallBackFuncName;
        public string S_CallBackFuncName
        {
            get { return s_CallBackFuncName; }
            set
            {
                s_CallBackFuncName = value;
                RaisePropertyChanged("S_CallBackFuncName");
            }
        }

        private string s_Parameters;
        public string S_Parameters
        {
            get { return s_Parameters; }
            set
            {
                s_Parameters = value;
                RaisePropertyChanged("S_Parameters");
            }
        }
        private string s_AddDateStartTime;
        public string S_AddDateStartTime
        {
            get { return s_AddDateStartTime; }
            set
            {
                s_AddDateStartTime = value;
                RaisePropertyChanged("S_AddDateStartTime");
            }
        }
        private string s_AddDateEndTime;
        public string S_AddDateEndTime
        {
            get { return s_AddDateEndTime; }
            set
            {
                s_AddDateEndTime = value;
                RaisePropertyChanged("S_AddDateEndTime");
            }
        }

        #endregion


        private List<UpperInterfaceInvoke> allDataLst = new List<UpperInterfaceInvoke>();
        /// <summary>
        /// 所有数据
        /// </summary>
        public List<UpperInterfaceInvoke> AllDataList
        {
            set
            {
                allDataLst = value;
                RaisePropertyChanged("TotalSize");
            }
            get { return allDataLst; }
        }


        private List<UpperInterfaceInvoke> currentShowlist = new List<UpperInterfaceInvoke>();
        /// <summary>
        /// 当前显示的数据
        /// </summary>
        public List<UpperInterfaceInvoke> CurrentShowList
        {
            get { return currentShowlist; }
            set
            {
                currentShowlist = value;
                RaisePropertyChanged("CurrentShowList");
            }

        }

        /// <summary>
        /// 初始化 计算总页数
        /// </summary>
        private void InitDataList()
        {
            PageViewModel.TotalCount = 0;
            CurrentShowList = new List<UpperInterfaceInvoke>();
            if (AllDataList == null || AllDataList.Count == 0)
            {
                PageViewModel.CurrentPage = 0;
                PageViewModel.TotalPageSize = 0;
                return;
            }
            if (AllDataList.Count % PageViewModel.PageSize == 0)
            {
                PageViewModel.TotalPageSize = AllDataList.Count / PageViewModel.PageSize;
            }
            else
            {
                PageViewModel.TotalPageSize = AllDataList.Count / PageViewModel.PageSize + 1;
            }
            if (AllDataList.Count > PageViewModel.PageSize)
            {
                if (AllDataList == null) return;
                var query = from order in AllDataList select order;
                var collection = query.Skip((int)(PageViewModel.PageIndex * PageViewModel.PageSize)).Take((int)PageViewModel.PageSize);
                CurrentShowList = collection.ToList();
            }
            else
            {
                CurrentShowList = AllDataList;
            }
            PageViewModel.TotalCount = AllDataList.Count;
        }



        private RelayCommand searchDataCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand SearchDataCommand
        {
            get
            {
                if (searchDataCommand == null)
                {
                    searchDataCommand = new RelayCommand(SwitchToSearchData);
                }
                return searchDataCommand;
            }
        }
        private RelayCommand execCommand;
        /// <summary>
        /// 执行调用 
        /// </summary>
        public RelayCommand ExecCommand
        {
            get
            {
                if (execCommand == null)
                {
                    execCommand = new RelayCommand(ExecInvokeMethod);
                }
                return execCommand;
            }
        }
        private RelayCommand clearInputCommand;
        /// <summary>
        /// 清理input数据 
        /// </summary>
        public RelayCommand ClearInputCommand
        {
            get
            {
                if (clearInputCommand == null)
                {
                    clearInputCommand = new RelayCommand(ClearInputParmsData);
                }
                return clearInputCommand;
            }
        }
        private RelayCommand clearOutputCommand;
        /// <summary>
        /// 清理output数据 
        /// </summary>
        public RelayCommand ClearOutputCommand
        {
            get
            {
                if (clearOutputCommand == null)
                {
                    clearOutputCommand = new RelayCommand(ClearOutputParmsData);
                }
                return clearOutputCommand;
            }
        }
        public void ExecInvokeMethod()
        {
            OperateResult<string> opResult = new OperateResult<string>();
            try
            {
                //组Json
                opResult = HttpHelper.Post(S_WebServiceUrl + "/" + S_DisPatchInterefaceMethodName, S_InputParms);
                if (opResult.IsSuccess)
                {
                    S_OutputParms = opResult.Content;
                }
                else
                {
                    S_OutputParms = opResult.Message;
                }

                //NotifyElement element = new NotifyElement(S_DisPatchInterefaceMethodName, "请求上架", null, S_InputParms);
                //OperateResult<object> result = this.methodExcute.Invoke(element);
                //S_OutputParms = result.Message;
            }
            catch (Exception ex)
            {
                S_OutputParms = OperateResult.ConvertException(ex);
            }
        }


        public void ClearInputParmsData()
        {
            S_InputParms = "";
        }
        public void ClearOutputParmsData()
        {
            S_OutputParms = "";
        }
        /// <summary>
        /// 查询
        /// </summary>
        public void SwitchToSearchData()
        {
            SwitchToCurDataPage();
            InitDataList();
        }

        /// <summary>
        /// 当前数据
        /// </summary>
        private void SwitchToCurDataPage()
        {
            var where = GetSql(S_MethodName, S_BusinessName, S_InvokeStatus, S_Result,
                                    S_CallBackFuncName, S_Parameters, S_AddDateStartTime, S_AddDateEndTime);
            var op = this.upperInterfaceDataAbstract.GetNotifyElementByWhere(where);
            if (op.IsSuccess)
            {
                AllDataList = op.Content;
            }
            else
            {
                AllDataList.Clear();
            }
        }
        public Expression<Func<UpperInterfaceInvoke, bool>> GetSql(string S_MethodName, string S_BusinessName, string S_InvokeStatus, string S_Result,
            string S_CallBackFuncName, string S_Parameters, string S_AddDateStartTime, string S_AddDateEndTime)
        {
            Expression<Func<UpperInterfaceInvoke, bool>> where = t => 1 == 1;
            //string sqlstr = string.Format("select * from T_BO_UPPERINTERFACEINVOKE where  1 ='{0}' ", "1");

            if (!string.IsNullOrEmpty(S_MethodName))
            {
                where = where.AndAlso(t => t.MethodName.Contains(S_MethodName));
                //sqlstr += " and METHODNAME like'" + "%" + S_MethodName + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_BusinessName))
            {
                where = where.AndAlso(t => t.BusinessName.Contains(S_BusinessName));
                //sqlstr += " and BUSINESSNAME like'" + "%" + S_BusinessName + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_InvokeStatus))
            {
                where = where.AndAlso(t => t.InvokeStatus==(InvokeStatusMode)Convert.ToInt32(S_InvokeStatus));
                //sqlstr += " and INVOKESTATUS like'" + "%" + S_InvokeStatus + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_Result))
            {
                where = where.AndAlso(t => t.Result == Convert.ToInt32(S_Result));
                //sqlstr += " and RESULT like'" + "%" + S_Result + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_CallBackFuncName))
            {
                where = where.AndAlso(t => t.CallBackFuncName.Contains(S_CallBackFuncName));
                //sqlstr += " and CALLBACKFUNC like'" + "%" + S_CallBackFuncName + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_Parameters))
            {
                where = where.AndAlso(t => t.Parameters.Contains(S_Parameters));
                //sqlstr += " and INVOKEPARAMETERS like'" + "%" + S_Parameters + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_AddDateStartTime) && !string.IsNullOrEmpty(S_AddDateEndTime))
            {
                DateTime startDate = Convert.ToDateTime(S_AddDateStartTime);
                DateTime endDate = Convert.ToDateTime(S_AddDateEndTime);
                where = where.AndAlso(t => t.AddDateTime>=startDate && t.AddDateTime<=endDate);
                //sqlstr += string.Format(" and  ADDDATE between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);
            }
            return where;
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}
