using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel
{
    public class DeviceBasicPropertyViewModel:ViewModelBase
    {
        public DeviceBasicPropertyViewModel(DeviceBasicProperty dataModel)
        {
            DataModel = dataModel;
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
        public DeviceBasicProperty DataModel { get; set; }

    }
}
