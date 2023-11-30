using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.License.DataModel;
using CLDC.Infrastructrue.UserCtrl;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.Service.License.ViewModel
{
    public class RegisterLicenseViewModel : ViewModelBase
    {
        public SerialNumberDataModel DataModel { get; private set; }

        private ILicenseService _licenseService;

        public RegisterLicenseViewModel(SerialNumberDataModel licenseModel)
        {
            DataModel = licenseModel;
            _licenseService = DependencyHelper.GetService<ILicenseService>();
            SystemName = SystemConfig.Instance.SysName;
            Version = SystemConfig.Instance.Version;
            CopyRight = SystemConfig.Instance.CopyRight;
        }

        /// <summary>
        /// 注册码
        /// </summary>
        public string RegisterSerialNum
        {
            get { return _registerSerialNum; }
            set
            {
                IsAvailableSerialNum = false;
                _registerSerialNum = value;
                RaisePropertyChanged();
            }
        }

        public string SystemName { get; set; }


        public string Version { get; set; }

        public string CopyRight { get; set; }

        public bool IsRegisterSuccess { get; set; }

        public bool IsAvailableSerialNum
        {
            get { return _isAvailableSerialNum; }
            set
            {
                _isAvailableSerialNum = value;
                RaisePropertyChanged();
            }
        }


        private ICommand _updateLicenseCommand;
        public ICommand UpdateLicenseCommand
        {
            get
            {
                if (_updateLicenseCommand == null)
                {
                    _updateLicenseCommand = new RelayCommand(UpdateLicense);
                }
                return _updateLicenseCommand;
            }
        }

        private void UpdateLicense()
        {
            OperateResult updateResult = _licenseService.UpdateLicense(RegisterSerialNum);
            IsRegisterSuccess = updateResult.IsSuccess;
            MessageBoxEx.Show(updateResult.Message, "提示", MessageBoxButton.OK);
        }


        private ICommand _checkSerialNumCommand;
        private bool _isAvailableSerialNum;
        private string _registerSerialNum;

        public ICommand CheckSerialNumCommand
        {
            get
            {
                if (_checkSerialNumCommand == null)
                {
                    _checkSerialNumCommand = new RelayCommand(CheckSerialNum);
                }
                return _checkSerialNumCommand;
            }
        }


        private void CheckSerialNum()
        {
            if (string.IsNullOrEmpty(RegisterSerialNum))
            {
                MessageBoxEx.Show("请输入注册码");
                return;
            }
            OperateResult checkResult = _licenseService.CheckSerialNumIsAvailable(RegisterSerialNum);
            if (!checkResult.IsSuccess)
            {
                MessageBoxEx.Show(checkResult.Message, "提示", MessageBoxButton.OK);
                return;
            }
            MessageBoxEx.Show("合法注册码", "提示", MessageBoxButton.OK);
            IsAvailableSerialNum = true;
        }



    }
}
