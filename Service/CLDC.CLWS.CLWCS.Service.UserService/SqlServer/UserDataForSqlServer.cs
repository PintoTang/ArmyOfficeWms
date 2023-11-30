using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.UserService.DataMode;

namespace CLDC.CLWS.CLWCS.Service.UserService.SqlServer
{
    public sealed class UserDataForSqlServer : UserDataAbstract
    {
        /// <summary>
        /// 是否存在指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override OperateResult IsExist(UserInformationMode data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override OperateResult Update(UserInformationMode data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override OperateResult Insert(UserInformationMode data)
        {
            throw new NotImplementedException();
        }

        public UserDataForSqlServer(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override OperateResult<AccountMode> Login(string account, string password)
        {
            AccountMode accountInfo = new AccountMode();
            OperateResult<AccountMode> result = OperateResult.CreateFailedResult(accountInfo, "无数据");
            try
            {
                string accountIdSql = string.Format(@"SELECT *FROM T_ST_ACCOUNT WHERE ACC_CODE='{0}'", account);
                DataSet ds = dbHelper.GetDataSet(accountIdSql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    accountInfo.Password = dr["PASSWORD"].ToString();
                    if (!password.Equals(accountInfo.Password))
                    {
                        result.Message = "密码错误";
                        result.IsSuccess = false;
                        return result;
                    }
                    accountInfo.UseStatus = (AccountStatusEnum)Enum.Parse(typeof(AccountStatusEnum), dr["USE_STATUS"].ToString());
                    if (accountInfo.UseStatus.Equals(AccountStatusEnum.Disable))
                    {
                        result.Message = "账号已停用";
                        result.IsSuccess = false;
                        return result;
                    }
                    accountInfo.AccId = int.Parse(dr["ACC_ID"].ToString());
                    accountInfo.AccCode = dr["ACC_CODE"].ToString();
                    accountInfo.GroupId = int.Parse(dr["GROUP_ID"].ToString());
                    accountInfo.RoleId =(RoleLevelEnum) Enum.Parse(typeof(RoleLevelEnum),dr["ROLE_ID"].ToString());
                }
                else
                {
                    result.Message = string.Format("不存在用户名：{0}", account);
                    result.IsSuccess = false;
                    return result;
                }
                return OperateResult.CreateSuccessResult(accountInfo);
            }
            catch (Exception ex)
            {
               result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override OperateResult Logout(string account, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 验证用户名和密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override OperateResult Verify(string account, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public override OperateResult UpdatePassword(string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
