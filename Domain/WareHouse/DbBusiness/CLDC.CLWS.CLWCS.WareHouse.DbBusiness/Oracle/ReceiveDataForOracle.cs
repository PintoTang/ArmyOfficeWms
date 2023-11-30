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

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Oracle
{
   public sealed class ReceiveDataForOracle:ReceiveDataAbstract
    {
       public ReceiveDataForOracle(IDbHelper dbHelper) : base(dbHelper)
       {
       }

       public override OperateResult IsExist(ReceiveDataModel data)
       {
           throw new NotImplementedException();
       }

       public override OperateResult Update(ReceiveDataModel data)
       {
           throw new NotImplementedException();
       }

       public override OperateResult Insert(ReceiveDataModel data)
       {
           throw new NotImplementedException();
       }


       private string CombineWhereSql(ReceiveDataFilter filter)
       {
           StringBuilder sqlBuilder = new StringBuilder();
           sqlBuilder.Append(string.Format(" WH_CODE ='{0}' ", WareHouseCode));
           if (!string.IsNullOrEmpty(filter.MethodName))
           {
               sqlBuilder.Append(string.Format(" AND rd_methodname LIKE '%{0}%'", filter.MethodName));
           }
           if (!string.IsNullOrEmpty(filter.HandleStatus))
           {
               sqlBuilder.Append(string.Format(" AND dhstatus_id={0}", filter.HandleStatus));
           }
           if (!string.IsNullOrEmpty(filter.HandleFromTime) && !string.IsNullOrEmpty(filter.HandleToTime))
           {
               sqlBuilder.Append(string.Format(" AND  rd_handlerdate between  '{0}' and '{1}'", filter.HandleFromTime, filter.HandleToTime));
           }
           if (!string.IsNullOrEmpty(filter.ReceiveFromTime) && !string.IsNullOrEmpty(filter.ReceiveToTime))
           {
               sqlBuilder.Append(string.Format(" AND  rd_receivedate between  '{0}' and '{1}'", filter.ReceiveFromTime, filter.ReceiveToTime));
           }
           return sqlBuilder.ToString();
       }

       public override List<ReceiveDataModel> SelectData(ReceiveDataFilter filter)
       {
           List<ReceiveDataModel> dataPool = new List<ReceiveDataModel>();
           string where = CombineWhereSql(filter);
           string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY rd_receivedate DESC) AS rownumber,* FROM T_BO_ReceiveData WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filter.PageSize, filter.PageIndex, where);
           try
           {
               using (DataSet ds = DbHelper.GetDataSet(selectSql))
               {
                   if (ds != null && ds.Tables[0].Rows.Count > 0)
                   {
                       foreach (DataRow dr in ds.Tables[0].Rows)
                       {
                           ReceiveDataModel task = new ReceiveDataModel
                           {
                               HandleDateTime = ConvertHepler.ConvertToDateTime(dr["rd_handlerdate"].ToString()),
                               HandleMessage = dr["rd_handlermessage"].ToString(),
                               HandleStatus = (ReceiveDataHandleStatus)ConvertHepler.ConvertToInt(dr["dhstatus_id"].ToString()),
                               MethodName = dr["rd_methodname"].ToString(),
                               MethodParamValue = dr["rd_paramvalue"].ToString(),
                               Note = dr["rd_note"].ToString(),
                               ReceiveDateTime = ConvertHepler.ConvertToDateTime(dr["rd_receivedate"].ToString()),
                               Source = dr["rd_source"].ToString(),
                               WhCode = dr["wh_code"].ToString()
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


       public override long GetTotalCount()
       {
           string sql = string.Format(" SELECT count(1) as Num FROM T_BO_ReceiveData T WHERE T.WH_CODE='{0}'", WareHouseCode);
           string num = DbHelper.ExecuteScalar(sql);
           long count;
           long.TryParse(num, out count);
           return count;
       }
    }
}
