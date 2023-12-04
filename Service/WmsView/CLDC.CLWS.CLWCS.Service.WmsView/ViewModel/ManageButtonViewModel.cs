using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl.Domain;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System.Collections.ObjectModel;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class ManageButtonViewModel: ViewModelBase
    {
        private AuthorizeService _authorizeService;
        public ObservableCollection<AccountMode> AccountList { get; set; }


        public ManageButtonViewModel()
        {
            AccountList = new ObservableCollection<AccountMode>();
            _authorizeService = DependencyHelper.GetService<AuthorizeService>();
        }

        private RelayCommand _inOrderManageCommand;

        public RelayCommand InOrderManageCommand
        {
            get
            {
                if (_inOrderManageCommand == null)
                {
                    _inOrderManageCommand = new RelayCommand(InOrderManageList);
                }
                return _inOrderManageCommand;
            }
        }

        private async void InOrderManageList()
        {
            UcInOrderManage inOrderListManage = new UcInOrderManage();
            await MaterialDesignThemes.Wpf.DialogHost.Show(inOrderListManage, "DialogHostWait");        
        }



    }
}
