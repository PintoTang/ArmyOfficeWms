using System;
using System.Collections.Generic;
using System.Data;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;

namespace CLDC.CLWS.CLWCS.Service.Authorize.Sqlite
{
    public sealed class UserDataForSqlite : UserDataAbstract
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

        public UserDataForSqlite(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult<GroupMode> GetGroupInfoById(int groupId)
        {
            GroupMode groupInfo = new GroupMode();
            OperateResult<GroupMode> result = OperateResult.CreateFailedResult(groupInfo, "无数据");
            try
            {
                string accountIdSql = string.Format(@"SELECT *FROM T_ST_GROUP WHERE GROUP_ID='{0}'", groupId);
                DataSet ds = DbHelper.GetDataSet(accountIdSql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    groupInfo.GroupId = groupId;
                    groupInfo.GroupName = dr["GROUP_NAME"].ToString();
                    groupInfo.Remark = dr["REMARK"].ToString();
                    groupInfo.Description = dr["DEPT"].ToString();
                }
                else
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

                string accountIdSql = string.Format(("SELECT  * FROM T_ST_PERSON_INFO WHERE ACC_CODE='{0}' limit 1 offset 0"),
                    accCode);

                //string accountIdSql = string.Format(@"SELECT TOP(1) * FROM T_ST_PERSON_INFO WHERE ACC_CODE='{0}'", accCode);
                using (DataSet ds = DbHelper.GetDataSet(accountIdSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        personInfo.Description = dr["DEPT"].ToString();
                        personInfo.Skills = dr["SKILLS"].ToString();
                        personInfo.AccCode = accCode;
                        personInfo.Address = dr["ADDRESS"].ToString();
                        personInfo.WorkId = dr["WORK_ID"].ToString();
                        personInfo.PersonName = dr["PERSON_NAME"].ToString();
                        personInfo.Remark = dr["REMARK"].ToString();
                        personInfo.TelephoneNo = dr["TELEPHONE"].ToString();
                        personInfo.Email = dr["E_MAIL"].ToString();
                        personInfo.GroupId = ConvertHepler.ConvertToInt(dr["GROUP_ID"].ToString());
                    }
                    else
                    {
                        result.Message = string.Format("不存在人物信息：{0}", accCode);
                        result.IsSuccess = false;
                        return result;
                    }
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
            string verifySql =
                string.Format(
                    @"SELECT COUNT(*) AS num FROM T_ST_ACCOUNT WHERE ACC_CODE='{0}' AND PASSWORD='{1}'",
                    account, password);
            using (DataSet ds = DbHelper.GetDataSet(verifySql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString()).Equals(0))
                    {
                        return OperateResult.CreateFailedResult("密码验证失败");
                    }
                    return OperateResult.CreateSuccessResult();
                }
                return OperateResult.CreateFailedResult();
            }
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
                string updateSql = string.Format(@"UPDATE T_ST_ACCOUNT SET PASSWORD='{0}' WHERE ACC_CODE='{1}'",
                newPassword, account);
                bool result = DbHelper.ExecuteNonQuery(updateSql);
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
            string selectSql = string.Format("SELECT COUNT(*) AS num FROM T_ST_PERSON_INFO WHERE ACC_CODE='{0}'",
                person.AccCode);
            using (DataSet ds = DbHelper.GetDataSet(selectSql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString()).Equals(0))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public override OperateResult UpdatePersonInfo(PersonMode person)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                string sql = string.Empty;
                if (IsExistPersonInfo(person))
                {
                     sql =
                   string.Format(
                       @"UPDATE T_ST_PERSON_INFO SET PERSON_NAME = '{0}', SKILLS = '{1}',DEPT= '{2}',ADDRESS = '{3}',TELEPHONE = '{4}',E_MAIL = '{5}',REMARK = '{6}',WORK_ID='{7}',GROUP_ID='{8}' WHERE	ACC_CODE = '{9}'",
                       person.PersonName, person.Skills, person.Description, person.Address, person.TelephoneNo, person.Email, person.Remark,person.WorkId,person.GroupId, person.AccCode);
                }
                else
                {
                    sql =
                        string.Format(
                            @"INSERT INTO T_ST_PERSON_INFO (PERSON_NAME, WORK_ID, SKILLS, DEPT, ADDRESS, TELEPHONE, E_MAIL,REMARK, GROUP_ID,ACC_CODE) VALUES (
                                                            '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')",
                                                              person.PersonName, person.WorkId, person.Skills, person.Description, person.Address, person.TelephoneNo, person.Email, person.Remark, person.GroupId,person.AccCode);
                }
                bool result = DbHelper.ExecuteNonQuery(sql);
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

        public override OperateResult<DataSet> GetAccountList(string @where)
        {

            OperateResult<DataSet> result = OperateResult.CreateFailedResult<DataSet>("无数据");
            try
            {
                DataSet ds = DbHelper.GetDataSet(@where);
                result.IsSuccess = true;
                result.Content = ds;
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
                string insertSql =
                    string.Format(
                        @"INSERT INTO T_ST_ACCOUNT (ACC_CODE, GROUP_ID, ROLE_ID, PASSWORD, ENABLE_TIME, DISABLE_TIME, USE_STATUS, CREATER, CREATE_DATE, MODIFIER, MOMODIFY_DATE, REMARK, ONLINE_STATUS) VALUES
                            ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
                                newAccount.AccCode,newAccount.GroupId,(int)newAccount.RoleLevel,newAccount.Password,newAccount.EnableTime,newAccount.DisableTime,(int)newAccount.UseStatus,newAccount.CreaterId,newAccount.CreateTime,newAccount.ModifierId,newAccount.ModifierTime,newAccount.Remark,(int)newAccount.OnlineStatus);
               bool result= DbHelper.ExecuteNonQuery(insertSql);
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
            List<GroupMode> groupList = new List<GroupMode>();
            try
            {
                string selectSql = "SELECT *FROM T_ST_GROUP";
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dataRow in ds.Tables[0].Rows)
                        {
                            GroupMode group = new GroupMode();
                            group.GroupId = ConvertHepler.ConvertToInt(dataRow["GROUP_ID"].ToString());
                            group.GroupName = dataRow["GROUP_NAME"].ToString();
                            groupList.Add(group);
                        }
                    }
                    else
                    {
                        return groupList;
                    }
                }
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
                    account.GroupId, (int)account.RoleLevel, account.UseStatus, DateTime.Now, account.Remark, (int)account.OnlineStatus, account.AccCode);
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
    }
}
