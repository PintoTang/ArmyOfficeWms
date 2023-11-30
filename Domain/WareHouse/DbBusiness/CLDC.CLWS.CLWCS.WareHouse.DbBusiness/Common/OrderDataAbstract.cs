using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CL.WCS.SystemConfigPckg;
using CLDC.Framework.Log;
using System.Reflection;
using System.ComponentModel;
using CL.WCS.SystemConfigPckg.Model;
using System.Linq.Expressions;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
	public abstract class OrderDataAbstract
	{
		protected string WareHouseCode = SystemConfig.Instance.WhCode;
		private IDbHelper dbHelper;
		public IDbHelper DbHelper
		{
			get { return dbHelper; }
			set { dbHelper = value; }
		}

		/// <summary>
		/// 保存订单信息
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public abstract bool SaveOrder(ExOrder order);
        /// <summary>
        /// 异步保存指令信息
        /// </summary>
        /// <param name="order"></param>
	    public abstract void SaveOrderAsync(ExOrder order);

		/// <summary>
		/// 根据指令Id获取相关信息
		/// </summary>
		/// <param name="orderIdList"></param>
		/// <returns></returns>
		public List<ExOrder> GetExOrderListByOrderId(List<int> orderIdList)
		{
			//string orderIds = "-1";
			//foreach (int orderId in orderIdList)
			//{
			//	orderIds += "," + orderId;
			//}
			//string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where  WH_CODE ='{0}' and ORDERID in ({1})  order by STATUS desc", WareHouseCode, orderIds);

            List<ExOrder> orderList = DbHelper.QueryList<ExOrder>(t =>t.WhCode==WareHouseCode && orderIdList.Contains(t.OrderId), "STATUS desc");//ChangeToExOrder(dbHelper.GetDataSet(sqlstr));
            orderList = ChangeToExOrder(orderList);
            if (orderList != null && orderList.Count > 0)
			{
				return orderList;
			}
			return null;
		}

        /// <summary>
        /// 根据指令Id获取相关信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ExOrder GetExOrderByOrderId(int orderId)
        {
            ExOrder item = DbHelper.Query<ExOrder>(t => t.WhCode == WareHouseCode && t.OrderId==orderId);//ChangeToExOrder(dbHelper.GetDataSet(sqlstr));
            return ChangeToExorder(item);
           // string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where  WH_CODE ='{0}' and ORDERID = {1}", WareHouseCode, orderId);
           //DataSet ds= dbHelper.GetDataSet(sqlstr);
           // if (ds.Tables[0].Rows.Count > 0)
           // {
           //     DataRow dr = ds.Tables[0].Rows[0];
           //     ExOrder order = ChangeToExorder(dr);
           //     return order;
           // }
            //return null;
        }
        public List<ExOrder> GetHisData(int pageIndex,int pageSize,string S_OrderID, string S_PackNum, string S_CurAddress, string S_NextAddress,
                                        string S_OrderStatu, string S_OrderType, string s_OrderAddStartTime, string s_OrderAddEndTime, out int totalCount)
        {
            Expression<Func<ExOrder, bool>> whereLambda = t => t.WhCode == WareHouseCode;

            if (!string.IsNullOrEmpty(S_OrderID))
            {
                whereLambda = whereLambda.AndAlso(t => t.OrderId == ConvertHepler.ConvertToInt(S_OrderID));
            }
            if (!string.IsNullOrEmpty(S_PackNum))
            {
                whereLambda = whereLambda.AndAlso(t => t.PileNo.Contains(S_PackNum));
            }

            if (!string.IsNullOrEmpty(S_CurAddress))
            {
                whereLambda = whereLambda.AndAlso(t => t.CurrAddrName.Contains(S_CurAddress));
            }
            if (!string.IsNullOrEmpty(S_NextAddress))
            {
                whereLambda = whereLambda.AndAlso(t => t.NextAddrName.Contains(S_NextAddress));
            }
            if (!string.IsNullOrEmpty(S_OrderStatu))
            {
                var status=GetStatusEnumByDescription(S_OrderStatu);
                if (status != null)
                {
                    whereLambda = whereLambda.AndAlso(t => t.Status == status);
                }
            }
            if (!string.IsNullOrEmpty(S_OrderType))
            {
                whereLambda = whereLambda.AndAlso(t => t.OrderTypeName == S_OrderType);
            }
            if (!string.IsNullOrEmpty(s_OrderAddStartTime) && !string.IsNullOrEmpty(s_OrderAddEndTime))
            {
                DateTime startDate = Convert.ToDateTime(s_OrderAddStartTime);
                DateTime endDate = Convert.ToDateTime(s_OrderAddEndTime);
                //sqlstr += string.Format(" and ADD_TIME between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);//oracle
                //sqlstr += string.Format(" and ADD_TIME between  '{0}' and  '{1}'", startDate, endDate);

                whereLambda = whereLambda.AndAlso(t => t.CreateTime>=startDate && t.CreateTime<=endDate);
            }
            var dbList = dbHelper.QueryPageList(pageIndex,pageSize, "OrderID DESC", out totalCount,whereLambda);
            List<ExOrder> orderList = ChangeToExOrder(dbList);
            if (orderList != null && orderList.Count > 0)
            {
                return orderList;
            }
            return null;
        }

        public List<ExOrder> GetTransportData(string S_OrderID, string S_PackNum, string S_CurAddress, string S_NextAddress,
                                      string S_OrderStatu, string S_OrderType, string s_OrderAddStartTime, string s_OrderAddEndTime)
        {
            string sqlstr = string.Format("select * from T_BO_TRANSPORTDATA where  WH_CODE ='{0}' ", WareHouseCode);
            if (!string.IsNullOrEmpty(S_OrderID))
            {
                sqlstr += " and TRANSPORTID='" + ConvertHepler.ConvertToInt(S_OrderID) + "'";
            }
            if (!string.IsNullOrEmpty(S_PackNum))
            {
                sqlstr += " and PILE_NO like'" + "%" + S_PackNum + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_CurAddress))
            {
                sqlstr += " and CURRENT_ADDR like'" + "%" + S_CurAddress + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_NextAddress))
            {
                sqlstr += " and NEXT_ADDR like'" + "%" + S_NextAddress + "%" + "'";
            }
            if (!string.IsNullOrEmpty(S_OrderStatu))
            {
                sqlstr += GetStatusEnumByDescription(S_OrderStatu);
            }
            if (!string.IsNullOrEmpty(S_OrderType))
            {
                sqlstr += GetOrderTypeEnumByDescription(S_OrderType);
            }
            if (!string.IsNullOrEmpty(s_OrderAddStartTime) && !string.IsNullOrEmpty(s_OrderAddEndTime))
            {
                DateTime startDate = Convert.ToDateTime(s_OrderAddStartTime);
                DateTime endDate = Convert.ToDateTime(s_OrderAddEndTime);
                //sqlstr += string.Format(" and ADD_TIME between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);//oracle
                sqlstr += string.Format(" and ADD_TIME between  '{0}' and '{1}'", startDate, endDate);
            }
            List<ExOrder> orderList = ChangeToTranSportExOrder(dbHelper.QueryDataTable(sqlstr));
            if (orderList != null && orderList.Count > 0)
            {
                return orderList;
            }
            return null;
        }


        private StatusEnum? GetStatusEnumByDescription(string strDescription)
        {
            //string strSql = "";
            var memberInfos = typeof(StatusEnum).GetMembers().Where(x => x.DeclaringType.ToString().Contains("StatusEnum"));
            foreach (var member in memberInfos)
            {
                var attrs = member.GetCustomAttributes();
                if (attrs != null && attrs.Count() > 0)
                {
                    DescriptionAttribute attr = (DescriptionAttribute)(member.GetCustomAttribute(typeof(DescriptionAttribute), false));
                    if (attr.Description.Contains(strDescription))
                    {
                        StatusEnum orderTypeEnum = (StatusEnum)Enum.Parse(typeof(StatusEnum), member.Name);
                        //int typeValue = (int)orderTypeEnum;
                        //strSql += " and STATUS like'" + "%" + typeValue.ToString() + "%" + "'";
                        return orderTypeEnum;
                    }
                }
            }
            return null;
        }

        private string GetOrderTypeEnumByDescription(string strOrderTypeName)
        {
            string strSql = "";
            MemberInfo[] memberInfos = (typeof(OrderTypeEnum)).GetMembers();
            try
            {
                OrderTypeEnum orderTypeEnum = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), strOrderTypeName);
                strSql += " and ORDERTYPE like'" + "%" + orderTypeEnum.ToString() + "%" + "'";
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
            return strSql;
        }
		/// <summary>
		/// 根据指令状态获取相关信息
		/// </summary>
		/// <param name="statusList">指令列表</param>
		/// <returns></returns>
		public List<ExOrder> GetDataByStatus(List<StatusEnum> statusList)
		{
			//string statusIds = "-1";
			//foreach (StatusEnum status in statusList)
			//{
			//	statusIds += "," + (int)status;
			//}
            List<ExOrder> orderList = dbHelper.QueryList<ExOrder>(t => t.WhCode == WareHouseCode && statusList.Contains(t.Status), "STATUS desc");

   //         string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where  WH_CODE ='{0}' and STATUS in ({1})  order by STATUS desc", WareHouseCode, statusIds);
			//List<ExOrder> orderList = ChangeToExOrder(dbHelper.QueryDataTable(sqlstr));
			//if (orderList != null && orderList.Count > 0)
			//{
			//	return orderList;
			//}
			return ChangeToExOrder(orderList);
		}

	    /// <summary>
	    /// 根据当前执行的设备和状态获取指令
	    /// </summary>
	    /// <param name="workerId"></param>
	    /// <param name="statusList"></param>
	    /// <returns></returns>
	    public abstract List<ExOrder> GetUnifinishedOrders(int workerId, List<StatusEnum> statusList);

		/// <summary>
		/// 获取未完成的指令列表
		/// </summary>
		/// <returns></returns>
		public abstract List<ExOrder> GetUnfinishedOrders();

		/// <summary>
		/// 获取已搬运完成的指令列表
		/// </summary>
		/// <returns></returns>
		public abstract List<ExOrder> GetTransportedOrders();

		public List<ExOrder> SelectOrder(int orderId, string pileNo, int? satatus = null)
		{
            Expression<Func<ExOrder, bool>> whereLambda = t => t.WhCode == WareHouseCode;
            //string sqlstr = string.Format("select * from T_BO_ORDEROPERATE where  WH_CODE ='{0}'", WareHouseCode);
			if (orderId != 0)
			{
                whereLambda = whereLambda.AndAlso(t => t.OrderId == orderId);
                //sqlstr += " and ORDERID='" + orderId + "'";
			}
			if (!string.IsNullOrEmpty(pileNo))
            {
                whereLambda = whereLambda.AndAlso(t => t.PileNo == pileNo);
                //sqlstr += " and Pile_NO='" + pileNo + "'";
			}
			if (satatus != null)
            {
                whereLambda = whereLambda.AndAlso(t => t.Status == (StatusEnum)satatus.GetValueOrDefault());
                //sqlstr += " and Status='" + satatus + "'";
			}
			List<ExOrder> orderList = ChangeToExOrder(dbHelper.QueryList(whereLambda));
			if (orderList != null && orderList.Count > 0)
			{
				return orderList;
			}
			return null;
		}

		/// <summary>
		/// 取指令表中最大的指令ID
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="tableName"></param>
		/// <param name="where"></param>
		/// <returns></returns>
		public abstract int GetMaxIDExist(string fieldName, string tableName, string where);

		/// <summary>
		/// 取指令表中最小的指令ID
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="tableName"></param>
		/// <param name="where"></param>
		/// <returns></returns>
		public abstract int GetMinIDExist(string fieldName, string tableName, string where);

		/// <summary>
		/// 更改指令优先级
		/// </summary>
		/// <param name="Priority"></param>
		/// <param name="barcode"></param>
		/// <returns></returns>
		public bool UpdatePriority(int Priority, string barcode)
		{
            bool result = DbHelper.Update<ExOrder>(t => new ExOrder
            {
                OrderPriority=Priority,
            }, t => t.PileNo == barcode && t.Status == StatusEnum.Processing)>0;
			//string sqlstr = string.Format(@"UPDATE T_BO_ORDEROPERATE  SET Priority={0} where PILE_NO='{1}' and status = 1", Priority, barcode);
			return result;
		}

		/// <summary>
		/// 根据垛号和状态查询指令
		/// </summary>
		/// <param name="pileNo"></param>
		/// <param name="satatus"></param>
		/// <returns></returns>
		public bool IsExistOrderByPileNoAndStatus(string pileNo, int satatus)
		{
            bool isExists=DbHelper.IsExist<ExOrder>(t => t.PileNo == pileNo && t.Status == (StatusEnum)satatus);
            return isExists;
			//StringBuilder strSQL = new StringBuilder("select * from T_BO_ORDEROPERATE  ");
			//strSQL.Append(" where Pile_NO='" + pileNo + "' and Status=" + satatus);
			//if (string.IsNullOrEmpty(dbHelper.ExecuteScalar(strSQL.ToString())))
			//{
			//	return false;
			//}
			//return true;
		}

     //   private ExOrder ChangeToExorder(DataRow dr)
	    //{
     //       try
     //       {
     //          ExOrder order = new ExOrder();
     //           order.OrderId = Convert.ToInt32(dr["ORDERID"]);
     //           order.PileNo = Convert.ToString(dr["PILE_NO"]);
     //           if (dr["START_ADDR"].ToString() != "")
     //           {
     //               order.StartAddr = new Addr(Convert.ToString(dr["START_ADDR"]));
     //           }
     //           if (dr["CURRENT_ADDR"].ToString() != "")
     //           {
     //               order.CurrAddr = new Addr(Convert.ToString(dr["CURRENT_ADDR"]));
     //           }
     //           if (dr["DESTINATION_ADD"].ToString() != "")
     //           {
     //               order.DestAddr = new Addr(Convert.ToString(dr["DESTINATION_ADD"]));
     //           }

     //           if (dr["NEXT_ADDR"].ToString() != "")
     //           {
     //               order.NextAddr = new Addr(Convert.ToString(dr["NEXT_ADDR"]));
     //           }

     //           if (dr["SOURCETASKTYPE"].ToString() != null)
     //           {
     //               order.SourceTaskType = (SourceTaskEnum)Enum.Parse(typeof(SourceTaskEnum), dr["SOURCETASKTYPE"].ToString().Trim());
     //           }

     //           order.OrderType = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), dr["ORDERTYPE"].ToString().Trim());
     //           order.Status = (StatusEnum)Convert.ToInt32(dr["STATUS"]);
     //           int priorRGVNo = Convert.ToInt32(dr["PRIOR_RGV_NO"]);
     //           if (-1 != priorRGVNo)
     //           {
     //               DeviceName deviceName = new DeviceName("RGV", priorRGVNo);
     //               order.PriorDeviceList = new List<DeviceName>() { deviceName };
     //           }
     //           order.OrderPriority = Convert.ToInt32(dr["PRIORITY"]);
     //           order.DocumentCode = Convert.ToString(dr["DOCUMENTCODE"]);
     //           order.BackFlag = Convert.ToInt32(dr["BACKFLAG"]);
     //           order.CreateTime = Convert.ToDateTime(dr["ADD_TIME"]);
     //           order.CurHandlerId = Convert.ToInt32(dr["CURRENT_DEVICE"].ToString());
     //           order.Source = (TaskSourceEnum)Enum.Parse(typeof(TaskSourceEnum), dr["SOURCE"].ToString().Trim());
     //           order.IsReport = Convert.ToBoolean(dr["ISREPORT"].ToString());
     //           order.GoodsType = ConvertHepler.ConvertToInt(dr["GOODS_TYPE"].ToString());
     //           order.GoodsCount = ConvertHepler.ConvertToInt(dr["GOODS_COUNT"].ToString());
     //           order.Qty = ConvertHepler.ConvertToInt(dr["QTY"].ToString());
     //           if (!string.IsNullOrEmpty(dr["FINISH_TYPE"].ToString()))
     //           {
     //               order.FinishType = (FinishType)Enum.Parse(typeof(FinishType), dr["FINISH_TYPE"].ToString().Trim());
     //           }
     //           return order;

     //       }
     //       catch (Exception ex)
     //       {
     //           Log.getExceptionFile().Info(ex);
     //       }
     //       return null;
	    //}

        private ExOrder ChangeToExorder(ExOrder dbItem)
        {
            try
            {
                if (dbItem == null)
                {
                    return null;
                }
                ExOrder order = new ExOrder();
                order.OrderId = dbItem.OrderId;
                order.PileNo = dbItem.PileNo;
                if (!string.IsNullOrEmpty(dbItem.StartAddrName))
                {
                    order.StartAddr = new Addr(dbItem.StartAddrName);
                }
                if (!string.IsNullOrEmpty(dbItem.CurrAddrName))
                {
                    order.CurrAddr = new Addr(dbItem.CurrAddrName);
                }
                if (!string.IsNullOrEmpty(dbItem.DestAddrName))
                {
                    order.DestAddr = new Addr(dbItem.DestAddrName);
                }

                if (!string.IsNullOrEmpty(dbItem.NextAddrName))
                {
                    order.NextAddr = new Addr(dbItem.NextAddrName);
                }
                if (dbItem.SourceTaskType!=null)
                {
                    order.SourceTaskType = dbItem.SourceTaskType;
                }

                order.OrderType = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum),dbItem.OrderTypeName);
                order.Status = dbItem.Status;
                int priorRGVNo = dbItem.HelperRGVNo;
                if (priorRGVNo>0)
                {
                    DeviceName deviceName = new DeviceName("RGV", priorRGVNo);
                    order.PriorDeviceList = new List<DeviceName>() { deviceName };
                }
                order.OrderPriority = dbItem.OrderPriority;
                order.DocumentCode = dbItem.DocumentCode;
                order.BackFlag = dbItem.BackFlag;
                order.CreateTime = dbItem.CreateTime;
                order.CurHandlerId = dbItem.CurHandlerId;
                order.Source = (TaskSourceEnum)Enum.Parse(typeof(TaskSourceEnum), dbItem.SourceName);
                order.IsReport = dbItem.IsReport;
                order.GoodsType = dbItem.GoodsType;
                order.GoodsCount = dbItem.GoodsCount;
                order.Qty = dbItem.Qty;
                if (dbItem.FinishType!=null)
                {
                    order.FinishType = dbItem.FinishType;
                }
                return order;

            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
            return null;
        }

        protected List<ExOrder> ChangeToExOrder(List<ExOrder> dbList)
        {
            List<ExOrder> orderList = null;

            if (dbList != null && dbList.Count>0)
            {
                orderList = dbList.Select(ChangeToExorder).Where(order => order != null).ToList();
            }
            return orderList;
        }

        //protected List<ExOrder> ChangeToExOrder(DataSet ds)
        //{
        //    List<ExOrder> orderList = null;

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        orderList = ds.Tables[0].Rows.Cast<DataRow>().Select(ChangeToExorder).Where(order => order != null).ToList();
        //    }
        //    return orderList;
        //}
		protected List<ExOrder> ChangeToTranSportExOrder(DataTable dt)
		{
			List<ExOrder> orderList = null;
			ExOrder order = null;
			if (dt.Rows.Count > 0)
			{
				orderList = new List<ExOrder>();

				foreach (DataRow dr in dt.Rows)
				{
					try
					{
						order = new ExOrder();
                        order.OrderId = Convert.ToInt32(dr["TRANSPORTID"]);
						order.PileNo = Convert.ToString(dr["PILE_NO"]);
						if (dr["START_ADDR"].ToString() != "")
						{
                            order.StartAddrName = Convert.ToString(dr["START_ADDR"]);
                            order.StartAddr = new Addr(Convert.ToString(dr["START_ADDR"]));
						}
						if (dr["CURRENT_ADDR"].ToString() != "")
                        {
                            order.CurrAddrName = Convert.ToString(dr["CURRENT_ADDR"]);
                            order.CurrAddr = new Addr(Convert.ToString(dr["CURRENT_ADDR"]));
						}
						if (dr["DESTINATION_ADD"].ToString() != "")
                        {
                            order.DestAddrName = Convert.ToString(dr["DESTINATION_ADD"]);
                            order.DestAddr = new Addr(Convert.ToString(dr["DESTINATION_ADD"]));
						}

						if (dr["NEXT_ADDR"].ToString() != "")
                        {
                            order.NextAddrName = Convert.ToString(dr["NEXT_ADDR"]);
                            order.NextAddr = new Addr(Convert.ToString(dr["NEXT_ADDR"]));
						}

						if (dr["SOURCETASKTYPE"].ToString()!=null)
						{
							order.SourceTaskType = (SourceTaskEnum)Enum.Parse(typeof(SourceTaskEnum), dr["SOURCETASKTYPE"].ToString().Trim());
						}

						order.OrderType = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), dr["ORDERTYPE"].ToString().Trim());
						order.Status = (StatusEnum)Convert.ToInt32(dr["STATUS"]);
						int priorRGVNo = Convert.ToInt32(dr["PRIOR_RGV_NO"]);
						if (-1 != priorRGVNo)
						{
							DeviceName deviceName = new DeviceName("RGV", priorRGVNo);
							order.PriorDeviceList = new List<DeviceName>() { deviceName };
						}
						order.OrderPriority = Convert.ToInt32(dr["PRIORITY"]);
						order.DocumentCode = Convert.ToString(dr["DOCUMENTCODE"]);
						order.BackFlag = Convert.ToInt32(dr["BACKFLAG"]);
						order.CreateTime = Convert.ToDateTime(dr["ADD_TIME"]);
						order.CurHandlerId = Convert.ToInt32(dr["CURRENT_DEVICE"].ToString());
					    order.Source = (TaskSourceEnum) Enum.Parse(typeof (TaskSourceEnum), dr["SOURCE"].ToString().Trim());
					    order.IsReport = Convert.ToBoolean(dr["ISREPORT"].ToString());
                        order.GoodsType = ConvertHepler.ConvertToInt(dr["GOODS_TYPE"].ToString());
                        order.GoodsCount = ConvertHepler.ConvertToInt(dr["GOODS_COUNT"].ToString());
                        order.Qty = ConvertHepler.ConvertToInt(dr["QTY"].ToString());
                        if (!string.IsNullOrEmpty(dr["FINISH_TYPE"].ToString()))
                        {
                            order.FinishType = (FinishType)Enum.Parse(typeof(FinishType), dr["FINISH_TYPE"].ToString().Trim());
                        }
						orderList.Add(order);
					}
					catch (Exception ex)
					{
						Log.getExceptionFile().Info(ex);
					}
				}
			}
			return orderList;
		}
	}
}
