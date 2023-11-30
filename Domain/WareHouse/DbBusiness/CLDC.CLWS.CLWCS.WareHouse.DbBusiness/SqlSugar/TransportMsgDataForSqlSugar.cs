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
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TransportMsgDataForSqlSugar : TransportMsgDataAbstract
    {
        public TransportMsgDataForSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {

        }

        public override OperateResult<List<TransportMsgModel>> GetTransportData(int owner, List<TransportResultEnum> statusList)
        {
            //string statusIds = "-1";
            //foreach (TransportResultEnum status in statusList)
            //{
            //    statusIds += "," + (int)status;
            //}
            List<TransportMsgModel> transportMsgLst = DbHelper.QueryList<TransportMsgModel>(t => t.WhCode == WareHouseCode && t.OwnerId == owner && statusList.Contains(t.TransportStatus), "STATUS desc");
            if (transportMsgLst != null && transportMsgLst.Count > 0)
            {
                OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                return getResult;
            }
            return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' and OWNERID='{2}' and STATUS in ({1})  order by STATUS desc", WareHouseCode, statusIds, owner);
            //using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            //{
            //    List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
            //    if (transportMsgLst != null && transportMsgLst.Count > 0)
            //    {
            //        OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
            //        return getResult;
            //    }
            //    return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //}

        }

        public override OperateResult<List<TransportMsgModel>> GetTransportDataByOwner(int owner)
        {
            List<TransportMsgModel> transportMsgLst = DbHelper.QueryList<TransportMsgModel>(t => t.WhCode == WareHouseCode && t.OwnerId == owner, "STATUS desc");
            if (transportMsgLst != null && transportMsgLst.Count > 0)
            {
                OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                return getResult;
            }
            return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' and OWNERID='{1}'  order by STATUS desc", WareHouseCode, owner);
            //using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            //{
            //    List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
            //    if (transportMsgLst != null && transportMsgLst.Count > 0)
            //    {
            //        OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
            //        return getResult;
            //    }
            //    return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //}
           
        }

        public override OperateResult Insert(TransportMsgModel transportMsg)
        {
            transportMsg.WhCode = WareHouseCode;
            transportMsg.AddDateTime = DateTime.Now;
            bool insertResult = DbHelper.Add(transportMsg) > 0;
            //string insertSql = string.Format(@"INSERT INTO T_BO_TRANSPORTDATA (TRANSPORTID, STARTID, DESTID, EXORDERID, STARTADDR, CURADDR, DESTADDR, OWNERID, STATUS, WH_CODE,GUID,ADDDATETIME,PILENO,FINISHTYPE) 
            //                                        VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}',current_timestamp,'{11}','{12}')", transportMsg.TransportId, transportMsg.StartId, transportMsg.DestId, transportMsg.ExOrderId, transportMsg.StartAddr, transportMsg.CurAddr, transportMsg.DestAddr, transportMsg.OwnerId, (int)transportMsg.TransportStatus, WareHouseCode, transportMsg.Guid, transportMsg.PileNo, (int)transportMsg.TransportFinishType);
            //bool insertResult = DbHelper.ExecuteNonQuery(insertSql);
            if (insertResult)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult(string.Format("插入新数据失败! "), 1);
        }

        public override OperateResult IsExist(TransportMsgModel transportMsg)
        {
            bool isExists = DbHelper.IsExist<TransportMsgModel>(t => t.Guid == transportMsg.Guid);
            if (isExists)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult();
            //string existSql = string.Format("SELECT COUNT(*) FROM T_BO_TRANSPORTDATA WHERE GUID='{0}'", transportMsg.Guid);
            //using (DataSet ds = DbHelper.GetDataSet(existSql))
            //{
            //    if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    {
            //        if (ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString()).Equals(0))
            //        {
            //            ds.Dispose();
            //            return OperateResult.CreateFailedResult();
            //        }
            //        return OperateResult.CreateSuccessResult();
            //    }
            //    return OperateResult.CreateFailedResult();
            //}
        }

        public override OperateResult Update(TransportMsgModel transportMsg)
        {
            bool updateResult = DbHelper.Update<TransportMsgModel>(t => new TransportMsgModel
            {
                TransportId = transportMsg.TransportId,
                StartId = transportMsg.StartId,
                DestId = transportMsg.DestId,
                ExOrderId = transportMsg.ExOrderId,
                StartAddr = transportMsg.StartAddr,
                CurAddr = transportMsg.CurAddr,
                DestAddr = transportMsg.DestAddr,
                OwnerId = transportMsg.OwnerId,
                TransportStatus = transportMsg.TransportStatus,
                TransportFinishType = transportMsg.TransportFinishType,
                TrayType = transportMsg.TrayType,
            }, t => t.Guid == transportMsg.Guid) > 0;

            //string updateSql = string.Format(@"UPDATE T_BO_TRANSPORTDATA SET TRANSPORTID='{0}', STARTID='{1}', DESTID='{2}', EXORDERID='{3}', STARTADDR='{4}', CURADDR='{5}', DESTADDR='{6}', OWNERID='{7}', STATUS='{8}',FINISHTYPE='{10}',UPDATEDATETIME=current_timestamp WHERE GUID = '{9}'", 
            //    transportMsg.TransportId, transportMsg.StartId, transportMsg.DestId, transportMsg.ExOrderId, 
            //    transportMsg.StartAddr, transportMsg.CurAddr, transportMsg.DestAddr, transportMsg.OwnerId, 
            //    (int)transportMsg.TransportStatus, transportMsg.Guid, (int)transportMsg.TransportFinishType);
            //bool updateResult = DbHelper.ExecuteNonQuery(updateSql);
            if (updateResult)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult(string.Format("更新数据失败! "), 1);
        }
        public override OperateResult<List<TransportMsgModel>> GetTransportData(int pageIndex,int pageSize,string orderBy,Expression<Func<TransportMsgModel, bool>> whereLambda,out int totalCount)
        {
            List<TransportMsgModel> transportMsgLst = DbHelper.QueryPageList(pageIndex,pageSize, orderBy, out totalCount,whereLambda);
            if (transportMsgLst != null && transportMsgLst.Count > 0)
            {
                OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                return getResult;
            }
            return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //using (DataSet ds = DbHelper.GetDataSet(where))
            //{
            //    List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
            //    if (transportMsgLst != null && transportMsgLst.Count > 0)
            //    {
            //        OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
            //        return getResult;
            //    }
            //    return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //}
        }

        public override OperateResult<List<TransportMsgModel>> GetTransportDataByStatus(List<TransportResultEnum> statusList)
        {
            //string statusIds = "-1";
            //foreach (TransportResultEnum status in statusList)
            //{
            //    statusIds += "," + (int)status;
            //}
            List<TransportMsgModel> transportMsgLst = DbHelper.QueryList<TransportMsgModel>(t=>t.WhCode==WareHouseCode && statusList.Contains(t.TransportStatus), "STATUS desc");
            if (transportMsgLst != null && transportMsgLst.Count > 0)
            {
                OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
                return getResult;
            }
            return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' and STATUS in ({1})  order by STATUS desc", WareHouseCode, statusIds);
            //using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            //{
            //    List<TransportMsgModel> transportMsgLst = ChangeToTransportMsgMode(ds);
            //    if (transportMsgLst != null && transportMsgLst.Count > 0)
            //    {
            //        OperateResult<List<TransportMsgModel>> getResult = OperateResult.CreateSuccessResult(transportMsgLst);
            //        return getResult;
            //    }
            //    return OperateResult.CreateFailedResult<List<TransportMsgModel>>(null, "不存在数据");
            //}

        }


    }
}
