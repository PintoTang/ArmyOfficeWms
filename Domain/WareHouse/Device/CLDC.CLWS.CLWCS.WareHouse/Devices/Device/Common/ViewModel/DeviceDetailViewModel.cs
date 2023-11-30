using System;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel
{
    public class DeviceDetailViewModel
    {
        public DeviceDetailViewModel(DeviceBaseAbstract device)
        {
            Device = device;
            DetailView = device.GetDetailViewForWpf();
            MonitorView = device.GetMonitorViewForWpf();
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

        public DeviceBaseAbstract Device { get; set; }
        public UserControl DetailView { get; set; }
        public UserControl MonitorView { get; set; }

        private MyCommand _openLogCommand;
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
                              LogManageHelper.Instance.OpenLogFile(Device.Name);
                          }
                        ));
                return _openLogCommand;
            }
        }

    }
}
