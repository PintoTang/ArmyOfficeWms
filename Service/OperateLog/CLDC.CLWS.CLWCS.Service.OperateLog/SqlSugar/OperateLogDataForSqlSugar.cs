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
using CLDC.CLWS.CLWCS.Service.OperateLog.Model;

namespace CLDC.CLWS.CLWCS.Service.OperateLog
{
    public sealed class OperateLogDataForSqlSugar:OperateLogDataAbstract
    {
        public OperateLogDataForSqlSugar(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        public override OperateResult Insert(OperateLogModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                data.WhCode = WareHouseCode;
                bool res = DbHelper.AddReturnBool(data);
                //string insertSql =
                //    string.Format(
                //    @"INSERT INTO T_AC_OPERATE_LOG (LOG_CONTENT,LOG_USER_NAME,LOG_USER_ACCOUNT,LOG_RECORD_DATE,LOG_WH_CODE) VALUES ('{0}', '{1}', '{2}', '{3}','{4}');",
                //    data.LogContent,data.LogUserName,data.LogUserAccount,data.LogRecordDate,data.WhCode);
                insertResult.IsSuccess = res;// DbHelper.ExecuteNonQuery(insertSql);
            }
            catch (Exception ex)
            {
                insertResult.Message = OperateResult.ConvertException(ex);
                insertResult.IsSuccess = false;

            }
            return insertResult;
        }

        public override long GetTotalCount()
        {
            int count=DbHelper.QueryCount<OperateLogModel>(t => t.WhCode == WareHouseCode);
            //string sql = string.Format(" SELECT count(1) as Num FROM T_AC_OPERATE_LOG T WHERE T.LOG_WH_CODE='{0}'", WareHouseCode);
            //string num = DbHelper.ExecuteScalar(sql);
            //long count;
            //long.TryParse(num, out count);
            return count;
        }

        private Expression<Func<OperateLogModel, bool>> CombineWhereSql(OperateLogFilter filter)
        {
            Expression<Func<OperateLogModel, bool>> where = t => t.WhCode == WareHouseCode;

            //StringBuilder sqlBuilder = new StringBuilder();
            //sqlBuilder.Append(string.Format(" LOG_WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.LogContent))
            {
                where = where.AndAlso(t => t.LogContent.Contains(filter.LogContent));
                //sqlBuilder.Append(string.Format(" AND LOG_CONTENT LIKE '%{0}%'", filter.LogContent));
            }
            if (!string.IsNullOrEmpty(filter.LogUserAccount))
            {
                where = where.AndAlso(t => t.LogUserAccount==filter.LogUserAccount);
                //sqlBuilder.Append(string.Format(" AND LOG_USER_ACCOUNT='{0}'", filter.LogUserAccount));
            }
            if (!string.IsNullOrEmpty(filter.LogUserName))
            {
                where = where.AndAlso(t => t.LogUserName == filter.LogUserName);
                //sqlBuilder.Append(string.Format(" AND LOG_USER_NAME='{0}'", filter.LogUserName));
            }
            if (!string.IsNullOrEmpty(filter.RecordFromTime) && !string.IsNullOrEmpty(filter.RecordToTime))
            {
                where = where.AndAlso(t => t.LogRecordDate>=Convert.ToDateTime(filter.RecordFromTime) && t.LogRecordDate<=Convert.ToDateTime(filter.RecordToTime));
                //sqlBuilder.Append(string.Format(" AND  LOG_RECORD_DATE between  '{0}' and '{1}'", filter.RecordFromTime, filter.RecordToTime));
            }
            return where;
        }
        public override List<OperateLogModel> SelectData(OperateLogFilter filterModel,out int totalCount)
        {
            totalCount = 0;
            List<OperateLogModel> dataPool = new List<OperateLogModel>();
            var where = CombineWhereSql(filterModel);
            //string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY LOG_RECORD_DATE DESC) AS rownumber,* FROM T_AC_OPERATE_LOG WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filterModel.PageSize, filterModel.PageIndex, where);
            try
            {
                dataPool=DbHelper.QueryPageList((int)filterModel.PageIndex, (int)filterModel.PageSize, "LOG_RECORD_DATE DESC",out totalCount, where);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            OperateLogModel task = new OperateLogModel
                //            {
                //                LogRecordDate = ConvertHepler.ConvertToDateTime(dr["LOG_RECORD_DATE"].ToString()),
                //                LogContent = dr["LOG_CONTENT"].ToString(),
                //                LogUserAccount = dr["LOG_USER_ACCOUNT"].ToString(),
                //                LogUserName = dr["LOG_USER_NAME"].ToString(),
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
    }
}
