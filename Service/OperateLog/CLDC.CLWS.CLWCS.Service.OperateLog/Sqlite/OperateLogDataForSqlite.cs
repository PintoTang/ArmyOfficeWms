using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.OperateLog.Model;

namespace CLDC.CLWS.CLWCS.Service.OperateLog.Sqlite
{
    public sealed class OperateLogDataForSqlite:OperateLogDataAbstract
    {
        public OperateLogDataForSqlite(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult Insert(OperateLogModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                string insertSql =
                    string.Format(
                    @"INSERT INTO T_AC_OPERATE_LOG (LOG_CONTENT,LOG_USER_NAME,LOG_USER_ACCOUNT,LOG_RECORD_DATE,LOG_WH_CODE) VALUES ('{0}', '{1}', '{2}', '{3}','{4}');",
                    data.LogContent, data.LogUserName, data.LogUserAccount, data.LogRecordDate.ToString("yyyy-MM-dd HH:MM:ss"), data.WhCode);
                insertResult.IsSuccess = DbHelper.ExecuteNonQuery(insertSql);
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
            string sql = string.Format(" SELECT count(1) as Num FROM T_AC_OPERATE_LOG T WHERE T.LOG_WH_CODE='{0}'", WareHouseCode);
            string num = DbHelper.ExecuteScalar(sql);
            long count;
            long.TryParse(num, out count);
            return count;
        }

        private string CombineWhereSql(OperateLogFilter filter)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(string.Format(" LOG_WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.LogContent))
            {
                sqlBuilder.Append(string.Format(" AND LOG_CONTENT LIKE '%{0}%'", filter.LogContent));
            }
            if (!string.IsNullOrEmpty(filter.LogUserAccount))
            {
                sqlBuilder.Append(string.Format(" AND LOG_USER_ACCOUNT='{0}'", filter.LogUserAccount));
            }
            if (!string.IsNullOrEmpty(filter.LogUserName))
            {
                sqlBuilder.Append(string.Format(" AND LOG_USER_NAME='{0}'", filter.LogUserName));
            }
            if (!string.IsNullOrEmpty(filter.RecordFromTime) && !string.IsNullOrEmpty(filter.RecordToTime))
            {
                sqlBuilder.Append(string.Format(" AND  LOG_RECORD_DATE between  '{0}' and '{1}'", filter.RecordFromTime, filter.RecordToTime));
            }
            return sqlBuilder.ToString();
        }

        public override List<OperateLogModel> SelectData(OperateLogFilter filterModel)
        {
            List<OperateLogModel> dataPool = new List<OperateLogModel>();
            string where = CombineWhereSql(filterModel);

            //string strsele =
            //    " select * from T_AC_OPERATE_LOG  WHERE 1=1 and  LOG_WH_CODE ='HZ1'  ORDER BY DATETIME(LOG_RECORD_DATE),DATETIME('YYYY-MM-DD HH:MM:SS', 'NNN years', 'NNN months', 'NNN days','NNN hours','NNN minutes','NNN.NNNN seconds') desc ";

            string selectSql = string.Format(@"select * from T_AC_OPERATE_LOG  WHERE 1=1 and {0}
order by DATETIME(LOG_RECORD_DATE)  desc limit {1} offset {2} ", where, filterModel.PageSize, filterModel.PageIndex);
//            string selectSql = string.Format(@"select * from T_AC_OPERATE_LOG  WHERE 1=1 and {0}
//order by LOG_RECORD_DATE limit {1} offset {2} ", where, filterModel.PageSize, filterModel.PageIndex);
            try
            {
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            OperateLogModel task = new OperateLogModel
                            {
                                LogRecordDate = ConvertHepler.ConvertToDateTime(dr["LOG_RECORD_DATE"].ToString()),
                                LogContent = dr["LOG_CONTENT"].ToString(),
                                LogUserAccount = dr["LOG_USER_ACCOUNT"].ToString(),
                                LogUserName = dr["LOG_USER_NAME"].ToString(),
                            };
                            dataPool.Add(task);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return dataPool;
        }
    }
}
