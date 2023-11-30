using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;


namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public class OrderDataForSqlServer : OrderDataAbstract
    {
        public OrderDataForSqlServer(IDbHelper dbhelper)
        {
            base.DbHelper = dbhelper;
        }

        public override bool SaveOrder(ExOrder order)
        {
            if (IsExistOrderById(order))
            {
                return UpdateOrder(order);
            }
            return InsertOrder(order);
        }

        public override void SaveOrderAsync(ExOrder order)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (IsExistOrderById(order))
                {
                    UpdateOrder(order);
                }
                InsertOrder(order);
            });
        }



        private bool IsExistOrderById(ExOrder order)
        {
            string sql = string.Format("select ORDERID from T_BO_ORDEROPERATE WHERE ORDERID='{0}' and WH_CODE='{1}'", order.OrderId, WareHouseCode);
            using (DataSet ds = DbHelper.GetDataSet(sql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                return false;  
            }
           
        }

        private bool InsertOrder(ExOrder order)
        {
            int priorRGVNo = -1;
            if (null != order.PriorDeviceList && order.PriorDeviceList.Count > 0)
            {
                foreach (DeviceName device in order.PriorDeviceList)
                {
                    if (device.Type == "RGV")
                    {
                        priorRGVNo = device.Number;
                    }
                }
            }

            ArrayList sqlList = new ArrayList();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into T_BO_ORDEROPERATE(");
            strSql.Append("ORDERID,WH_CODE,PILE_NO,START_ADDR,CURRENT_ADDR,DESTINATION_ADD,NEXT_ADDR,ORDERTYPE,ADD_TIME,STATUS,PRIOR_RGV_NO,PRIORITY,DOCUMENTCODE,BACKFLAG,CURRENT_DEVICE,FINISH_TYPE,SOURCE,ISREPORT,SOURCETASKTYPE,QTY,GOODS_COUNT,GOODS_TYPE)");
            strSql.Append(" values (");
            string sql = strSql.Append(string.Format("{0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',current_timestamp,{9},{10},{11},'{12}',{13},'{14}',{15},'{16}','{17}',{18},{19},{20},{21})",
                order.OrderId, WareHouseCode, order.PileNo, order.StartAddr!=null?order.StartAddr.FullName:string.Empty, order.CurrAddr!=null?order.CurrAddr.FullName:string.Empty, order.DestAddr!=null?order.DestAddr.FullName:string.Empty, order.NextAddr!=null?order.NextAddr.FullName:string.Empty, order.OrderType.ToString(),
                DateTime.Now, ((int)order.Status).ToString(), priorRGVNo, order.OrderPriority, order.DocumentCode, order.BackFlag, order.CurHandlerId, (int)order.FinishType, order.Source.ToString(), order.IsReport, (int)order.SourceTaskType,order.Qty,order.GoodsCount,order.GoodsType)).ToString();
            sqlList.Add(sql);

            if (SystemConfig.Instance.IsUseCellPile)
            {
                if ((order.OrderType == OrderTypeEnum.Out || order.OrderType == OrderTypeEnum.Move) && order.StartAddr.Type == "Cell")
                {
                    string sql2 = string.Format(@"Update t_bd_cell set CS_CODE={0} where CELLNO='{1}'", 8, order.StartAddr!=null?order.StartAddr.FullName:string.Empty);
                    sqlList.Add(sql2);
                }
                if ((order.OrderType == OrderTypeEnum.In || order.OrderType == OrderTypeEnum.Move) && order.DestAddr.Type == "Cell")
                {
                    string sql3 = string.Format(@"Update t_bd_cell set CS_CODE={0} where CELLNO='{1}'", 8, order.DestAddr!=null?order.DestAddr.FullName:string.Empty);
                    sqlList.Add(sql3);
                }
            }
            return DbHelper.ExecuteNonQuery(sqlList) > 0;
        }

        private bool UpdateOrder(ExOrder order)
        {
            int priorRGVNo = -1;
            if (null != order.PriorDeviceList && order.PriorDeviceList.Count > 0)
            {
                foreach (DeviceName device in order.PriorDeviceList)
                {
                    if (device.Type == "RGV")
                    {
                        priorRGVNo = device.Number;
                    }
                }
            }
            string sqlstr = string.Format(@"UPDATE T_BO_ORDEROPERATE  SET PILE_NO='{0}',START_ADDR='{1}',CURRENT_ADDR='{2}',DESTINATION_ADD='{3}',ORDERTYPE='{4}',STATUS={5},PRIOR_RGV_NO={6},PRIORITY={7},DOCUMENTCODE='{8}',BACKFLAG={9},NEXT_ADDR='{10}',CURRENT_DEVICE='{11}',FINISH_TYPE={12},SOURCE='{13}',ISREPORT='{14}'  WHERE ORDERID={15} and WH_CODE='{16}' ",
                order.PileNo, order.StartAddr!=null?order.StartAddr.FullName:string.Empty, order.CurrAddr!=null?order.CurrAddr.FullName:string.Empty, order.DestAddr!=null?order.DestAddr.FullName:string.Empty, order.OrderType.ToString(), (int)order.Status, priorRGVNo, order.OrderPriority, order.DocumentCode, order.BackFlag, order.NextAddr!=null?order.NextAddr.FullName:string.Empty, order.CurHandlerId, (int)order.FinishType, order.Source.ToString(), order.IsReport, order.OrderId, WareHouseCode);

            return DbHelper.ExecuteNonQuery(sqlstr);
        }

        public override int GetMaxIDExist(string fieldName, string tableName, string where)
        {
            string strsql = "select ISNULL(max(" + fieldName + "),0) from " + tableName + " " + where;
            using (DataSet ds = DbHelper.GetDataSet(strsql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString());
                }
                return -0x7FFFFFFF;
            }
           
        }

        public override int GetMinIDExist(string fieldName, string tableName, string where)
        {
            string strsql = "select ISNULL(min(" + fieldName + "),0) from " + tableName + " " + where;
            using (DataSet ds = DbHelper.GetDataSet(strsql))
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ConvertHepler.ConvertToInt(ds.Tables[0].Rows[0][0].ToString());
                }
                return -0x7FFFFFFF;
            }
        
        }

        public override List<ExOrder> GetUnfinishedOrders()
        {
            string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where STATUS !={0} and  WH_CODE ='{1}' order by STATUS desc", (int)StatusEnum.CmdSent, WareHouseCode);
            using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            {
                List<ExOrder> orderList = ChangeToExOrder(ds);
                if (orderList != null && orderList.Count > 0)
                {
                    return orderList;
                }
                return null;
            }
           
        }

        public override List<ExOrder> GetTransportedOrders()
        {
            string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where STATUS ={0} and  WH_CODE ='{1}' order by STATUS desc", (int)StatusEnum.TransportCompleted, WareHouseCode);
            using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            {
                List<ExOrder> orderList = ChangeToExOrder(ds);
                if (orderList != null && orderList.Count > 0)
                {
                    return orderList;
                }
                return null;   
            }
           
        }

        public override List<ExOrder> GetUnifinishedOrders(int workerId, List<StatusEnum> statusList)
        {
            string statusIds = "-1";
            foreach (StatusEnum status in statusList)
            {
                statusIds += "," + (int)status;
            }

            string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where  WH_CODE ='{0}' and CURRENT_DEVICE='{2}' and STATUS in ({1})  order by STATUS desc", WareHouseCode, statusIds, workerId);
            using (DataSet ds = DbHelper.GetDataSet(sqlstr))
            {
                List<ExOrder> orderList = ChangeToExOrder(ds);
                if (orderList != null && orderList.Count > 0)
                {
                    return orderList;
                }
                return null;
            }
            
        }
    }
}
