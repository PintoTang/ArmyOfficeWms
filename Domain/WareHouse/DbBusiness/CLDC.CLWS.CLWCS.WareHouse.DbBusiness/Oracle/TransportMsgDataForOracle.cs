using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.DataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TransportMsgDataForOracle : TransportMsgDataAbstract
    {
        public TransportMsgDataForOracle(IDbHelper dbHelper)
            : base(dbHelper)
        {

        }

        public override OperateResult<List<TransportMsgModel>> GetTransportData(int ownerId, List<TransportResultEnum> statusList)
        {
            string statusIds = "-1";
            foreach (TransportResultEnum status in statusList)
            {
                statusIds += "," + (int)status;
            }
            string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' and OWNERID='{2}' and STATUS in ({1})  order by STATUS desc", WareHouseCode, statusIds, ownerId);
            using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            {
                List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
                if (transportMsgLst != null && transportMsgLst.Count > 0)
                {
                    OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                    return getResult;
                }
                return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            }
           
        }

        public override OperateResult<List<TransportMsgModel>> GetTransportDataByOwner(int ownerId)
        {
            string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' and OWNERID='{1}'  order by STATUS desc", WareHouseCode, ownerId);
            using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            {
                List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
                if (transportMsgLst != null && transportMsgLst.Count > 0)
                {
                    OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                    return getResult;
                }
                return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            }
        }

        public override OperateResult Insert(TransportMsgModel transportMsg)
        {
            string insertSql = string.Format(@"INSERT INTO T_BO_TRANSPORTDATA (TRANSPORTID, STARTID, DESTID, EXORDERID, STARTADDR, CURADDR, DESTADDR, OWNERID, STATUS, WH_CODE,GUID,ADDDATETIME,PILENO,FINISHTYPE) 
                                                    VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}',current_timestamp,'{11}','{12}')", transportMsg.TransportId, transportMsg.StartId, transportMsg.DestId, transportMsg.ExOrderId, transportMsg.StartAddr, transportMsg.CurAddr, transportMsg.DestAddr, transportMsg.OwnerId, (int)transportMsg.TransportStatus, WareHouseCode, transportMsg.Guid, transportMsg.PileNo, (int)transportMsg.TransportFinishType);
            bool insertResult = DbHelper.ExecuteNonQuery(insertSql);
            if (insertResult)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult(string.Format("插入新数据失败! \r\n 插入的语句：{0}", insertSql), 1);
        }

        public override OperateResult IsExist(TransportMsgModel transportMsg)
        {
            string existSql = string.Format("SELECT COUNT(*) FROM T_BO_TRANSPORTDATA WHERE GUID='{0}'", transportMsg.Guid);
            DataSet ds = DbHelper.GetDataSet(existSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString()).Equals(0))
                {
                    return OperateResult.CreateFailedResult();
                }
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult Update(TransportMsgModel transportMsg)
        {
            string updateSql = string.Format(@"UPDATE T_BO_TRANSPORTDATA SET TRANSPORTID='{0}', STARTID='{1}', DESTID='{2}', EXORDERID='{3}', STARTADDR='{4}', CURADDR='{5}', DESTADDR='{6}', OWNERID='{7}', STATUS='{8}',FINISHTYPE='{10}',UPDATEDATETIME=current_timestamp WHERE GUID = '{9}'", transportMsg.TransportId, transportMsg.StartId, transportMsg.DestId, transportMsg.ExOrderId, transportMsg.StartAddr, transportMsg.CurAddr, transportMsg.DestAddr, transportMsg.OwnerId, (int)transportMsg.TransportStatus, transportMsg.Guid, (int)transportMsg.TransportFinishType);
            bool updateResult = DbHelper.ExecuteNonQuery(updateSql);
            if (updateResult)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult(string.Format("更新数据失败! \r\n 更新的语句：{0}", updateSql), 1);
        }
        public override OperateResult<List<TransportMsgModel>> GetTransportData(string where)
        {
            using (DataSet ds = DbHelper.GetDataSet(where))
            {
                List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
                if (transportMsgLst != null && transportMsgLst.Count > 0)
                {
                    OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                    return getResult;
                }
                return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            }
          
        }

        public override OperateResult<List<TransportMsgModel>> GetTransportDataByStatus(List<TransportResultEnum> statusList)
        {
            string statusIds = "-1";
            foreach (TransportResultEnum status in statusList)
            {
                statusIds += "," + (int)status;
            }
            string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}'  and STATUS in ({1})  order by STATUS desc", WareHouseCode, statusIds);
            using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            {
                List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
                if (transportMsgLst != null && transportMsgLst.Count > 0)
                {
                    OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                    return getResult;
                }
                return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            }
           
        }

        private List<TransportMsgModel> ChangeToTransportMsgMode(DataSet ds)
        {
            List<TransportMsgModel> transportMsgModeLst = new List<TransportMsgModel>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        TransportMsgModel msgMode = new TransportMsgModel();
                        msgMode.DestId = ConvertHepler.ConvertToInt(dr["DESTID"].ToString());
                        msgMode.StartId = ConvertHepler.ConvertToInt(dr["STARTID"].ToString());
                        msgMode.TransportId = ConvertHepler.ConvertToInt(dr["TRANSPORTID"].ToString());
                        msgMode.ExOrderId = ConvertHepler.ConvertToInt(dr["EXORDERID"].ToString());
                        msgMode.TransportStatus = (TransportResultEnum)ConvertHepler.ConvertToInt(dr["STATUS"].ToString());
                        msgMode.OwnerId = ConvertHepler.ConvertToInt(dr["OWNERID"].ToString());
                        msgMode.CurAddr = Convert.ToString(dr["CURADDR"]);
                        msgMode.DestAddr = Convert.ToString(dr["DESTADDR"]);
                        msgMode.StartAddr = Convert.ToString(dr["STARTADDR"]);
                        msgMode.Guid = Convert.ToString(dr["GUID"]);
                        msgMode.PileNo = dr["PILENO"].ToString();
                        //msgMode.AddDateTime = Convert.ToDateTime(dr["ADDDATETIME"]);
                        //msgMode.UpdateDateTime = Convert.ToDateTime(dr["UPDATEDATETIME"]);
                        msgMode.TransportFinishType = (FinishType)ConvertHepler.ConvertToInt(dr["FINISHTYPE"].ToString());
                        transportMsgModeLst.Add(msgMode);
                    }
                    catch (Exception ex)
                    {
                        Log.getExceptionFile().Info(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
            return transportMsgModeLst;
        }

     

    }
}
