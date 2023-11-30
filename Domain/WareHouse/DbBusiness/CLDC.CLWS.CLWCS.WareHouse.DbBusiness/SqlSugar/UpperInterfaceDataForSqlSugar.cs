using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class UpperInterfaceDataForSqlSugar : UpperInterfaceDataAbstract
    {
        public UpperInterfaceDataForSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }
        public override OperateResult<UpperInterfaceInvoke> GetNotifyElement(string guidId)
        {
            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
            OperateResult<UpperInterfaceInvoke> interfaceResult = OperateResult.CreateFailedResult(invoke, "无数据");
            try
            {
                UpperInterfaceInvoke item = DbHelper.Query<UpperInterfaceInvoke>(t => t.GuidId == guidId);
                interfaceResult.Content = item;
                interfaceResult.IsSuccess = item != null;

            }
            catch (Exception ex)
            {
                interfaceResult.IsSuccess = false;
                interfaceResult.Message = OperateResult.ConvertException(ex);
            }
            return interfaceResult;
        }

        public override OperateResult<List<UpperInterfaceInvoke>> GetNotifyElement(InvokeStatusMode status)
        {
            List<UpperInterfaceInvoke> list = new List<UpperInterfaceInvoke>();
            OperateResult<List<UpperInterfaceInvoke>> result = OperateResult.CreateFailedResult(list, "无数据");
            try
            {
                list = DbHelper.QueryList<UpperInterfaceInvoke>(t => t.InvokeStatus == status);
                result.Content = list;
                result.IsSuccess = list != null && list.Count>0;
                
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult<List<UpperInterfaceInvoke>> GetNotifyElementByMethodName(string methodName)
        {
            List<UpperInterfaceInvoke> list = new List<UpperInterfaceInvoke>();
            OperateResult<List<UpperInterfaceInvoke>> result = OperateResult.CreateFailedResult(list, "无数据");
            try
            {
                list = DbHelper.QueryList<UpperInterfaceInvoke>(t => t.MethodName == methodName);
                result.Content = list;
                result.IsSuccess = list != null && list.Count > 0;
                //string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE METHODNAME='{0}'", methodName);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                //            invoke.Message = dr["MESSAGE"].ToString();
                //            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                //            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                //            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                //            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                //            invoke.GuidId = dr["GUID_ID"].ToString();
                //            invoke.Barcode = dr["BARCODE"].ToString();
                //            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                //            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                //            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                //            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                //            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                //            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                //            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                //            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                //            invoke.MethodName = dr["METHODNAME"].ToString();
                //            list.Add(invoke);
                //        }
                //        result.IsSuccess = true;
                //    }
                //    else
                //    {
                //        result.IsSuccess = false;
                //    }
                //}

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult IsExist(UpperInterfaceInvoke notifyElement)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                result.IsSuccess = DbHelper.IsExist<UpperInterfaceInvoke>(t => t.GuidId == notifyElement.GuidId);
                //string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE GUID_ID='{0}'", notifyElement.GuidId);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //    {
                //        result.IsSuccess = true;
                //    }
                //    else
                //    {
                //        result.IsSuccess = false;
                //    }
                //}

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult Update(UpperInterfaceInvoke interfaceInfo)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                bool res = DbHelper.Update<UpperInterfaceInvoke>(t => new UpperInterfaceInvoke
                {
                    InvokeTime = interfaceInfo.InvokeTime,
                    Result = interfaceInfo.Result,
                    InvokeDelay = interfaceInfo.InvokeDelay,
                    Message = interfaceInfo.Message,
                    InvokeStatus = interfaceInfo.InvokeStatus,
                    FirstInvokDateTime = interfaceInfo.FirstInvokDateTime,
                    InvokeFinishDateTime= interfaceInfo.InvokeFinishDateTime
                }, t => t.GuidId == interfaceInfo.GuidId) > 0;

                //string updateSql = string.Format(@"UPDATE T_BO_UPPERINTERFACEINVOKE SET INVOKETIME='{0}',RESULT='{1}',INVOKEDELAY='{2}',MESSAGE='{3}',INVOKESTATUS='{4}',FIRSTINVOKEDATETIME='{5}',INVOKEFINISHDATETIME='{6}' WHERE GUID_ID='{7}'",
                //    interfaceInfo.InvokeTime, interfaceInfo.Result, interfaceInfo.InvokeDelay, interfaceInfo.Message, 
                //    (int)interfaceInfo.InvokeStatus, interfaceInfo.FirstInvokDateTime, interfaceInfo.InvokeFinishDateTime, interfaceInfo.GuidId);
                result.IsSuccess = res;// DbHelper.ExecuteNonQuery(updateSql);
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult Insert(UpperInterfaceInvoke interfaceInfo)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                interfaceInfo.WhCode = WareHouseCode;
                interfaceInfo.AddDateTime = DateTime.Now;
                bool res = DbHelper.Add(interfaceInfo) > 0;
                //string insertSql = string.Format(
                //      @"INSERT INTO T_BO_UPPERINTERFACEINVOKE(GUID_ID,METHODNAME,BUSINESSNAME,CALLBACKFUNC,INVOKETIME,MAXTIME,RESULT,INVOKETIMEOUT,INVOKEDELAY,MESSAGE,INVOKESTATUS,FIRSTINVOKEDATETIME,INVOKEPARAMETERS,ADDDATE,BARCODE,WH_CODE)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')",
                //      interfaceInfo.GuidId, interfaceInfo.MethodName, interfaceInfo.BusinessName,
                //      interfaceInfo.CallBackFuncName, interfaceInfo.InvokeTime, interfaceInfo.MaxTime,
                //      interfaceInfo.Result, interfaceInfo.TimeOut, interfaceInfo.InvokeDelay, interfaceInfo.Message,
                //      (int)interfaceInfo.InvokeStatus, interfaceInfo.FirstInvokDateTime,
                //      interfaceInfo.Parameters, interfaceInfo.AddDateTime, interfaceInfo.Barcode,interfaceInfo.WhCode);
                result.IsSuccess = res;//DbHelper.ExecuteNonQuery(insertSql);
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult<List<UpperInterfaceInvoke>> GetNotifyElement(List<InvokeStatusMode> statusList)
        {
            List<UpperInterfaceInvoke> list = new List<UpperInterfaceInvoke>();
            OperateResult<List<UpperInterfaceInvoke>> result = OperateResult.CreateFailedResult(list, "无数据");
            string statusIds = "-1";
            foreach (InvokeStatusMode status in statusList)
            {
                statusIds += "," + (int)status;
            }
            try
            {

                list = DbHelper.QueryList<UpperInterfaceInvoke>(t => statusList.Contains((InvokeStatusMode)t.InvokeStatus));
                result.Content = list;
                result.IsSuccess = true;

                //string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE INVOKESTATUS in ({0})", statusIds);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                //            invoke.Message = dr["MESSAGE"].ToString();
                //            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                //            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                //            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                //            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                //            invoke.GuidId = dr["GUID_ID"].ToString();
                //            invoke.Barcode = dr["BARCODE"].ToString();
                //            invoke.InvokeDelay = long.Parse(dr["INVOKEDELAY"].ToString());
                //            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                //            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                //            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                //            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                //            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                //            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                //            invoke.TimeOut = long.Parse(dr["INVOKETIMEOUT"].ToString());
                //            invoke.MethodName = dr["METHODNAME"].ToString();
                //            list.Add(invoke);
                //        }
                //        result.IsSuccess = true;
                //    }
                //    else
                //    {
                //        result.IsSuccess = true;
                //    }
                //}
               
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult<List<UpperInterfaceInvoke>> GetNotifyElementByWhere(Expression<Func<UpperInterfaceInvoke, bool>> whereLambda)
        {
            List<UpperInterfaceInvoke> list = new List<UpperInterfaceInvoke>();
            OperateResult<List<UpperInterfaceInvoke>> result = OperateResult.CreateFailedResult(list, "无数据");
            try
            {
                list=DbHelper.QueryList(whereLambda);
                result.Content = list;
                result.IsSuccess = list != null && list.Count > 0;
                //using (DataSet ds = DbHelper.GetDataSet(where))
                //{
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                //            invoke.Message = dr["MESSAGE"].ToString();
                //            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                //            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                //            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                //            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                //            invoke.GuidId = dr["GUID_ID"].ToString();
                //            invoke.Barcode = dr["BARCODE"].ToString();
                //            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                //            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                //            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                //            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                //            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                //            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                //            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                //            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                //            invoke.MethodName = dr["METHODNAME"].ToString();
                //            list.Add(invoke);
                //        }
                //        result.IsSuccess = true;
                //    }
                //    else
                //    {
                //        result.IsSuccess = false;
                //    }
                //}

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }
        private Expression<Func<UpperInterfaceInvoke, bool>> CombineWhereSql(UpperInterfaceInvokeFilter filter)
        {
            Expression<Func<UpperInterfaceInvoke, bool>> whereLambda = t => t.WhCode == WareHouseCode;

            //StringBuilder sqlBuilder = new StringBuilder();
            //sqlBuilder.Append(string.Format(" WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.MethodName))
            {
                whereLambda = whereLambda.AndAlso(t => t.MethodName.Contains(filter.MethodName));
                //sqlBuilder.Append(string.Format(" AND METHODNAME LIKE '%{0}%'", filter.MethodName));
            }
            if (!string.IsNullOrEmpty(filter.HandleStatus))
            {
                whereLambda = whereLambda.AndAlso(t => t.InvokeStatus==(InvokeStatusMode?)Convert.ToInt32(filter.HandleStatus));
                //sqlBuilder.Append(string.Format(" AND INVOKESTATUS={0}", filter.HandleStatus));
            }
            if (!string.IsNullOrEmpty(filter.HandleResult))
            {
                whereLambda = whereLambda.AndAlso(t => t.Result == Convert.ToInt32(filter.HandleResult));
                //sqlBuilder.Append(string.Format(" AND MESSAGE LIKE '%{0}%'", filter.HandleResult));
            }
            if (!string.IsNullOrEmpty(filter.Barcode))
            {
                whereLambda = whereLambda.AndAlso(t => t.Barcode.Contains(filter.Barcode));
                //sqlBuilder.Append(string.Format(" AND BARCODE LIKE '%{0}%'", filter.Barcode));
            }
            if (!string.IsNullOrEmpty(filter.HandleFromTime) && !string.IsNullOrEmpty(filter.HandleToTime))
            {
                whereLambda = whereLambda.AndAlso(t => t.AddDateTime >= Convert.ToDateTime(filter.HandleFromTime) && t.AddDateTime<=Convert.ToDateTime(filter.HandleToTime));
                //sqlBuilder.Append(string.Format(" AND  ADDDATE between  '{0}' and '{1}'", filter.HandleFromTime, filter.HandleToTime));
            }
           
            return whereLambda;
        }
        public override long GetTotalCount()
        {
            int count = DbHelper.QueryCount<UpperInterfaceInvoke>(t => t.WhCode == WareHouseCode);
            //string sql = string.Format(" SELECT count(1) as Num FROM T_BO_UPPERINTERFACEINVOKE T WHERE T.WH_CODE='{0}'", WareHouseCode);
            //string num = DbHelper.ExecuteScalar(sql);
            //long count;
            //long.TryParse(num, out count);
            return count;
        }

        public override List<UpperInterfaceInvoke> SelectData(UpperInterfaceInvokeFilter filterModel,out int totalCount)
        {
            totalCount = 0;
            List<UpperInterfaceInvoke> dataPool = new List<UpperInterfaceInvoke>();
            var where = CombineWhereSql(filterModel);

            //string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY ADDDATE DESC) AS rownumber,* FROM T_BO_UPPERINTERFACEINVOKE WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filterModel.PageSize, filterModel.PageIndex, where);
            try
            {
                dataPool=DbHelper.QueryPageList<UpperInterfaceInvoke>((int)filterModel.PageIndex, (int)filterModel.PageSize, "ADDDATE DESC", out totalCount, where);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                //            invoke.Message = dr["MESSAGE"].ToString();
                //            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                //            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                //            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                //            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                //            invoke.GuidId = dr["GUID_ID"].ToString();
                //            invoke.Barcode = dr["BARCODE"].ToString();
                //            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                //            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                //            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                //            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                //            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                //            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                //            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                //            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                //            invoke.MethodName = dr["METHODNAME"].ToString();
                //            dataPool.Add(invoke);
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
