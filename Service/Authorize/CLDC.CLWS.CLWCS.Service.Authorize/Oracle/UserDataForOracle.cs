using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
   public sealed class UserDataForOracle : UserDataAbstract
    {
       public override OperateResult<GroupMode> GetGroupInfoById(int groupId)
       {
           GroupMode groupInfo = null;
           OperateResult<GroupMode> result = OperateResult.CreateFailedResult(groupInfo, "无数据");
           try
           {
                groupInfo = DbHelper.Query<GroupMode>(t => t.GroupId == groupId);
               //string accountIdSql = string.Format(@"SELECT *FROM T_ST_GROUP WHERE GROUP_ID='{0}'", groupId);
               //DataSet ds = DbHelper.GetDataSet(accountIdSql);
               //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
               //{
               //    DataRow dr = ds.Tables[0].Rows[0];
               //    groupInfo.GroupId = groupId;
               //    groupInfo.GroupName = dr["GROUP_NAME"].ToString();
               //    groupInfo.Remark = dr["REMARK"].ToString();
               //}
               //else
               if(groupInfo==null)
               {
                   result.Message = string.Format("不存在用户名：{0}", groupId);
                   result.IsSuccess = false;
                   return result;
               }
               return OperateResult.CreateSuccessResult(groupInfo);
           }
           catch (Exception ex)
           {
               result.Message = OperateResult.ConvertException(ex);
           }
           return result;
       }

       public override OperateResult<AccountMode> GetAccountInfoById(string accountId)
       {
           AccountMode accountInfo = new AccountMode();
           OperateResult<AccountMode> result = OperateResult.CreateFailedResult(accountInfo, "无数据");
           try
           {
               string accountIdSql = string.Format(@"SELECT *FROM T_ST_ACCOUNT WHERE ACC_CODE='{0}'", accountId);
               DataSet ds = DbHelper.GetDataSet(accountIdSql);
               if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
               {
                   DataRow dr = ds.Tables[0].Rows[0];
                   accountInfo.Password = dr["PASSWORD"].ToString();
                   accountInfo.UseStatus = (AccountStatusEnum)Enum.Parse(typeof(AccountStatusEnum), dr["USE_STATUS"].ToString());
                   accountInfo.AccId = int.Parse(dr["ACC_ID"].ToString());
                   accountInfo.AccCode = dr["ACC_CODE"].ToString();
                   accountInfo.GroupId = int.Parse(dr["GROUP_ID"].ToString());
                   accountInfo.RoleLevel = (RoleLevelEnum)Enum.Parse(typeof(RoleLevelEnum), dr["ROLE_ID"].ToString());
                   accountInfo.ModifierId = int.Parse(dr["MODIFIER"].ToString());
                   accountInfo.Remark = dr["REMARK"].ToString();
                   accountInfo.CreaterId = int.Parse(dr["CREATER"].ToString());
               }
               else
               {
                   result.Message = string.Format("不存在用户名：{0}", accountId);
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

       public override OperateResult<PersonMode> GetPersonInfoByAccCode(string accCode)
       {
           PersonMode personInfo = new PersonMode();
           OperateResult<PersonMode> result = OperateResult.CreateFailedResult(personInfo, "无数据");
           try
           {
               string accountIdSql = string.Format(@"SELECT *FROM T_ST_PERSON_INFO WHERE ACC_CODE='{0}'", accCode);
               DataSet ds = DbHelper.GetDataSet(accountIdSql);
               if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
               {
                   DataRow dr = ds.Tables[0].Rows[0];
                   personInfo.Description = dr["SKILLS"].ToString();
                   personInfo.Address = dr["ADDRESS"].ToString();
                   personInfo.WorkId = dr["WORK_ID"].ToString();
                   personInfo.PersonName = dr["PERSON_NAME"].ToString();
                   personInfo.Remark = dr["REMARK"].ToString();
                   personInfo.TelephoneNo = dr["TELEPHONE"].ToString();
                   personInfo.Email = dr["E_MAIL"].ToString();
                   personInfo.AccCode = accCode;
               }
               else
               {
                   result.Message = string.Format("不存在账号：{0} 的人物信息", accCode);
                   result.IsSuccess = false;
                   return result;
               }
               return OperateResult.CreateSuccessResult(personInfo);
           }
           catch (Exception ex)
           {
               result.Message = OperateResult.ConvertException(ex);
           }
           return result;
       }

       public new OperateResult<AccountMode> Login(string account, string password)
        {
           AccountMode accountInfo = new AccountMode();
           accountInfo.RoleLevel= RoleLevelEnum.超级运维人员;
           OperateResult<AccountMode> result = OperateResult.CreateFailedResult(accountInfo, "无数据");
           result.IsSuccess = true;
           return result;
            try
            {
                string accountIdSql = string.Format(@"SELECT *FROM T_ST_ACCOUNT WHERE ACC_CODE='{0}'", account);
                DataSet ds = DbHelper.GetDataSet(accountIdSql);
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
                    if (accountInfo.UseStatus.Equals(AccountStatusEnum.禁用))
                    {
                        result.Message = "账号已停用";
                        result.IsSuccess = false;
                        return result;
                    }
                    accountInfo.AccId = int.Parse(dr["ACC_ID"].ToString());
                    accountInfo.AccCode = dr["ACC_CODE"].ToString();
                    accountInfo.GroupId = int.Parse(dr["GROUP_ID"].ToString());
                    accountInfo.RoleLevel = (RoleLevelEnum)Enum.Parse(typeof(RoleLevelEnum), dr["ROLE_ID"].ToString());
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

        public override OperateResult UpdatePassword(string account,string newPassword)
        {
            throw new NotImplementedException();
        }

       public override OperateResult UpdatePersonInfo(PersonMode person)
       {
           throw new NotImplementedException();
       }

       public override OperateResult<DataSet> GetAccountList(string @where)
       {
           throw new NotImplementedException();
       }

       public override OperateResult CreateNewAccount(AccountMode newAccount)
       {
           throw new NotImplementedException();
       }

       public override List<GroupMode> GetAllGroupList()
       {
           throw new NotImplementedException();
       }

       public override OperateResult DeleteAccount(AccountMode account)
       {
           OperateResult result = OperateResult.CreateFailedResult();
           try
           {
               string sql = string.Format("DELETE FROM T_ST_ACCOUNT  WHERE ACC_CODE='{0}'", account.AccCode);
               bool done = DbHelper.ExecuteNonQuery(sql);
               result.IsSuccess = done;
               result.Message = "操作成功";
           }
           catch (Exception ex)
           {
               result.IsSuccess = false;
               result.Message = OperateResult.ConvertException(ex);
           }

           return result;
       }

       public override OperateResult EditAccount(AccountMode account)
       {
           OperateResult result = OperateResult.CreateFailedResult();
           try
           {
               string sql = string.Format(@"UPDATE TOP(1) T_ST_ACCOUNT SET GROUP_ID='{0}', ROLE_ID='{1}',USE_STATUS='{2}',MOMODIFY_DATE='{3}', REMARK='{4}',ONLINE_STATUS='{5}' WHERE ACC_CODE='{6}'", 
                   account.GroupId,(int)account.RoleLevel,account.UseStatus,DateTime.Now,account.Remark,(int)account.OnlineStatus,account.AccCode);
               bool done = DbHelper.ExecuteNonQuery(sql);
               result.IsSuccess = done;
               result.Message = "操作成功";
           }
           catch (Exception ex)
           {
               result.IsSuccess = false;
               result.Message = OperateResult.ConvertException(ex);
           }

           return result;
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
