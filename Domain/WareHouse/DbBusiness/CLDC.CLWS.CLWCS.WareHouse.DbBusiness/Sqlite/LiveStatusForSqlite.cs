using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class LiveStatusForSqlite : LiveStatusAbstract
    {
        public LiveStatusForSqlite(IDbHelper dbHepler)
            : base(dbHepler)
        {

        }

        public override OperateResult IsExist(LiveStatusData data)
        {
            OperateResult isExistResult = OperateResult.CreateFailedResult();
            try
            {
                string sql = string.Format(" SELECT *FROM T_AC_LIVESTATUS T WHERE T.DEVICE_ID='{0}' AND T.WH_CODE='{1}'", data.Id, WareHouseCode);
                DataSet ds = DbHelper.GetDataSet(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
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

        public override OperateResult Update(LiveStatusData data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                string updateSql =
        string.Format(
            @"UPDATE T_AC_LIVESTATUS SET NAME='{0}',USESTATE='{1}',RUNSTATE='{2}',CONTROLSTATE='{3}',DISPATCHSTATE='{4}',ALIAS='{5}' WHERE WH_CODE='{6}' AND DEVICE_ID='{7}'",
            data.Name, data.UseState, data.RunState, data.ControlState, data.DispatchState, data.Alias, WareHouseCode, data.Id);
                updateResult.IsSuccess = DbHelper.ExecuteNonQuery(updateSql);
            }
            catch (Exception ex)
            {
                updateResult.Message = OperateResult.ConvertException(ex);
                updateResult.IsSuccess = false;

            }
            return updateResult;
        }

        public override OperateResult Insert(LiveStatusData data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                string insertSql =
                    string.Format(
                    @"INSERT INTO T_AC_LIVESTATUS(WH_CODE,DEVICE_ID,NAME,USESTATE,RUNSTATE,CONTROLSTATE,DISPATCHSTATE,ALIAS)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                    WareHouseCode, data.Id, data.Name, data.UseState, data.RunState, data.ControlState, data.DispatchState, data.Alias);

                insertResult.IsSuccess = DbHelper.ExecuteNonQuery(insertSql);
            }
            catch (Exception ex)
            {
                insertResult.Message = OperateResult.ConvertException(ex);
                insertResult.IsSuccess = false;

            }
            return insertResult;
        }

        public override OperateResult<LiveStatusData> GetDeviceLiveDataByDeviceId(int deviceId)
        {
            LiveStatusData liveData = new LiveStatusData();
            OperateResult<LiveStatusData> getResult = OperateResult.CreateFailedResult(liveData, "无数据");
            try
            {
                string selectSql =
                    string.Format(@"SELECT *FROM T_AC_LIVESTATUS T WHERE T.DEVICE_ID='{0}' AND T.WH_CODE='{1}'", deviceId, WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        liveData.Id = deviceId;
                        liveData.WH_Code = WareHouseCode;
                        liveData.Alias = dr["ALIAS"].ToString();
                        liveData.ControlState = ConvertHepler.ConvertToInt(dr["CONTROLSTATE"].ToString());
                        liveData.DispatchState = ConvertHepler.ConvertToInt(dr["DISPATCHSTATE"].ToString());
                        liveData.Name = dr["NAME"].ToString();
                        liveData.UseState = ConvertHepler.ConvertToInt(dr["USESTATE"].ToString());
                        liveData.RunState = ConvertHepler.ConvertToInt(dr["RUNSTATE"].ToString());
                        getResult.IsSuccess = true;
                    }
                    else
                    {
                        getResult.IsSuccess = false;
                    }
                }

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
