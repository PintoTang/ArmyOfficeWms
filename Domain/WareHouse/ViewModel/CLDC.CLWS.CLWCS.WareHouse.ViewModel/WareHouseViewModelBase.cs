using System;
using System.Collections.ObjectModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Framework.Log.Helper;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public enum ViewModelTypeEnum
    {
        /// <summary>
        /// 设备
        /// </summary>
        Device,
        /// <summary>
        /// 工作者
        /// </summary>
        Woker
    }
    public abstract class WareHouseViewModelBase : ViewModelBase, INotifyAttributeChange, IManageable
    {
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
                    //logMsg.Level = MonitorTest();//模拟测试用 待删除

                    LogInfoModelLst.Add(logMsg);
                }), DispatcherPriority.Background, msg, level);
        }
      
        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession!=null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
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


        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }

        public ViewModelTypeEnum ViewModelType { get; set; }

        public string Name { get; set; }
        
        private SolidColorBrush _titleForeGroundColor;
        public SolidColorBrush TitleForeGroundColor
        {
            get
            {
                return _titleForeGroundColor;
            }
            set
            {
                if (_titleForeGroundColor != value)
                {
                    _titleForeGroundColor = value;
                    RaisePropertyChanged("TitleForeGroundColor");
                }
            }
        }

        private bool _isChangedColor = false;
        /// <summary>
        /// 是否开启定位工位的颜色提醒
        /// </summary>
        public bool IsChangedColor
        {
            get { return _isChangedColor; }
            set
            {
                _isChangedColor = value;
                this.RaisePropertyChanged("IsChangedColor");
            }
        }

        private MyCommand _cmdDeviceOpenLog;
        /// <summary>
        /// 打开当前日志
        /// </summary>
        public MyCommand CmdDeviceOpenLog
        {
            get
            {
                if (_cmdDeviceOpenLog == null)
                    _cmdDeviceOpenLog = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              LogManageHelper.Instance.OpenLogFile(this.Name);
                          }
                        ));
                return _cmdDeviceOpenLog;
            }
        }

        private ObservableCollection<UcStationLogInfoModel> _logInfoModelLst = new ObservableCollection<UcStationLogInfoModel>();
        public ObservableCollection<UcStationLogInfoModel> LogInfoModelLst
        {
            get { return _logInfoModelLst; }
            set
            {
                if (_logInfoModelLst != value)
                {
                    _logInfoModelLst = value;
                    RaisePropertyChanged("LogInfoModelLst");
                }
            }
        }
        public abstract void NotifyAttributeChange(string attributeName, object newValue);

    }
}
