using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CLDC.CLWS.CLWCS.Service.Authorize.View;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.Service.Authorize.ViewModel
{
    public class AuthServiceViewModel : ViewModelBase
    {
        public AuthServiceViewModel()
        {
            ItemSelectedCommand = new RelayCommand<object>(OnItemSelected);

            MenuItems.Add(new MenuItem("账号信息", PackIconKind.Home, new AccountInfoView()));
            MenuItems.Add(new MenuItem("账号管理", PackIconKind.Magnify, new AccountManageView()));
            InitView();
        }

        private void InitView()
        {
            MenuItem firstOrDefault = MenuItems.FirstOrDefault();
            if (firstOrDefault != null) SelectedItem = firstOrDefault.UserView;
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                Set(ref _selectedItem, value);
            }
        }
        private void OnItemSelected(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
        public ICommand ItemSelectedCommand { get; set; }
        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> MenuItems { get { return _menuItems; } }
    }
}
