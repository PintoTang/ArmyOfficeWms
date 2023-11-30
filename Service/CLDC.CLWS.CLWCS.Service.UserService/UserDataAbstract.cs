using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.UserService.DataMode;

namespace CLDC.CLWS.CLWCS.Service.UserService
{
    public abstract class UserDataAbstract : DatabaseBusinessAbstract<UserInformationMode>
    {
        protected UserDataAbstract(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }
       /// <summary>
       /// 登陆
       /// </summary>
       /// <param name="account"></param>
       /// <param name="password"></param>
       /// <returns></returns>
        public abstract OperateResult<AccountMode> Login(string account, string password);
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
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public abstract OperateResult UpdatePassword(string newPassword);

        public OperateResult ChangePassword(string account, string oldPassword, string newPassword)
        {
            OperateResult result=OperateResult.CreateFailedResult();
            OperateResult verifyResult = Verify(account, oldPassword);
            if (!verifyResult.IsSuccess)
            {
                result.Message = verifyResult.Message;
                result.IsSuccess = false;
            }
            OperateResult changeResult = UpdatePassword(newPassword);
            return changeResult;
        }


    }
}
