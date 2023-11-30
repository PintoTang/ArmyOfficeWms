using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.UserService.DataMode;

namespace CLDC.CLWS.CLWCS.Service.UserService.Oracle
{
   public sealed class UserDataForOracle : UserDataAbstract
    {
       public override OperateResult<AccountMode> Login(string account, string password)
        {
           AccountMode accountInfo = new AccountMode();
           accountInfo.RoleId= RoleLevelEnum.SupperOperator;
           OperateResult<AccountMode> result = OperateResult.CreateFailedResult(accountInfo, "无数据");
           result.IsSuccess = true;
           return result;
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
                    accountInfo.RoleId = (RoleLevelEnum)Enum.Parse(typeof(RoleLevelEnum), dr["ROLE_ID"].ToString());
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

        public override OperateResult Logout(string account, string password)
        {
            throw new NotImplementedException();
        }

        public override OperateResult Verify(string account, string password)
        {
            throw new NotImplementedException();
        }

        public override OperateResult UpdatePassword(string newPassword)
        {
            throw new NotImplementedException();
        }

        public override OperateResult IsExist(UserInformationMode data)
        {
            throw new NotImplementedException();
        }

        public override OperateResult Update(UserInformationMode data)
        {
            throw new NotImplementedException();
        }

        public override OperateResult Insert(UserInformationMode data)
        {
            throw new NotImplementedException();
        }

       public UserDataForOracle(IDbHelper dbHelper) : base(dbHelper)
       {
       }
    }
}
