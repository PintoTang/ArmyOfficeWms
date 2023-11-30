using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class ReceiveDataForSqlSugar : ReceiveDataAbstract
    {
        public ReceiveDataForSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult IsExist(ReceiveDataModel data)
        {
            OperateResult isExistResult = OperateResult.CreateFailedResult();
            try
            {
                bool isExists=DbHelper.IsExist<ReceiveDataModel>(t => t.WhCode == WareHouseCode && t.GuidId == data.GuidId);
                
                //string[] sql = new string[1];
                //sql[0] = string.Format("select * from T_BO_RECEIVEDATA  where rd_guid='{0}' and wh_code='{1}'", 
                //    data.GuidId, data.WhCode);
                //DataSet ds = DbHelper.GetDataSet(sql);
                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                //{
                //    isExistResult.IsSuccess = true;
                //    return isExistResult;
                //}
                isExistResult.IsSuccess = isExists;
                return isExistResult;
            }
            catch (Exception ex)
            {
                isExistResult.IsSuccess = true;
                isExistResult.Message = OperateResult.ConvertException(ex);
            }
            return isExistResult;
        }

        public override OperateResult Update(ReceiveDataModel data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                int result=DbHelper.Update<ReceiveDataModel>(t => new ReceiveDataModel
                {
                    HandleStatus=data.HandleStatus,
                    HandleDateTime=data.HandleDateTime,
                    HandleMessage=data.HandleMessage,
                }, t => t.WhCode == WareHouseCode && t.GuidId == data.GuidId);
                //var sql = string.Format("update T_BO_RECEIVEDATA set dhstatus_id='{0}',rd_handlerdate='{1}',rd_handlermessage='{2}' where rd_guid='{3}' and wh_code='{4}'", 
                //    (int)data.HandleStatus, data.HandleDateTime, data.HandleMessage, data.GuidId, data.WhCode);
                //int result = DbHelper.ExecuteNonQuery(new[] { sql });
                if (result <= 0)
                {
                    updateResult.IsSuccess = false;
                    updateResult.Message = string.Format("更新数据失败");
                    return updateResult;
                }
                updateResult.IsSuccess = true;
                return updateResult;
            }
            catch (Exception ex)
            {
                updateResult.IsSuccess = false;
                updateResult.Message = OperateResult.ConvertException(ex);
            }
            return updateResult;
        }

        public override OperateResult Insert(ReceiveDataModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                using (var dbContext = DbHelper.CreateContext())
                {
                    dbContext.Ado.BeginTran();
                    bool result = DbHelper.Add(dbContext, new ReceiveDataModel
                    {
                        GuidId=data.GuidId,
                        WhCode = WareHouseCode,
                        MethodName = data.MethodName,
                        MethodParamValue = data.MethodParamValue,
                        HandleMessage = data.HandleMessage,
                        Source = data.Source,
                        HandleStatus=ReceiveDataHandleStatus.UnHandle,
                        ReceiveDateTime=DateTime.Now,
                        InstructIds=data.InstructIds,
                    })>0;
                    if (!result)
                    {
                        dbContext.Ado.RollbackTran();
                        return OperateResult.CreateFailedResult("插入失败");
                    }
                    result=DbHelper.Add(dbContext, new LogRecord
                    {
                        LR_GUID=data.GuidId,
                        WH_Code= WareHouseCode,
                        LR_TriggerSystem=2,
                        LR_MethodName= data.MethodName,
                        LR_CreateDate=DateTime.Now,
                        LR_ParamValue= data.MethodParamValue,
                    })>0;
                    if (!result)
                    {
                        dbContext.Ado.RollbackTran();
                        return OperateResult.CreateFailedResult("插入失败");
                    }
                    dbContext.Ado.CommitTran();
                }
                //    string[] sql = new string[2];
                //sql[0] = string.Format("insert into T_AC_LogRecord (LR_GUID,WH_Code,LR_TriggerSystem,LR_MethodName, LR_ParamValue) values ('{0}','{1}','{2}','{3}','{4}')", 
                //    data.GuidId, data.WhCode, 2, data.MethodName, data.MethodParamValue);
                //sql[1] = string.Format("insert into T_BO_ReceiveData(RD_GUID,WH_Code,RD_MethodName,RD_ParamValue,rd_handlermessage,rd_source) values ('{0}','{1}','{2}','{3}','{4}','{5}')", 
                //    data.GuidId, data.WhCode, data.MethodName, data.MethodParamValue, data.HandleMessage, data.Source);
                //int result = DbHelper.ExecuteNonQuery(sql);
                //if (result != 2)
                //{

                //    insertResult.IsSuccess = false;
                //    insertResult.Message = "当前调用的方法为：" + data.MethodName + "，写：" +
                //                           data.MethodParamValue + " 入数据库时出错,插入语句为：" + string.Join(";", sql);
                //    return insertResult;
                //}
                insertResult.IsSuccess = true;
                return insertResult;
            }
            catch (Exception ex)
            {
                insertResult.IsSuccess = false;
                insertResult.Message = OperateResult.ConvertException(ex);

            }
            return insertResult;
        }

        private Expression<Func<ReceiveDataModel, bool>> CombineWhereSql(ReceiveDataFilter filter)
        {
            Expression<Func<ReceiveDataModel, bool>> whereLambda = t => t.WhCode == WareHouseCode;
            //StringBuilder sqlBuilder = new StringBuilder();
            //sqlBuilder.Append(string.Format(" WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.MethodName))
            {
                whereLambda = whereLambda.AndAlso(t => t.MethodName.Contains(filter.MethodName));
                //sqlBuilder.Append(string.Format(" AND rd_methodname LIKE '%{0}%'", filter.MethodName));
            }
            if (!string.IsNullOrEmpty(filter.HandleStatus))
            {
                whereLambda = whereLambda.AndAlso(t => t.HandleStatus==(ReceiveDataHandleStatus)Convert.ToInt32(filter.HandleStatus));
                //sqlBuilder.Append(string.Format(" AND dhstatus_id={0}", filter.HandleStatus));
            }
            if (!string.IsNullOrEmpty(filter.HandleFromTime) && !string.IsNullOrEmpty(filter.HandleToTime))
            {
                whereLambda = whereLambda.AndAlso(t => t.HandleDateTime >= Convert.ToDateTime(filter.HandleFromTime) && t.HandleDateTime <= Convert.ToDateTime(filter.HandleToTime));
                //sqlBuilder.Append(string.Format(" AND  rd_handlerdate between  '{0}' and '{1}'", filter.HandleFromTime, filter.HandleToTime));
            }
            if (!string.IsNullOrEmpty(filter.ReceiveFromTime) && !string.IsNullOrEmpty(filter.ReceiveToTime))
            {
                whereLambda = whereLambda.AndAlso(t => t.ReceiveDateTime >= Convert.ToDateTime(filter.ReceiveFromTime) && t.HandleDateTime <= Convert.ToDateTime(filter.ReceiveToTime));
                //sqlBuilder.Append(string.Format(" AND  rd_receivedate between  '{0}' and '{1}'", filter.ReceiveFromTime, filter.ReceiveToTime));
            }
            return whereLambda;
        }
        public override List<ReceiveDataModel> SelectData(ReceiveDataFilter filter,out int totalCount)
        {
            totalCount = 0;
            List<ReceiveDataModel> dataPool = new List<ReceiveDataModel>();
            var where = CombineWhereSql(filter);
            //string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY rd_receivedate DESC) AS rownumber,* FROM T_BO_ReceiveData WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filter.PageSize, filter.PageIndex, where);
            try
            {
                dataPool = DbHelper.QueryPageList<ReceiveDataModel>((int)filter.PageIndex, (int)filter.PageSize, "rd_receivedate DESC", out totalCount, where);
                
                //using (DataTable dt = DbHelper.QueryDataTable(selectSql))
                //{
                //    if (dt.Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in dt.Rows)
                //        {
                //            ReceiveDataModel task = new ReceiveDataModel
                //            {
                //                HandleDateTime = ConvertHepler.ConvertToDateTime(dr["rd_handlerdate"].ToString()),
                //                HandleMessage = dr["rd_handlermessage"].ToString(),
                //                HandleStatus = (ReceiveDataHandleStatus)ConvertHepler.ConvertToInt(dr["dhstatus_id"].ToString()),
                //                MethodName = dr["rd_methodname"].ToString(),
                //                MethodParamValue = dr["rd_paramvalue"].ToString(),
                //                Note = dr["rd_note"].ToString(),
                //                ReceiveDateTime = ConvertHepler.ConvertToDateTime(dr["rd_receivedate"].ToString()),
                //                Source = dr["rd_source"].ToString(),
                //                WhCode = dr["wh_code"].ToString()
                //            };
                //            dataPool.Add(task);
                //        }
                //    }
                //}
            }
            catch (Exception)
            {

            }
            return dataPool;

        }

        public override long GetTotalCount()
        {
            int count=DbHelper.QueryCount<ReceiveDataModel>(t => t.WhCode == WareHouseCode);
            //string sql = string.Format(" SELECT count(1) as Num FROM T_BO_ReceiveData T WHERE T.WH_CODE='{0}'", WareHouseCode);
            //string num = DbHelper.ExecuteScalar(sql);
            //long count;
            //long.TryParse(num, out count);
            return count;
        }
    }
}
