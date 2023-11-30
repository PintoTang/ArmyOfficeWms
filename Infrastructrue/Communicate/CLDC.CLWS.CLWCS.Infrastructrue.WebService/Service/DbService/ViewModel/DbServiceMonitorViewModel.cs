using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;

//using DataMonitor = CLDC.CLWS.CLWCS.WareHouse.BusinessHandleAbstract.DataMonitor;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.ViewModel
{
    public class DataList
    {
        /// <summary>
        /// 仓库代码
        /// </summary>
        public string WH_Code { get; set; }
        /// <summary>
        /// 处理状态 ID
        /// </summary>
        public string DHSTATUS_ID { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string RD_METHODNAME { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public string RD_PARAMVALUE { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public string RD_RECEIVEDATE { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string RD_HANDLERDATE { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string RD_NOTE { get; set; }
    }

    /// <summary>
    /// 服务数据监控 ViewModel
    /// </summary>
    public class DbServiceMonitorViewModel : WareHouseViewModelBase
    {
        public event Action InvalidPage;
        public DbServiceMonitor ServiceMonitor;
        private UcSplitPagerViewModel _pageViewModel;
        public UcSplitPagerViewModel PageViewModel
        {
            get { return _pageViewModel; }
            set
            {
                _pageViewModel = value;
                RaisePropertyChanged("PageViewModel");
            }
        }
        public DbServiceMonitorViewModel(DbServiceMonitor dataMonitor)
        {
            this.ServiceMonitor = dataMonitor;

            PageViewModel = new UcSplitPagerViewModel();
            PageViewModel.PageChange += InitDataList;
            InitDataList();
        }

        private string s_MonitorState = "1000";
        /// <summary>
        /// 模拟状态
        /// </summary>
        public string S_MonitorState
        {
            get
            {
                if (this.ServiceMonitor != null)
                {
                    Enum curState = (RunStateMode)Enum.Parse(typeof(RunStateMode), this.ServiceMonitor.CurRunState.ToString());
                    s_MonitorState = GetDescription(curState);
                }
                return s_MonitorState;
            }
            set
            {
                if (s_MonitorState != value)
                {
                    s_MonitorState = value;
                    RaisePropertyChanged("S_MonitorState");
                }
            }
        }
        private string s_BtnStartOrPauseContent = "开始";
        public string S_BtnStartOrPauseContent
        {
            get { return s_BtnStartOrPauseContent; }
            set
            {
                s_BtnStartOrPauseContent = value;
                RaisePropertyChanged("S_BtnStartOrPauseContent");
            }
        }


        private string s_MonitorInterval = "1000";
        /// <summary>
        /// 时间间隔
        /// </summary>
        public string S_MonitorInterval
        {
            get
            {
                return s_MonitorInterval;
            }
            set
            {
                if (s_MonitorInterval != value)
                {
                    s_MonitorInterval = value;
                    RaisePropertyChanged("S_MonitorInterval");
                }
            }
        }
        private string s_SelectSql;
        /// <summary>
        /// 查询语句
        /// </summary>
        public string S_SelectSql
        {
            get
            {
                if (this.ServiceMonitor != null)
                {
                    s_SelectSql = this.ServiceMonitor.SelectSql;
                }
                return s_SelectSql;
            }
            set
            {
                if (s_SelectSql != value)
                {
                    s_SelectSql = value;
                    RaisePropertyChanged("S_SelectSql");
                }
            }
        }

        private string s_UpdateSql;
        /// <summary>
        /// 更新语句
        /// </summary>
        public string S_UpdateSql
        {
            get
            {
                if (this.ServiceMonitor != null)
                {
                    s_UpdateSql = this.ServiceMonitor.UpdateSql;
                }
                return s_UpdateSql;
            }
            set
            {
                if (s_UpdateSql != value)
                {
                    s_UpdateSql = value;
                    RaisePropertyChanged("S_UpdateSql");
                }
            }
        }


        /// <summary>  
        /// 获取枚举描述
        /// </summary>  
        /// <param name="en">枚举</param>  
        /// <returns>返回枚举的描述 </returns>  
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();   //获取类型  
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                as DescriptionAttribute[];   //获取描述特性  
                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述
                }
            }
            return en.ToString();

        }

        /// <summary>
        /// 初始化 计算总页数
        /// </summary>
        private void InitDataList()
        {
            PageViewModel.TotalCount = 0;
            CurrentShowList = new List<DataList>();
            if (AllDataList == null || AllDataList.Count == 0)
            {
                PageViewModel.CurrentPage = 0;
                return;
            }
            PageViewModel.TotalPageSize = (long)Math.Ceiling((double)AllDataList.Count / PageViewModel.PageSize);
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


        ///// <summary>
        /////  StatusEnum 通过描述 找到 StatusEnum对象
        ///// </summary>
        ///// <param name="strDescription"></param>
        ///// <returns></returns>
        //private StatusEnum GetStatusEnumByDescription(string strDescription)
        //{
        //    StatusEnum orderTypeEnum = StatusEnum.Waiting;//给个默认值
        //    var memberInfos = typeof(StatusEnum).GetMembers().Where(x => x.DeclaringType.ToString().Contains("StatusEnum"));
        //    foreach (var member in memberInfos)
        //    {
        //        var attrs = member.GetCustomAttributes();
        //        if (attrs != null && attrs.Count() > 0)
        //        {
        //            DescriptionAttribute attr = (DescriptionAttribute)(member.GetCustomAttribute(typeof(DescriptionAttribute), false));
        //            if (attr.Description.Contains(strDescription))
        //            {
        //                orderTypeEnum = (StatusEnum)Enum.Parse(typeof(StatusEnum), member.Name);
        //                break;
        //            }
        //        }
        //    }
        //    return orderTypeEnum;
        //}
        #region property

        //查询条件
        private string s_WH_Code;
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string S_WH_Code
        {
            set
            {
                s_WH_Code = value;
                RaisePropertyChanged("S_WH_Code");
            }
            get { return s_WH_Code; }
        }

        /// <summary>
        /// 处理状态
        /// </summary>
        private string s_HandleStatu;
        public string S_HandleStatu
        {
            set
            {
                s_HandleStatu = value;
                RaisePropertyChanged("S_HandleStatu");
            }
            get { return s_HandleStatu; }
        }
        private string s_MethodName;
        /// <summary>
        /// 方法名称
        /// </summary>
        public string S_MethodName
        {
            set
            {
                s_MethodName = value;
                RaisePropertyChanged("S_MethodName");
            }
            get { return s_MethodName; }
        }

        private string s_InputDataParms;
        /// <summary>
        /// 输入参数
        /// </summary>
        public string S_InputDataParms
        {
            set
            {
                s_InputDataParms = value;
                RaisePropertyChanged("S_InputDataParms");
            }
            get { return s_InputDataParms; }
        }


        private string s_ReceviveStartTime;
        /// <summary>
        /// 接收 开始时间
        /// </summary>
        public string S_ReceviveStartTime
        {
            set
            {
                s_ReceviveStartTime = value;
                RaisePropertyChanged("S_ReceviveStartTime");
            }
            get { return s_ReceviveStartTime; }
        }

        private string s_ReceviveEndTime;
        /// <summary>
        /// 接收 结束时间
        /// </summary>
        public string S_ReceviveEndTime
        {
            set
            {
                s_ReceviveEndTime = value;
                RaisePropertyChanged("S_ReceviveEndTime");
            }
            get { return s_ReceviveEndTime; }
        }

        private string s_HandleStartTime;
        /// <summary>
        /// 处理 开始时间
        /// </summary>
        public string S_HandleStartTime
        {
            set
            {
                s_HandleStartTime = value;
                RaisePropertyChanged("S_HandleStartTime");
            }
            get { return s_HandleStartTime; }
        }

        private string s_HandleEndTime;
        /// <summary>
        /// 处理 结束时间
        /// </summary>
        public string S_HandleEndTime
        {
            set
            {
                s_HandleEndTime = value;
                RaisePropertyChanged("S_HandleEndTime");
            }
            get { return s_HandleEndTime; }
        }


        private List<string> handleStatuLst = new List<string>();
        /// <summary>
        /// 处理状态列表
        /// </summary>
        public List<string> HandleStatuLst
        {
            get
            {
                if (handleStatuLst.Count == 0)
                {

                    //string[] nameArrList = System.Enum.GetNames(typeof(StatusEnum));
                    handleStatuLst.Add("全部");
                    //foreach (string strName in nameArrList)
                    //{
                    //    Enum em = (StatusEnum)Enum.Parse(typeof(StatusEnum), strName);
                    //    handleStatuLst.Add(GetDescription(em));
                    //}
                }
                return handleStatuLst;
            }

        }
        #endregion

        private List<DataList> allDataLst = new List<DataList>();
        /// <summary>
        /// 所有数据
        /// </summary>
        public List<DataList> AllDataList
        {
            set
            {
                allDataLst = value;
                RaisePropertyChanged("TotalSize");
            }
            get { return allDataLst; }
        }


        private List<DataList> currentShowlist = new List<DataList>();
        /// <summary>
        /// 当前显示的数据
        /// </summary>
        public List<DataList> CurrentShowList
        {
            get { return currentShowlist; }
            set
            {
                currentShowlist = value;
                RaisePropertyChanged("CurrentShowList");
            }

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
        private RelayCommand startOrPauseCommand;
        /// <summary>
        /// 开始/暂停
        /// </summary>
        public RelayCommand StartOrPauseCommand
        {
            get
            {
                if (startOrPauseCommand == null)
                {
                    startOrPauseCommand = new RelayCommand(DataMonitorStartOrPause);
                }
                return startOrPauseCommand;
            }
        }
        /// <summary>
        /// 开始和暂停功能
        /// </summary>
        private void DataMonitorStartOrPause()
        {

            if (ServiceMonitor == null) return;
            try
            {
                if (ServiceMonitor.CurRunState != RunStateMode.Run)
                {
                    if (!string.IsNullOrEmpty(S_MonitorState))
                    {
                        ServiceMonitor.MonitorInterval = int.Parse(S_MonitorInterval);
                    }

                    ServiceMonitor.Start();
                    S_BtnStartOrPauseContent = "暂停";
                    RaisePropertyChanged(S_MonitorState);
                }
                else
                {
                    ServiceMonitor.Pause();
                    S_BtnStartOrPauseContent = "开始";
                    RaisePropertyChanged(S_MonitorState);
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region Event
        public void OnInvalidPage()
        {
            if (InvalidPage != null)
            {
                InvalidPage();
            }
        }
        #endregion

        /// <summary>
        /// 查询数据
        /// </summary>
        private async void SwitchToSearchData()
        {
            await SwitchToCurDataPage();
            InitDataList();
        }
        /// <summary>
        /// 当前数据
        /// </summary>
        private Task SwitchToCurDataPage()
        {
            var result = Task.Run(() =>
            {
                DataTable dt = this.ServiceMonitor.GetDataByParms(S_HandleStatu, S_MethodName, S_InputDataParms, S_ReceviveStartTime, S_ReceviveEndTime, S_HandleStartTime, S_HandleEndTime);
                List<DataList> tempAllDataLst = new List<DataList>();
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataList dataLst = new DataList
                        {
                            WH_Code = dr["WH_Code"].ToString(),
                            DHSTATUS_ID = dr["DHSTATUS_ID"].ToString(),
                            RD_METHODNAME = dr["RD_METHODNAME"].ToString(),
                            RD_HANDLERDATE = dr["RD_HANDLERDATE"].ToString(),
                            RD_PARAMVALUE = dr["RD_PARAMVALUE"].ToString(),
                            RD_RECEIVEDATE = dr["RD_RECEIVEDATE"].ToString(),
                            RD_NOTE = dr["RD_NOTE"].ToString(),
                        };
                        tempAllDataLst.Add(dataLst);
                    }
                }
                AllDataList = tempAllDataLst;

            });
            return result;
        }




        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}
