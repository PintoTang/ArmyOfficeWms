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
    public sealed class LiveStatusForSqlSugar : LiveStatusAbstract
    {
        public LiveStatusForSqlSugar(IDbHelper dbHepler)
            : base(dbHepler)
        {

        }

        public override OperateResult IsExist(LiveStatusData data)
        {
            OperateResult isExistResult = OperateResult.CreateFailedResult();
            try
            {
                bool isExists = DbHelper.IsExist<LiveStatusData>(t => t.Id == data.Id && t.WH_Code == WareHouseCode);
                if (isExists)
                {
                    return OperateResult.CreateSuccessResult();
                }
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
                bool result = DbHelper.Update<LiveStatusData>(t => new LiveStatusData
                {
                    Name = data.Name,
                    UseState = data.UseState,
                    RunState = data.RunState,
                    ControlState = data.ControlState,
                    DispatchState = data.DispatchState,
                    Alias = data.Alias,
                }, t => t.Id == data.Id && t.WH_Code == WareHouseCode) > 0;

                //        string updateSql =
                //string.Format(
                //    @"UPDATE T_AC_LIVESTATUS SET NAME='{0}',USESTATE='{1}',RUNSTATE='{2}',CONTROLSTATE='{3}',DISPATCHSTATE='{4}',ALIAS='{5}' WHERE WH_CODE='{6}' AND DEVICE_ID='{7}'",
                //    data.Name, data.UseState, data.RunState, data.ControlState, data.DispatchState, data.Alias, WareHouseCode, data.Id);
                updateResult.IsSuccess = result;//DbHelper.ExecuteNonQuery(updateSql);
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
                data.WH_Code = WareHouseCode;
                bool result = DbHelper.Add(data) > 0;
                //string insertSql =
                //    string.Format(
                //    @"INSERT INTO T_AC_LIVESTATUS(WH_CODE,NAME,USESTATE,RUNSTATE,CONTROLSTATE,DISPATCHSTATE,ALIAS)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                //    WareHouseCode, data.Name, data.UseState, data.RunState, data.ControlState, data.DispatchState, data.Alias);

                insertResult.IsSuccess = result;//DbHelper.ExecuteNonQuery(insertSql);
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
                liveData = DbHelper.Query<LiveStatusData>(t => t.Id == deviceId && t.WH_Code == WareHouseCode);
                getResult.IsSuccess = liveData!=null && !string.IsNullOrEmpty(liveData.Name);
                getResult.Content = liveData;
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
