using System.Windows.Input;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Service.Authorize.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CLDC.CLWS.CLWCS.Service.Authorize.ViewModel
{
    public class AccountInfoViewModel : ViewModelBase
    {
        private Session _session;

        public AccountInfoViewModel(Session session)
        {
            _session = session;
        }

        /// <summary>
        /// 相片的资源
        /// </summary>
        public string ImageSource { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string AccountId {
            get { return _session.UserInfo.Account.AccCode; } }
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName {
            get { return _session.UserInfo.Group.GroupName; } }
        /// <summary>
        /// 组描述
        /// </summary>
        public string GroupDesc {
            get { return GroupName + "   " + _session.UserInfo.Group.Remark; } }

        /// <summary>
        /// 权限
        /// </summary>
        public string LevelName
        {
            get {return  _session.RoleLevel.GetDescription(); }
            
        }

        private ICommand _modifyCommand;
        public ICommand ModifyCommand
        {
            get
            {
                if (_modifyCommand == null)
                {
                    _modifyCommand = new RelayCommand<object>(Modify);
                }
                return _modifyCommand;
            }
        }


        private void Modify(object para)
        {
            UserInfoModifyView modifyView=new UserInfoModifyView();
            modifyView.ShowDialog();
        }

    }
}
