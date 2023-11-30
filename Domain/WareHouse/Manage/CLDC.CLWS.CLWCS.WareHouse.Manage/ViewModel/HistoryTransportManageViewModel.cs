using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel
{
    public class HistoryTransportManageViewModel : ViewModelBase, INotifyAttributeChange
    {
        private readonly TransportMsgDataAbstract _transportMsgDataAbstract;
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
        public HistoryTransportManageViewModel()
        {
            this._transportMsgDataAbstract = DependencyHelper.GetService<TransportMsgDataAbstract>();


            PageViewModel = new UcSplitPagerViewModel();
            PageViewModel.PageChange += InitOrderList;
            //加载历史数据
        }
        //private List<TransportMsgModel> transportOrderList = new List<TransportMsgModel>();
        //public List<TransportMsgModel> TransportOrderList
        //{
        //    set
        //    {
        //        transportOrderList = value;
        //        RaisePropertyChanged("TotalSize");
        //    }
        //    get { return transportOrderList; }
        //}


        private List<TransportMsgModel> _currentTransportShowlist = new List<TransportMsgModel>();
        public List<TransportMsgModel> CurrentShowTransportList
        {
            set
            {
                _currentTransportShowlist = value;
                RaisePropertyChanged("CurrentShowTransportList");
            }
            get { return _currentTransportShowlist; }
        }

        /// <summary>
        /// 初始化 计算总页数
        /// </summary>
        private void InitOrderList()
        {
            var where = GetSql(S_TransportId, S_StartId, S_DestId, S_ExOrderId,
                        S_StartAddr, S_CurAddr, S_DestAddr, S_OwnerName, S_TransportStatus,
                        S_AddStartTime, S_AddEndTime, S_UpdateStartDateTime, S_UpdateEndDateTime);
            var data = _transportMsgDataAbstract.GetTransportData((int)PageViewModel.PageIndex, (int)PageViewModel.PageSize,"ADDDATETIME DESC", where,out int totalCount);
            if (data.IsSuccess)
            {
                this.CurrentShowTransportList = data.Content;
            }
            else
            {
                this.CurrentShowTransportList.Clear();
            }
            PageViewModel.TotalCount = totalCount;

            //PageViewModel.TotalCount = 0;
            //CurrentShowTransportList = new List<TransportMsgModel>();
            //if (TransportOrderList == null || TransportOrderList.Count == 0)
            //{
            //    PageViewModel.TotalPageSize = 0;
            //    PageViewModel.TotalCount = 0;
            //    PageViewModel.CurrentPage = 0;
            //    return;
            //}
            //if (TransportOrderList.Count % PageViewModel.PageSize == 0)
            //{
            //    PageViewModel.TotalPageSize = TransportOrderList.Count / PageViewModel.PageSize;
            //}
            //else
            //{
            //    PageViewModel.TotalPageSize = TransportOrderList.Count / PageViewModel.PageSize + 1;
            //}
            //if (TransportOrderList.Count > PageViewModel.PageSize)
            //{
            //    if (TransportOrderList == null) return;
            //    var query = from order in TransportOrderList select order;
            //    var collection = query.Skip((int)(PageViewModel.PageIndex * PageViewModel.PageSize)).Take((int)PageViewModel.PageSize);
            //    CurrentShowTransportList = collection.ToList();
            //}
            //else
            //{
            //    CurrentShowTransportList = TransportOrderList;
            //}
            //PageViewModel.TotalCount = TransportOrderList.Count;
        }



        /// <summary>
        ///  TransportResultEnum 通过描述 找到 TransportResultEnum对象
        /// </summary>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        private TransportResultEnum GetStatusEnumByDescription(string strDescription)
        {
            TransportResultEnum transportResultEnum = TransportResultEnum.Wait;//给个默认值
            var memberInfos = typeof(TransportResultEnum).GetMembers().Where(x => x.DeclaringType.ToString().Contains("TransportResultEnum"));
            foreach (var member in memberInfos)
            {
                var attrs = member.GetCustomAttributes();
                if (attrs != null && attrs.Count() > 0)
                {
                    DescriptionAttribute attr = (DescriptionAttribute)(member.GetCustomAttribute(typeof(DescriptionAttribute), false));
                    if (attr.Description.Contains(strDescription))
                    {
                        transportResultEnum = (TransportResultEnum)Enum.Parse(typeof(TransportResultEnum), member.Name);
                        break;
                    }
                }
            }
            return transportResultEnum;
        }
        public Expression<Func<TransportMsgModel, bool>> GetSql(string S_TransportId, string S_StartId, string S_DestId, string S_ExOrderId,
                               string S_StartAddr, string S_CurAddr, string S_DestAddr,
                                string S_OwnerName, string S_TransportStatus,
                                string S_AddStartTime, string S_AddEndTime, string S_UpdateStartDateTime, string S_UpdateEndDateTime)
        {
            Expression<Func<TransportMsgModel, bool>> whereLambda = t => t.WhCode == SystemConfig.Instance.WhCode;
            //string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' ", SystemConfig.Instance.WhCode);
            if (!string.IsNullOrEmpty(S_TransportId))
            {
                whereLambda = whereLambda.AndAlso(t => t.TransportId == Convert.ToInt32(S_TransportId));
                //sqlstr += " and TRANSPORTID like'" + "%" + S_TransportId + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_StartId))
            {
                whereLambda = whereLambda.AndAlso(t => t.StartId == Convert.ToInt32(S_StartId));
                //sqlstr += " and STARTID like'" + "%" + S_StartId + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_DestId))
            {
                whereLambda = whereLambda.AndAlso(t => t.DestId == Convert.ToInt32(S_DestId));
                //sqlstr += " and DESTID like'" + "%" + S_DestId + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_ExOrderId))
            {
                whereLambda = whereLambda.AndAlso(t => t.ExOrderId == Convert.ToInt32(S_ExOrderId));
                //sqlstr += " and EXORDERID like'" + "%" + S_ExOrderId + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_StartAddr))
            {
                whereLambda = whereLambda.AndAlso(t => t.StartAddr.Contains(S_StartAddr));
                //sqlstr += " and STARTADDR like'" + "%" + S_StartAddr + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_CurAddr))
            {
                whereLambda = whereLambda.AndAlso(t => t.CurAddr.Contains(S_CurAddr));
                //sqlstr += " and CURADDR like'" + "%" + S_CurAddr + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_DestAddr))
            {
                whereLambda = whereLambda.AndAlso(t => t.DestAddr.Contains(S_DestAddr));
                //sqlstr += " and DESTADDR like'" + "%" + S_DestAddr + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_OwnerName))
            {
                int.TryParse(S_OwnerName, out int ownerId);
                whereLambda = whereLambda.AndAlso(t => t.OwnerId== ownerId);
                //sqlstr += " and OWNERNAME like'" + "%" + S_OwnerName + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_TransportStatus))
            {
                TransportResultEnum transResultEnum = GetStatusEnumByDescription(S_TransportStatus);
                whereLambda = whereLambda.AndAlso(t => t.TransportStatus == transResultEnum);
                //sqlstr += " and STATUS like'" + "%" + (int)transResultEnum + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_AddStartTime) && !string.IsNullOrEmpty(S_AddEndTime))
            {
                DateTime startDate = Convert.ToDateTime(S_AddStartTime);
                DateTime endDate = Convert.ToDateTime(S_AddEndTime);
                //sqlstr += string.Format(" and ADDDATETIME between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);//oracle
                //sqlstr += string.Format(" and ADDDATETIME between '{0}' and  '{1}'", startDate, endDate);

                whereLambda = whereLambda.AndAlso(t => t.AddDateTime>=startDate && t.AddDateTime<=endDate);
            }

            if (!string.IsNullOrEmpty(S_UpdateStartDateTime) && !string.IsNullOrEmpty(S_UpdateEndDateTime))
            {
                DateTime startDate = Convert.ToDateTime(S_UpdateStartDateTime);
                DateTime endDate = Convert.ToDateTime(S_UpdateEndDateTime);
                //sqlstr += string.Format(" and UPDATEDATETIEM between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);//oracle
                //sqlstr += string.Format(" and UPDATEDATETIEM between  '{0}'  and  '{1}'", startDate, endDate);
                whereLambda = whereLambda.AndAlso(t => t.UpdateDateTime >= startDate && t.AddDateTime <= endDate);
            }
            return whereLambda;

        }


        ///// <summary>
        /////  加载历史数据
        ///// </summary>
        //private Task  LoadData()
        //{
        //    var result = Task.Run(() =>
        //    {
        //        try
        //        {

                    
        //        }
        //        catch (Exception ex)
        //        {
        //            //读取数据库异常
        //        }
        //    });
        //    return result;

        //}


        #region property

        //查询条件
        private string s_TransportId;
        /// <summary>
        /// 搬运信息 ID  
        /// </summary>
        public string S_TransportId
        {
            set
            {
                s_TransportId = value;
                RaisePropertyChanged("S_TransportId");
            }
            get { return s_TransportId; }
        }

        /// <summary>
        /// 开始设备 ID  
        /// </summary>
        private string s_StartId;
        public string S_StartId
        {
            get { return s_StartId; }
            set
            {
                s_StartId = value;
                RaisePropertyChanged("S_StartId");
            }
        }

        /// <summary>
        /// 目标设备 ID  
        /// </summary>
        private string s_DestId;
        public string S_DestId
        {
            get { return s_DestId; }
            set
            {
                s_DestId = value;
                RaisePropertyChanged("S_DestId");
            }
        }

        /// <summary>
        /// 搬运指令 ID  
        /// </summary>
        private string s_ExOrderId;
        public string S_ExOrderId
        {
            get { return s_ExOrderId; }
            set
            {
                s_ExOrderId = value;
                RaisePropertyChanged("S_ExOrderId");
            }
        }
        private string s_StartAddr;
        /// <summary>
        /// 开始地址
        /// </summary>
        public string S_StartAddr
        {
            set
            {
                s_StartAddr = value;
                RaisePropertyChanged("S_StartAddr");
            }
            get { return s_StartAddr; }
        }


        private string s_CurAddr;
        /// <summary>
        /// 当前地址
        /// </summary>
        public string S_CurAddr
        {
            set
            {
                s_CurAddr = value;
                RaisePropertyChanged("S_CurAddr");
            }
            get { return s_CurAddr; }
        }

        private string s_DestAddr;
        /// <summary>
        /// 目标地址
        /// </summary>
        public string S_DestAddr
        {
            set
            {
                s_DestAddr = value;
                RaisePropertyChanged("S_DestAddr");
            }
            get { return s_DestAddr; }
        }

        private string s_OwnerName;
        /// <summary>
        /// 拥有者名称
        /// </summary>
        public string S_OwnerName
        {
            set
            {
                s_OwnerName = value;
                RaisePropertyChanged("S_OwnerName");
            }
            get { return s_OwnerName; }
        }

        private string s_TransportStatus;
        /// <summary>
        /// 结果状态
        /// </summary>
        public string S_TransportStatus
        {
            set
            {
                s_TransportStatus = value;
                RaisePropertyChanged("S_TransportStatus");
            }
            get { return s_TransportStatus; }
        }

        private string s_AddStartTime;
        /// <summary>
        /// 添加 开始时间
        /// </summary>
        public string S_AddStartTime
        {
            set
            {
                s_AddStartTime = value;
                RaisePropertyChanged("S_AddStartTime");
            }
            get { return s_AddStartTime; }
        }
        private string s_AddEndTime;
        /// <summary>
        /// 添加 结束时间
        /// </summary>
        public string S_AddEndTime
        {
            set
            {
                s_AddEndTime = value;
                RaisePropertyChanged("S_AddEndTime");
            }
            get { return s_AddEndTime; }
        }

        private string s_UpdateStartDateTime;
        /// <summary>
        /// 完成 开始时间
        /// </summary>
        public string S_UpdateStartDateTime
        {
            set
            {
                s_UpdateStartDateTime = value;
                RaisePropertyChanged("S_UpdateStartDateTime");
            }
            get { return s_UpdateStartDateTime; }
        }
        private string s_UpdateEndDateTime;
        /// <summary>
        /// 完成 结束时间
        /// </summary>
        public string S_UpdateEndDateTime
        {
            set
            {
                s_UpdateEndDateTime = value;
                RaisePropertyChanged("S_UpdateEndDateTime");
            }
            get { return s_UpdateEndDateTime; }
        }

        private List<string> transportResultStatuLst = new List<string>();
        /// <summary>
        /// 搬运指令 结果列表
        /// </summary>
        public List<string> TransportResultStatuLst
        {
            get
            {
                if (transportResultStatuLst.Count == 0)
                {
                    string[] nameArrList = System.Enum.GetNames(typeof(TransportResultEnum));
                    foreach (string strName in nameArrList)
                    {
                        Enum em = (TransportResultEnum)Enum.Parse(typeof(TransportResultEnum), strName);
                        transportResultStatuLst.Add(GetDescription(em));
                    }
                }
                return transportResultStatuLst;
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



        private RelayCommand searchTransportCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand SearchTransportCommand
        {
            get
            {
                if (searchTransportCommand == null)
                {
                    searchTransportCommand = new RelayCommand(SwitchToSearchData);
                }
                return searchTransportCommand;
            }
        }

        #endregion property





        /// <summary>
        /// 查询数据
        /// </summary>
        private async void SwitchToSearchData()
        {
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(S_TransportId) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_StartId) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_DestId) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_ExOrderId) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_StartAddr) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_CurAddr) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_DestAddr) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(S_OwnerName))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }
            //await LoadData();
            InitOrderList();
        }


        public void NotifyAttributeChange(string attributeName, object newValue)
        {

        }
    }
}
