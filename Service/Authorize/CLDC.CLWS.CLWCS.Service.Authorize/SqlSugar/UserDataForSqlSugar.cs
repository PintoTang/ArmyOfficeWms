using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    public sealed class UserDataForSqlSugar : UserDataAbstract
    {
        /// <summary>
        /// 是否存在指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override OperateResult IsExist(UserInformationMode data)
        {
            bool isExists=DbHelper.IsExist<AccountMode>(t => t.AccCode == data.Account.AccCode);
            
            if (isExists)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult();
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

        public UserDataForSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult<GroupMode> GetGroupInfoById(int groupId)
        {
            GroupMode groupInfo = null;
            OperateResult<GroupMode> result = OperateResult.CreateFailedResult(groupInfo, "无数据");
            try
            {
                groupInfo = DbHelper.Query<GroupMode>(t => t.GroupId == groupId);
                if(groupInfo==null)
                {
                    result.Message = string.Format("不存在组编号：{0}", groupId);
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
            AccountMode accountInfo = null;
            OperateResult<AccountMode> result = OperateResult.CreateFailedResult(accountInfo, "无数据");
            try
            {
                accountInfo = DbHelper.Query<AccountMode>(t => t.AccCode == accountId);
                if(accountInfo==null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("不存在用户名：{0}，请核对用户名", accountId);
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
            PersonMode personInfo = null;
            OperateResult<PersonMode> result = OperateResult.CreateFailedResult(personInfo, "无数据");
            try
            {
                personInfo = DbHelper.Query<PersonMode>(t => t.AccCode == accCode);
                //string accountIdSql = string.Format(@"SELECT TOP(1) * FROM T_ST_PERSON_INFO WHERE ACC_CODE='{0}'", accCode);
                //using (DataSet ds = DbHelper.GetDataSet(accountIdSql))
                //{
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //    {
                //        DataRow dr = ds.Tables[0].Rows[0];
                //        personInfo.Description = dr["DEPT"].ToString();
                //        personInfo.Skills = dr["SKILLS"].ToString();
                //        personInfo.AccCode = accCode;
                //        personInfo.Address = dr["ADDRESS"].ToString();
                //        personInfo.WorkId = dr["WORK_ID"].ToString();
                //        personInfo.PersonName = dr["PERSON_NAME"].ToString();
                //        personInfo.Remark = dr["REMARK"].ToString();
                //        personInfo.TelephoneNo = dr["TELEPHONE"].ToString();
                //        personInfo.Email = dr["E_MAIL"].ToString();
                //        personInfo.GroupId = ConvertHepler.ConvertToInt(dr["GROUP_ID"].ToString());
                //    }
                //    else
                //    {
                //        result.Message = string.Format("不存在人物信息：{0}", accCode);
                //        result.IsSuccess = false;
                //        return result;
                //    }
                //}
                if (personInfo == null)
                {
                    result.Message = string.Format("不存在人物信息：{0}", accCode);
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
            int count = DbHelper.QueryCount<AccountMode>(t => t.AccCode == account && t.Password == password);
            if (count == 0)
            {
                return OperateResult.CreateFailedResult("密码验证失败");
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public override OperateResult UpdatePassword(string account, string newPassword)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                bool result=DbHelper.Update<AccountMode>(t => new AccountMode { Password = newPassword }, t => t.AccCode == account)>0;
                //string updateSql = string.Format(@"UPDATE T_ST_ACCOUNT SET PASSWORD='{0}' WHERE ACC_CODE='{1}'",
                //newPassword, account);
                //bool result = DbHelper.ExecuteNonQuery(updateSql);
                updateResult.IsSuccess = result;
                if (!result)
                {
                    updateResult.Message = "更新密码执行失败";
                }
                return updateResult;
            }
            catch (Exception ex)
            {
                updateResult.IsSuccess = false;
                updateResult.Message = OperateResult.ConvertException(ex);

            }
            return updateResult;
        }

        private bool IsExistPersonInfo(PersonMode person)
        {
            bool isExists=DbHelper.IsExist<PersonMode>(t => t.AccCode == person.AccCode);
            return isExists;
            //string selectSql = string.Format("SELECT COUNT(*) AS num FROM T_ST_PERSON_INFO WHERE ACC_CODE='{0}'",
            //    person.AccCode);
            //using (DataSet ds = DbHelper.GetDataSet(selectSql))
            //{
            //    if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    {
            //        if (ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString()).Equals(0))
            //        {
            //            return false;
            //        }
            //        return true;
            //    }
            //    return false;
            //}
        }

        public override OperateResult UpdatePersonInfo(PersonMode person)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = false;
                var dbInfo = DbHelper.Query<PersonMode>(t => t.AccCode == person.AccCode);
                if (dbInfo != null)
                {
                    dbInfo.PersonName = person.PersonName;
                    dbInfo.Skills = person.Skills;
                    dbInfo.Description = person.Description;
                    dbInfo.Address = person.Address;
                    dbInfo.TelephoneNo = person.TelephoneNo;
                    dbInfo.Email = person.Email;
                    dbInfo.Remark = person.Remark;
                    dbInfo.WorkId = person.WorkId;
                    dbInfo.GroupId = person.GroupId;
                    result=DbHelper.Update(dbInfo)>0;
                }
                else
                {
                    result = DbHelper.Add(person)>0;
                }
                updateResult.IsSuccess = result;
                if (!result)
                {
                    updateResult.Message = "更新用户信息执行失败";
                }
                return updateResult;
            }
            catch (Exception ex)
            {
                updateResult.IsSuccess = false;
                updateResult.Message = OperateResult.ConvertException(ex);
            }
            return updateResult;
        }

        public override OperateResult<List<AccountMode>> GetAccountList(Expression<Func<AccountMode, bool>> whereLambda = null)
        {

            OperateResult<List<AccountMode>> result = OperateResult.CreateFailedResult<List<AccountMode>>("无数据");
            try
            {
                List<AccountMode> list=DbHelper.QueryList(whereLambda);
                //DataSet ds = DbHelper.GetDataSet(@where);
                result.IsSuccess = true;
                result.Content = list;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        public override OperateResult CreateNewAccount(AccountMode newAccount)
        {
            OperateResult createResult=OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(newAccount) > 0;
               // string insertSql =
               //     string.Format(
               //         @"INSERT INTO T_ST_ACCOUNT (ACC_CODE, GROUP_ID, ROLE_ID, PASSWORD, ENABLE_TIME, DISABLE_TIME, USE_STATUS, CREATER, CREATE_DATE, MODIFIER, MOMODIFY_DATE, REMARK, ONLINE_STATUS) VALUES
               //             ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
               //                 newAccount.AccCode,newAccount.GroupId,(int)newAccount.RoleLevel,newAccount.Password,newAccount.EnableTime,newAccount.DisableTime,(int)newAccount.UseStatus,newAccount.CreaterId,newAccount.CreateTime,newAccount.ModifierId,newAccount.ModifierTime,newAccount.Remark,(int)newAccount.OnlineStatus);
               //bool result= DbHelper.ExecuteNonQuery(insertSql);
                if (!result)
                {
                    createResult.Message = "操作数据库错误";
                }
                createResult.IsSuccess = result;
                return createResult;
            }
            catch (Exception ex)
            {
                createResult.IsSuccess = false;
                createResult.Message = OperateResult.ConvertException(ex);
            }
            return createResult;
        }

        public override List<GroupMode> GetAllGroupList()
        {
            List<GroupMode> groupList = null;
            try
            {
                groupList=DbHelper.QueryList<GroupMode>();
                //string selectSql = "SELECT *FROM T_ST_GROUP";
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables.Count > 0)
                //    {
                //        foreach (DataRow dataRow in ds.Tables[0].Rows)
                //        {
                //            GroupMode group = new GroupMode();
                //            group.GroupId = ConvertHepler.ConvertToInt(dataRow["GROUP_ID"].ToString());
                //            group.GroupName = dataRow["GROUP_NAME"].ToString();
                //            groupList.Add(group);
                //        }
                //    }
                //    else
                //    {
                //        return groupList;
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
            return groupList;
        }

        public override OperateResult DeleteAccount(AccountMode account)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                bool done=DbHelper.Delete<AccountMode>(t => t.AccCode == account.AccCode)>0;
                //string sql = string.Format("DELETE FROM T_ST_ACCOUNT  WHERE ACC_CODE='{0}'", account.AccCode);
                //bool done = DbHelper.ExecuteNonQuery(sql);
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
                bool done=DbHelper.Update<AccountMode>(t => new AccountMode { 
                    GroupId=account.GroupId,
                    RoleLevel=account.RoleLevel,
                    UseStatus=account.UseStatus,
                    ModifierTime=DateTime.Now,
                    Remark=account.Remark,
                    OnlineStatus=account.OnlineStatus,
                }, t => t.AccCode == account.AccCode)>0;
                //string sql = string.Format(@"UPDATE TOP(1) T_ST_ACCOUNT SET GROUP_ID='{0}', ROLE_ID='{1}',USE_STATUS='{2}',MOMODIFY_DATE='{3}', REMARK='{4}',ONLINE_STATUS='{5}' WHERE ACC_CODE='{6}'",
                //    account.GroupId, (int)account.RoleLevel, (int)account.UseStatus, DateTime.Now, account.Remark, (int)account.OnlineStatus, account.AccCode);
                //bool done = DbHelper.ExecuteNonQuery(sql);
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
    }
}
