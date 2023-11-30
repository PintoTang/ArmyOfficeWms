using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.SqlServer
{
    public sealed class StringCharTaskForOracle : StringCharTaskAbstract
    {
        public StringCharTaskForOracle(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult IsExist(StringCharTaskModel data)
        {
            OperateResult isExistResult = OperateResult.CreateFailedResult();
            try
            {
                string sql =
                    string.Format(
                        @"SELECT COUNT(1) AS Num FROM T_AC_StringCharTaskModel t WHERE t.TASK_CODE='{0}' AND t.WH_CODE='{1}'",
                        data.UniqueCode, WareHouseCode);
                string num = DbHelper.ExecuteScalar(sql);
                int count;
                int.TryParse(num, out count);
                if (count > 0)
                {
                    isExistResult.IsSuccess = true;
                    return isExistResult;
                }
                isExistResult.IsSuccess = false;
                return isExistResult;
            }
            catch (Exception ex)
            {
                isExistResult.IsSuccess = false;
                isExistResult.Message = OperateResult.ConvertException(ex);
            }
            return isExistResult;
        }

        public override OperateResult Update(StringCharTaskModel data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                string sql =
                    string.Format(
                        @"UPDATE T_AC_StringCharTaskModel SET TASK_PROCESS_STATUS={0},BELONG_DEVICE_ID={1} WHERE TASK_CODE='{2}' AND WH_CODE='{3}'",
                        (int)data.ProcessStatus, data.DeviceId, data.UniqueCode, WareHouseCode);
                updateResult.IsSuccess = DbHelper.ExecuteNonQuery(sql);
                return updateResult;
            }
            catch (Exception ex)
            {
                updateResult.IsSuccess = false;
                updateResult.Message = OperateResult.ConvertException(ex);
            }
            return updateResult;
        }

        public override OperateResult Insert(StringCharTaskModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                string sql =
    string.Format(
        @"INSERT INTO T_AC_StringCharTaskModel(TASK_CODE,UPPER_TASK_CODE,LOWER_TASK_CODE,TASK_SOURCE,TASK_TYPE,TASK_PROCESS_STATUS,BELONG_DEVICE_ID,TASK_VALUE,WH_CODE) 
                                                                      VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},'{7}','{8}')", data.UniqueCode, data.UpperTaskCode, data.LowerTaskCode, (int)data.TaskSource, (int)data.TaskType, (int)data.ProcessStatus, data.DeviceId, data.TaskValue, WareHouseCode);

                insertResult.IsSuccess = DbHelper.ExecuteNonQuery(sql);
                return insertResult;
            }
            catch (Exception ex)
            {
                insertResult.IsSuccess = false;
                insertResult.Message = OperateResult.ConvertException(ex);
            }
            return insertResult;
        }

        public override OperateResult<List<StringCharTaskModel>> GetUnFinishTask(int deviceId)
        {
            List<StringCharTaskModel> content = new List<StringCharTaskModel>();
            OperateResult<List<StringCharTaskModel>> getResult = OperateResult.CreateFailedResult<List<StringCharTaskModel>>(content, "初始化");
            try
            {
                string sql = string.Format(@"SELECT *FROM T_AC_StringCharTaskModel t WHERE t.BELONG_DEVICE_ID='{0}' AND t.TASK_PROCESS_STATUS='{1}'", deviceId, 2);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringCharTaskModel livedata = new StringCharTaskModel
                            {
                                TaskSource = ConvertHepler.ConvertToInt(dr["TASK_SOURCE"].ToString()),
                                UniqueCode = dr["TASK_CODE"].ToString(),
                                UpperTaskCode = dr["UPPER_TASK_CODE"].ToString(),
                                TaskType = ConvertHepler.ConvertToInt(dr["TASK_TYPE"].ToString()),
                                LowerTaskCode = dr["LOWER_TASK_CODE"].ToString(),
                                ProcessStatus = ConvertHepler.ConvertToInt(dr["TASK_PROCESS_STATUS"].ToString()),
                                DeviceId = ConvertHepler.ConvertToInt(dr["BELONG_DEVICE_ID"].ToString()),
                                TaskValue = dr["TASK_VALUE"].ToString()
                            };
                            content.Add(livedata);
                        }
                    }
                }
                getResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                getResult.IsSuccess = false;
                getResult.Message = OperateResult.ConvertException(ex);
            }
            return getResult;
        }

        public override OperateResult<List<StringCharTaskModel>> GetTaskByStatus(int deviceId, List<int> statusList)
        {
            string statusIds = "-1";
            foreach (int status in statusList)
            {
                statusIds += "," + (int)status;
            }
            List<StringCharTaskModel> content = new List<StringCharTaskModel>();
            OperateResult<List<StringCharTaskModel>> getResult = OperateResult.CreateFailedResult<List<StringCharTaskModel>>(content, "初始化");
            try
            {
                string sql = string.Format(@"SELECT *FROM T_AC_StringCharTaskModel t WHERE t.BELONG_DEVICE_ID='{0}' AND t.TASK_PROCESS_STATUS in ('{1}')", deviceId, statusIds);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            StringCharTaskModel livedata = new StringCharTaskModel
                            {
                                TaskSource = ConvertHepler.ConvertToInt(dr["TASK_SOURCE"].ToString()),
                                UniqueCode = dr["TASK_CODE"].ToString(),
                                UpperTaskCode = dr["UPPER_TASK_CODE"].ToString(),
                                TaskType = ConvertHepler.ConvertToInt(dr["TASK_TYPE"].ToString()),
                                LowerTaskCode = dr["LOWER_TASK_CODE"].ToString(),
                                ProcessStatus = ConvertHepler.ConvertToInt(dr["TASK_PROCESS_STATUS"].ToString()),
                                DeviceId = ConvertHepler.ConvertToInt(dr["BELONG_DEVICE_ID"].ToString()),
                                TaskValue = dr["TASK_VALUE"].ToString()
                            };
                            content.Add(livedata);
                        }
                    }
                }
                getResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                getResult.IsSuccess = false;
                getResult.Message = OperateResult.ConvertException(ex);
            }
            return getResult;
        }

    }
}
