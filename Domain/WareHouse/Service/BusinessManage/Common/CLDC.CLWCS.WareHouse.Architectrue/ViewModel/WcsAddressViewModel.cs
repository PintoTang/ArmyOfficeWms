using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWCS.WareHouse.Architectrue.ViewModel
{
    public class WcsAddressViewModel : ViewModelBase
    {
        public WcsAddressViewModel(string serviceName)
        {
            DataViewModel = new WhAddressDataViewModel(serviceName);
        }
        public WhAddressDataViewModel DataViewModel { get; set; }
           /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand RefreshDataCommand
        {
            get
            {
                if (_refreshDataCommand == null)
                {
                    _refreshDataCommand = new RelayCommand(RefreshData);
                }
                return _refreshDataCommand;
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

        private void RefreshData()
        {
            IWmsWcsArchitecture architecture  = DependencyHelper.GetService<IWmsWcsArchitecture>();
            architecture.Refresh();
        }

        private RelayCommand _refreshDataCommand;
        

    }
}
