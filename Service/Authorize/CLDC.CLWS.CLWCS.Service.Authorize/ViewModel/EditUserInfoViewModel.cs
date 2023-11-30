using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Service.Authorize.ViewModel
{
    public class EditUserInfoViewModel : ViewModelBase
    {
        public UserInformationMode DataModel { get; set; }
        public EditUserInfoViewModel(UserInformationMode dataModel)
        {
            DataModel = dataModel;
        }

        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(SaveUserInfo);
                }
                return _saveCommand;
            }
        }

        /// <summary>
        /// 添加数据到
        /// </summary>
        private void SaveUserInfo()
        {
            try
            {

                MessageBoxResult rt = MessageBoxEx.Show("您确认保存？", "警告", MessageBoxButton.YesNo);
                if (rt.Equals(MessageBoxResult.Yes))
                {
                    AuthorizeService authorizeService = DependencyHelper.GetService<AuthorizeService>();

                   OperateResult updateResult= authorizeService.UpdatePersonInfo(DataModel.Person);
                   if (!updateResult.IsSuccess)
                   {
                       MessageBoxEx.Show("用户信息保存失败：" + updateResult.Message);
                       return;
                   }
                    SnackbarQueue.MessageQueue.Enqueue("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("用户信息保存失败:" + OperateResult.ConvertException(ex));
            }
        }


    }
}
