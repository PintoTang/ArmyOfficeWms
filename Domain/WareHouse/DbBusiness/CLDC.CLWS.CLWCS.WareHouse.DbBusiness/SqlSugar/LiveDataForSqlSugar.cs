using System;
using System.Collections.Generic;
using System.Data;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class LiveDataForSqlSugar : LiveDataAbstract
    {
        public LiveDataForSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }
        public override OperateResult IsExist(LiveData data)
        {
            bool isExists = DbHelper.IsExist<LiveData>(t => t.DeviceId == data.DeviceId && t.WH_Code == WareHouseCode && t.Index == data.Index);
            //string sql = string.Format(" SELECT count(1) as Num FROM T_AC_LIVESDATA T WHERE T.DEVICE_ID='{0}' AND T.WH_CODE='{1}' AND T.DATA_INDEX='{2}'", data.DeviceId, WareHouseCode, data.Index);
            //string num = DbHelper.ExecuteScalar(sql);
            //int count;
            //int.TryParse(num, out count);
            if (isExists)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult Update(LiveData data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                bool result=DbHelper.Update<LiveData>(t => new LiveData
                {
                    Name=data.Name,
                    HandleStatus = data.HandleStatus,
                    Alias = data.Alias,
                    DataValue = data.DataValue,
                }, t => t.DeviceId == data.DeviceId && t.WH_Code == WareHouseCode)>0;
                //string updateSql =
        //string.Format(
        //    @"UPDATE T_AC_LIVESDATA SET DEVICE_NAME='{0}',HANDLESTATUS='{1}',ALIAS='{2}',DATAVALUE='{3}'  WHERE WH_CODE='{4}' AND DEVICE_ID='{5}' ",
        //    data.Name, (int)data.HandleStatus, data.Alias, data.DataValue, WareHouseCode, data.DeviceId);
                updateResult.IsSuccess = result;
            }
            catch (Exception ex)
            {
                updateResult.Message = OperateResult.ConvertException(ex);
                updateResult.IsSuccess = false;

            }
            return updateResult;
        }

        public override OperateResult Insert(LiveData data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                data.WH_Code = WareHouseCode;
                bool result = DbHelper.Add(data)>0;
                //string insertSql =
                //    string.Format(
                //    @"INSERT INTO T_AC_LIVESDATA(WH_CODE,DEVICE_ID,DEVICE_NAME,HANDLESTATUS,DATAVALUE,ALIAS,DATA_INDEX)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                //WareHouseCode, data.DeviceId, data.Name, (int)data.HandleStatus, data.DataValue, data.Alias, data.Index);
                insertResult.IsSuccess = result;// DbHelper.ExecuteNonQuery(insertSql);
            }
            catch (Exception ex)
            {
                insertResult.Message = OperateResult.ConvertException(ex);
                insertResult.IsSuccess = false;

            }
            return insertResult;
        }


        public override bool IsExistLiveData(LiveData data)
        {
            bool isExists=DbHelper.IsExist<LiveData>(t => t.DeviceId == data.DeviceId && t.WH_Code == WareHouseCode && t.DataValue == data.DataValue && t.HandleStatus == data.HandleStatus);
            return isExists;
        }

        public override OperateResult<List<LiveData>> GetAllLiveData(int deviceId)
        {
            List<LiveData> liveDataList = null;
            OperateResult<List<LiveData>> result = OperateResult.CreateSuccessResult(liveDataList);
            try
            {
                liveDataList = DbHelper.QueryList<LiveData>(t => t.DeviceId == deviceId && t.WH_Code == WareHouseCode && t.HandleStatus == HandleStatusEnum.UnFinished);
                //string sql = string.Format(" SELECT *FROM T_AC_LIVESDATA T WHERE T.DEVICE_ID='{0}' AND T.WH_CODE='{1}' AND T.HANDLESTATUS='{2}'",
                //    deviceId, WareHouseCode, (int)HandleStatusEnum.UnFinished);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            LiveData livedata = new LiveData
                //            {
                //                Index = ConvertHepler.ConvertToInt(dr["DATA_INDEX"].ToString()),
                //                Alias = dr["ALIAS"].ToString(),
                //                DataValue = dr["DATAVALUE"].ToString(),
                //                DeviceId = ConvertHepler.ConvertToInt(dr["DEVICE_ID"].ToString()),
                //                Name = dr["DEVICE_NAME"].ToString(),
                //                WH_Code = dr["WH_CODE"].ToString(),
                //                HandleStatus = (HandleStatusEnum)ConvertHepler.ConvertToInt(dr["HANDLESTATUS"].ToString())
                //            };
                //            liveDataList.Add(livedata);
                //        }
                //    }  
                //}
                result.Content = liveDataList;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult DeleteLiveData(LiveData data)
        {
            OperateResult deleteResult = OperateResult.CreateFailedResult();
            try
            {
                bool result=DbHelper.Delete<LiveData>(t => t.DataValue == data.DataValue && t.WH_Code == WareHouseCode && t.DeviceId == data.DeviceId && t.Index == data.Index)>0;
                //    string updateSql = string.Format(@"DELETE T_AC_LIVESDATA WHERE DATAVALUE='{0}' AND DATA_INDEX='{3}' AND WH_CODE='{1}' AND DEVICE_ID='{2}'",
                //data.DataValue, WareHouseCode, data.DeviceId,data.Index);
                //    deleteResult.IsSuccess = DbHelper.ExecuteNonQuery(updateSql);
                deleteResult.IsSuccess = result;
            }
            catch (Exception ex)
            {
                deleteResult.Message = OperateResult.ConvertException(ex);
                deleteResult.IsSuccess = false;

            }
            return deleteResult;
        }

        public override OperateResult ClearLiveData(int deviceId)
        {
            OperateResult deleteResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Delete<LiveData>(t => t.WH_Code == WareHouseCode && t.DeviceId == deviceId)>0;
             //   string updateSql = string.Format(@"DELETE T_AC_LIVESDATA WHERE  WH_CODE='{0}' AND DEVICE_ID='{1}'",
             //WareHouseCode, deviceId);
                deleteResult.IsSuccess = result;
            }
            catch (Exception ex)
            {
                deleteResult.Message = OperateResult.ConvertException(ex);
                deleteResult.IsSuccess = false;

            }
            return deleteResult;
        }
    }
}
