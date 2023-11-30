using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLAT.License;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.License.DataModel;
using CLDC.CLWS.CLWCS.Service.License.View;
using CLDC.CLWS.CLWCS.Service.License.ViewModel;
using CLDC.Infrastructrue.UserCtrl;

namespace CLDC.CLWS.CLWCS.Service.License
{
    /// <summary>
    /// 注册服务
    /// </summary>
    public class LicenseService : ILicenseService
    {

        public LicenseService()
        {
            _deviceCode = ComputerAttributes.GetIdentifyCode();
            _filePath = GetFilePath();
            OperateResult<SerialNumberDataModel> getLicenseResult = GetLicense();
            if (getLicenseResult.IsSuccess)
            {
                _curLicense = getLicenseResult.Content;
            }
        }

        private string GetFilePath()
        {
            string strAppPath = Directory.GetCurrentDirectory();
            string licensePath = Path.Combine(strAppPath, @"SerialFile\SerialNumber.ini");
            return licensePath;
        }

        private string _deviceCode;

        private string _privateKey = "SZCLOU.PRODUCT.WCS.LICENSE";
        private string _filePath;

        public string PrivateKey
        {
            get { return _privateKey; }
            set { _privateKey = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        private SerialNumberDataModel _curLicense;

        private OperateResult UpdateSerialNumToText(SerialNumberDataModel serialNum)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<string> createPlainTextResult = CreatePlainText(serialNum);
                if (!createPlainTextResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "根据当前的注册信息，创建明文失败";
                    return result;
                }
                OperateResult<string> encryptTextResult = EncryptStringAes(createPlainTextResult.Content);
                if (!encryptTextResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "加密明文失败";
                    return result;
                }
                WriteSeriaNumber(encryptTextResult.Content);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private OperateResult<string> EncryptStringAes(string plainText)
        {
            string encryptAesValue = string.Empty;
            OperateResult<string> result = OperateResult.CreateFailedResult<string>(encryptAesValue);
            try
            {
                encryptAesValue = LicenseManager.EncryptStringAes(plainText, _privateKey);
                result.Content = encryptAesValue;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private OperateResult<string> CreatePlainText(SerialNumberDataModel serialNum)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                string plainText = string.Join("|", new string[] { _deviceCode, serialNum.ExpiryDate.ToString(CultureInfo.InvariantCulture), serialNum.ForecastDays.ToString(), "0", DateTime.Now.ToString(CultureInfo.InvariantCulture) });
                result.IsSuccess = true;
                result.Content = plainText;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        private OperateResult<string> DecryptStringFromAes(string cipherText)
        {
            string decryptAesValue = string.Empty;
            OperateResult<string> result = OperateResult.CreateFailedResult<string>(decryptAesValue);
            try
            {
                decryptAesValue = LicenseManager.DecryptStringFromBytesAes(cipherText, _privateKey);
                result.Content = decryptAesValue;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        private void WriteSeriaNumber(string serialNumber)
        {
            LicenseManager.WriteSeriaNumber(EmStorageType.File, _filePath, serialNumber);
        }

        private string ReadSeriaNumber(string path)
        {
            return LicenseManager.ReadSeriaNumber(EmStorageType.File, path);
        }


        public OperateResult<SerialNumberDataModel> GetLicense()
        {
            OperateResult<SerialNumberDataModel> serialNumberResult = new OperateResult<SerialNumberDataModel>();
            try
            {
                string seriaNumber = ReadSeriaNumber(_filePath);
                OperateResult<string> decryptSerialNumResult = DecryptStringFromAes(seriaNumber);
                if (!decryptSerialNumResult.IsSuccess)
                {
                    serialNumberResult.IsSuccess = false;
                    return serialNumberResult;
                }
                string decryptSeralNum = decryptSerialNumResult.Content;
                string[] decryptInfo = decryptSeralNum.Split('|');
                if (decryptInfo.Length == 5)
                {
                    SerialNumberDataModel serialInfo = new SerialNumberDataModel();
                    serialInfo.IdentificationCode = decryptInfo[0];
                    serialInfo.ExpiryDate = ConvertHepler.ConvertToDateTime(decryptInfo[1]);
                    serialInfo.ForecastDays = ConvertHepler.ConvertToInt(decryptInfo[2]);
                    serialInfo.FrontDate = ConvertHepler.ConvertToDateTime(decryptInfo[4]);
                    serialNumberResult.IsSuccess = true;
                    serialNumberResult.Content = serialInfo;
                }
                else if (decryptInfo.Length == 4)
                {
                    SerialNumberDataModel serialInfo = new SerialNumberDataModel();
                    serialInfo.IdentificationCode = decryptInfo[0];
                    serialInfo.ExpiryDate = ConvertHepler.ConvertToDateTime(decryptInfo[1]);
                    serialInfo.ForecastDays = ConvertHepler.ConvertToInt(decryptInfo[2]);
                    serialInfo.FrontDate = DateTime.Now;
                    serialNumberResult.IsSuccess = true;
                    serialNumberResult.Content = serialInfo;
                }
                else
                {
                    serialNumberResult.IsSuccess = false;
                    serialNumberResult.Message = "序列号解析失败";
                }
                return serialNumberResult;
            }
            catch (Exception ex)
            {
                serialNumberResult.IsSuccess = false;
                serialNumberResult.Message = OperateResult.ConvertException(ex);
            }
            return serialNumberResult;
        }

        public OperateResult IsLicenseAvailable(bool isShowRegisterView)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (_curLicense == null)
            {
                _curLicense = new SerialNumberDataModel();
                _curLicense.IdentificationCode = _deviceCode;
                result.IsSuccess = false;
                result.Message = "获取不到注册信息，请联系管理员";
            }
            else
            {

                if (!_curLicense.IdentificationCode.Equals(_deviceCode))
                {
                    result.IsSuccess = false;
                    result.Message = "注册信息的机器识别码不一致，请联系管理员";
                }
                else
                {
                    DateTime now = DateTime.Now;

                    int compareNow = DateTime.Compare(now, _curLicense.FrontDate);
                    if (compareNow > 0)
                    {
                        int compareResult = DateTime.Compare(now, _curLicense.ExpiryDate);
                        if (compareResult > 0)
                        {
                            result.IsSuccess = false;
                            result.Message = "注册码已过期，请联系管理员";
                        }
                        else
                        {
                            result.IsSuccess = true;
                            SystemConfig.Instance.IsLicenseAvailable = true;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "系统记录时间大于电脑时间，请检查本机电脑日期是否正确设置或者重新注册";
                    }

                }
            }
            if (!isShowRegisterView)
            {
                SystemConfig.Instance.IsLicenseAvailable = result.IsSuccess;
                return result;
            }
            if (!result.IsSuccess)
            {
                MessageBoxEx.Show(string.Format("系统注册码失效！\r\n失效原因：{0}\r\n请重新注册!", result.Message));
                OperateResult registerResult = ShowAndRegisterLicense();
                result.IsSuccess = registerResult.IsSuccess;
                SystemConfig.Instance.IsLicenseAvailable = registerResult.IsSuccess;
            }
            return result;
        }


        public OperateResult<int> IsNeedNoticeExpired()
        {
            OperateResult<int> result = new OperateResult<int>();
            if (_curLicense == null)
            {
                result.IsSuccess = true;
                result.Message = "获取不到注册信息";
                result.Content = -1;
                return result;
            }
            DateTime now = DateTime.Now.Date;
            DateTime expiryDate = _curLicense.ExpiryDate.Date;

            TimeSpan span = expiryDate.Subtract(now);

            int days = span.Days;
            if (days <= _curLicense.ForecastDays)
            {
                result.IsSuccess = true;
                result.Content = days;
                return result;
            }
            result.IsSuccess = false;
            result.Content = days;
            return result;

        }

        public OperateResult StartCheckLicense()
        {
            Task.Factory.StartNew(CheckLicense, TaskCreationOptions.LongRunning);
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult ShowAndRegisterLicense()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                RegisterLicenseViewModel viewModel = new RegisterLicenseViewModel(_curLicense);
                RegisterLicenseView registerView = new RegisterLicenseView { DataContext = viewModel };
                bool? registerResult = registerView.ShowDialog();
                if (registerResult.HasValue)
                {
                    if (registerResult.Value)
                    {
                        result.IsSuccess = true;
                        result.Message = "更改注册码成功";
                        IsLicenseAvailable(false);
                        return result;
                    }
                }
                result.IsSuccess = false;
                result.Message = "更改注册码失败";
                return result;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public OperateResult CheckSerialNumIsAvailable(string serialNum)
        {
            OperateResult result = new OperateResult();

            string seriaNumber = serialNum;
            if (string.IsNullOrEmpty(seriaNumber))
            {
                result.IsSuccess = false;
                result.Message = "序列号不能为空";
                return result;
            }
            try
            {
                if (seriaNumber.Length < 8)
                {
                    result.IsSuccess = false;
                    result.Message = "非法序列号";
                    return result;
                }
                if (seriaNumber.ElementAt(8) != 'C'
                   || seriaNumber.ElementAt(16) != 'L'
                   || seriaNumber.ElementAt(24) != 'O'
                   || seriaNumber.ElementAt(32) != 'U')
                {
                    result.IsSuccess = false;
                    result.Message = "非法序列号";
                    return result;
                }
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult UpdateLicense(string serialNum)
        {
            OperateResult result = new OperateResult();
            try
            {

                OperateResult checkResult = CheckSerialNumIsAvailable(serialNum);
                if (!checkResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = checkResult.Message;
                    return result;
                }

                serialNum = serialNum.Remove(40, 1);
                serialNum = serialNum.Remove(32, 1);
                serialNum = serialNum.Remove(24, 1);
                serialNum = serialNum.Remove(16, 1);
                serialNum = serialNum.Remove(8, 1);

                WriteSeriaNumber(serialNum);

                OperateResult<SerialNumberDataModel> getLicenseResult = GetLicense();
                if (getLicenseResult.IsSuccess)
                {
                    _curLicense.IdentificationCode = getLicenseResult.Content.IdentificationCode;
                    _curLicense.ExpiryDate = getLicenseResult.Content.ExpiryDate;
                    _curLicense.ForecastDays = getLicenseResult.Content.ForecastDays;
                    _curLicense.FrontDate=DateTime.Now;
                    result.IsSuccess = true;
                    UpdateSerialNumToText(_curLicense);
                    result.Message = "更新注册码成功";
                    return result;
                }
                result.IsSuccess = false;
                result.Message = "更新注册码失败";
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;

        }

        private readonly int _checkInterval = 1000 * 60;
        private void CheckLicense()
        {
            while (!SystemConfig.Instance.isExitApp)
            {
                try
                {
                    Thread.Sleep(_checkInterval);

                    if (Application.Current != null && Application.Current.Dispatcher != null)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            OperateResult isLicenseAvailable = IsLicenseAvailable(true);

                            if (isLicenseAvailable.IsSuccess)
                            {
                                OperateResult<int> isNeedNoticeResult = IsNeedNoticeExpired();
                                if (isNeedNoticeResult.IsSuccess)
                                {
                                    string showMsg = string.Format("系统注册码即将在：{0} 天后过期\r\n注册码过期后系统将停止使用！请联系管理员及时更新系统注册码", isNeedNoticeResult.Content);
                                    MessageBoxEx.Show(showMsg);
                                }
                                UpdateSerialNumToText(_curLicense);
                            }
                        });
                }
                catch (Exception ex)
                {

                }
            }
        }





    }
}
