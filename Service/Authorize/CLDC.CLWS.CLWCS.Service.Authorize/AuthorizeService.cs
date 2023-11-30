using System.Collections.Generic;
using System.Data;
using System.Linq;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.Security;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;

using System;
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    /// <summary>
    /// Session服务
    /// </summary>
    public class AuthorizeService
    {
        private  List<Session> _sessions = new List<Session>();

        private readonly UserDataAbstract _userDbAccess;
        public AuthorizeService(UserDataAbstract userDbAbstract)
        {
            _userDbAccess = userDbAbstract;
        }
        /// <summary>
        /// 当前的登陆信息
        /// </summary>
        public  List<Session> Sessions
        {
            get { return _sessions; }
            set { _sessions = value; }
        }

        public OperateResult<Session> Login(string account, string password)
        {
            Session newSession = new Session();
            OperateResult<Session> result = OperateResult.CreateFailedResult(newSession, "未登陆");
            string encryptPd = SecurityHelper.Encrypt(password);
            OperateResult<UserInformationMode> loginResult = _userDbAccess.Login(account, encryptPd);
            if (!loginResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = loginResult.Message;
                return result;
            }
            UserInformationMode userInfo = loginResult.Content;
            newSession.UserInfo = userInfo;
            _sessions.Add(newSession);
            result.Content = newSession;
            result.IsSuccess = true;
            result.Message = "登陆成功";
            return result;
        }

        public OperateResult Logout(Session session)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (_sessions.Exists(s => s.SessionId.Equals(session.SessionId)))
            {
                Session logoutSession = Sessions.First(e => e.SessionId.Equals(session.SessionId));
                bool remove = _sessions.Remove(logoutSession);
                if (!remove)
                {
                    return result;
                }
                else
                {
                    return OperateResult.CreateSuccessResult();
                }
            }
            else
            {
                string account = session.UserInfo.Account.AccCode;
                string password = session.UserInfo.Account.Password;
                OperateResult loginResult = _userDbAccess.Logout(account, password);
                if (!loginResult.IsSuccess)
                {
                    result.Message = loginResult.Message;
                    result.IsSuccess = false;
                    return result;
                }
                result.IsSuccess = true;
                result.Message = "注销成功";
                return result;
            }
        }

        public OperateResult UpdatePersonInfo(PersonMode person)
        {
            OperateResult updateResult = _userDbAccess.UpdatePersonInfo(person);
            return updateResult;
        }

        public OperateResult ChangePassword(string account, string oldPassword, string newPassword)
        {
            OperateResult changePasswordResult = _userDbAccess.ChangePassword(account, oldPassword, newPassword);
            return changePasswordResult;
        }

        public OperateResult<List<AccountMode>> GetAccountList(Expression<Func<AccountMode, bool>> whereLambda = null)
        {
            return _userDbAccess.GetAccountList(whereLambda);
        }

        public OperateResult CreateNewAccount(AccountMode newAccount)
        {
            return _userDbAccess.CreateNewAccount(newAccount);
        }

        public List<GroupMode> GetAllGroupList()
        {
            return _userDbAccess.GetAllGroupList();
        }

        public OperateResult DeleteAccount(AccountMode account)
        {
            return _userDbAccess.DeleteAccount(account);
        }

        public OperateResult EditAccount(AccountMode account)
        {
            return _userDbAccess.EditAccount(account);
        }

    }
}
