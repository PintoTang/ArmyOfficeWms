using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    public abstract class UserDataAbstract : DatabaseBusinessAbstract<UserInformationMode>
    {
        protected UserDataAbstract(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }


        /// <summary>
        /// 通过组编号获取组信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public abstract OperateResult<GroupMode> GetGroupInfoById(int groupId);
        /// <summary>
        /// 通过用户名查找账号信息
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public abstract OperateResult<AccountMode> GetAccountInfoById(string accountId);
        /// <summary>
        /// 通过用户名查找人物信息
        /// </summary>
        /// <param name="accCode"></param>
        /// <returns></returns>
        public abstract OperateResult<PersonMode> GetPersonInfoByAccCode(string accCode);

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public OperateResult<UserInformationMode> Login(string account, string password)
        {
            UserInformationMode userInfo=new UserInformationMode();
            OperateResult<UserInformationMode> loginResult = OperateResult.CreateFailedResult(userInfo, "无数据");
            OperateResult<AccountMode> getAccountInfo = GetAccountInfoById(account);
            if (!getAccountInfo.IsSuccess)
            {
                string msg = string.Format("登陆失败，原因：{0}",getAccountInfo.Message);
                loginResult.IsSuccess = false;
                loginResult.Message = msg;
                return loginResult;
            }
            if (!getAccountInfo.Content.Password.Equals(password))
            {
                string msg ="登陆失败，密码不正确";
                loginResult.IsSuccess = false;
                loginResult.Message = msg;
                return loginResult;
            }
            if (getAccountInfo.Content.UseStatus.Equals(AccountStatusEnum.禁用))
            {
                loginResult.Message = "账号已停用";
                loginResult.IsSuccess = false;
                return loginResult;
            }
            loginResult.Content.Account = getAccountInfo.Content;
            loginResult.Message = "登陆成功";
            loginResult.IsSuccess = true;
            OperateResult<GroupMode> getGroupInfo = GetGroupInfoById(getAccountInfo.Content.GroupId.GetValueOrDefault());
            if (getGroupInfo.IsSuccess)
            {
                loginResult.Content.Group = getGroupInfo.Content;
            }
            OperateResult<PersonMode> getPersonInfo = GetPersonInfoByAccCode(getAccountInfo.Content.AccCode);
            if (getPersonInfo.IsSuccess)
            {
                loginResult.Content.Person = getPersonInfo.Content;
            }
            loginResult.Content.Person.AccCode = getAccountInfo.Content.AccCode;
            loginResult.Content.Person.GroupId = getAccountInfo.Content.GroupId;
            return loginResult;
        }
    
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract OperateResult Logout(string account, string password);

        /// <summary>
        /// 验证用户名和密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract OperateResult Verify(string account, string password);

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public abstract OperateResult UpdatePassword(string account,string newPassword);

        public OperateResult ChangePassword(string account, string oldPassword, string newPassword)
        {
            OperateResult result=OperateResult.CreateFailedResult();
            OperateResult verifyResult = Verify(account, oldPassword);
            if (!verifyResult.IsSuccess)
            {
                result.Message = verifyResult.Message;
                result.IsSuccess = false;
                return verifyResult;
            }
            OperateResult changeResult = UpdatePassword(account, newPassword);
            return changeResult;
        }

        /// <summary>
        /// 更新个人信息
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public abstract OperateResult UpdatePersonInfo(PersonMode person);

        public abstract OperateResult<List<AccountMode>> GetAccountList(Expression<Func<AccountMode, bool>> whereLambda = null);

        /// <summary>
        /// 创建新用户
        /// </summary>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        public abstract OperateResult CreateNewAccount(AccountMode newAccount);

        /// <summary>
        /// 获取所有部门信息
        /// </summary>
        /// <returns></returns>
        public abstract List<GroupMode> GetAllGroupList();

        /// <summary>
        /// 删除用户名
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public abstract OperateResult DeleteAccount(AccountMode account);

        public abstract OperateResult EditAccount(AccountMode account);

    }
}
