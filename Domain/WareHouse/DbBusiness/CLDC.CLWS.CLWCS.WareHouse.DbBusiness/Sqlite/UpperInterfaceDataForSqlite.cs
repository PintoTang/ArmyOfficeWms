using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class UpperInterfaceDataForSqlite : UpperInterfaceDataAbstract
    {
        public UpperInterfaceDataForSqlite(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }
        public override OperateResult<UpperInterfaceInvoke> GetNotifyElement(string guidId)
        {
            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
            OperateResult<UpperInterfaceInvoke> interfaceResult = OperateResult.CreateFailedResult(invoke, "无数据");
            try
            {
                string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE GUID_ID='{0}'", guidId);
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        invoke.Message = dr["MESSAGE"].ToString();
                        invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                        invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                        invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                        invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                        invoke.GuidId = dr["GUID_ID"].ToString();
                        invoke.Barcode = dr["BARCODE"].ToString();
                        invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                        invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                        invoke.InvokeStatus =  (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                        invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                        invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                        invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                        invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                        invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                        invoke.MethodName = dr["METHODNAME"].ToString();
                        interfaceResult.IsSuccess = true;
                    }
                    else
                    {
                        interfaceResult.IsSuccess = false;
                    }
                }

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
                string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE INVOKESTATUS='{0}'", (int)status);
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                            invoke.Message = dr["MESSAGE"].ToString();
                            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                            invoke.GuidId = dr["GUID_ID"].ToString();
                            invoke.Barcode = dr["BARCODE"].ToString();
                            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                            invoke.MethodName = dr["METHODNAME"].ToString();
                            list.Add(invoke);
                        }
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                    }
                }

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
                string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE METHODNAME='{0}'", methodName);
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                            invoke.Message = dr["MESSAGE"].ToString();
                            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                            invoke.GuidId = dr["GUID_ID"].ToString();
                            invoke.Barcode = dr["BARCODE"].ToString();
                            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                            invoke.MethodName = dr["METHODNAME"].ToString();
                            list.Add(invoke);
                        }
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                    }
                }

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
                string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE GUID_ID='{0}'", notifyElement.GuidId);
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                    }
                }

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
                string updateSql = string.Format(@"UPDATE T_BO_UPPERINTERFACEINVOKE SET INVOKETIME='{0}',RESULT='{1}',INVOKEDELAY='{2}',MESSAGE='{3}',INVOKESTATUS='{4}',FIRSTINVOKEDATETIME='{5}',INVOKEFINISHDATETIME='{6}' WHERE GUID_ID='{7}'",
                    interfaceInfo.InvokeTime, interfaceInfo.Result, interfaceInfo.InvokeDelay, interfaceInfo.Message, (int)interfaceInfo.InvokeStatus, interfaceInfo.FirstInvokDateTime, interfaceInfo.InvokeFinishDateTime, interfaceInfo.GuidId);
                result.IsSuccess = DbHelper.ExecuteNonQuery(updateSql);
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
                string insertSql = string.Format(
                      @"INSERT INTO T_BO_UPPERINTERFACEINVOKE(GUID_ID,METHODNAME,BUSINESSNAME,CALLBACKFUNC,INVOKETIME,MAXTIME,RESULT,INVOKETIMEOUT,INVOKEDELAY,MESSAGE,INVOKESTATUS,FIRSTINVOKEDATETIME,INVOKEPARAMETERS,ADDDATE,BARCODE,WH_CODE)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')",
                      interfaceInfo.GuidId, interfaceInfo.MethodName, interfaceInfo.BusinessName,
                      interfaceInfo.CallBackFuncName, interfaceInfo.InvokeTime, interfaceInfo.MaxTime,
                      interfaceInfo.Result, interfaceInfo.TimeOut, interfaceInfo.InvokeDelay, interfaceInfo.Message,
                      (int)interfaceInfo.InvokeStatus, interfaceInfo.FirstInvokDateTime,
                      interfaceInfo.Parameters, interfaceInfo.AddDateTime, interfaceInfo.Barcode, interfaceInfo.WhCode);
                result.IsSuccess = DbHelper.ExecuteNonQuery(insertSql);
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
                string selectSql = string.Format(@"SELECT *FROM T_BO_UPPERINTERFACEINVOKE WHERE INVOKESTATUS in ({0})", statusIds);
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                            invoke.Message = dr["MESSAGE"].ToString();
                            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                            invoke.GuidId = dr["GUID_ID"].ToString();
                            invoke.Barcode = dr["BARCODE"].ToString();
                            invoke.InvokeDelay = long.Parse(dr["INVOKEDELAY"].ToString());
                            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                            invoke.TimeOut = long.Parse(dr["INVOKETIMEOUT"].ToString());
                            invoke.MethodName = dr["METHODNAME"].ToString();
                            list.Add(invoke);
                        }
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
               
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult<List<UpperInterfaceInvoke>> GetNotifyElementByWhere(string where)
        {
            List<UpperInterfaceInvoke> list = new List<UpperInterfaceInvoke>();
            OperateResult<List<UpperInterfaceInvoke>> result = OperateResult.CreateFailedResult(list, "无数据");
            try
            {
                using (DataSet ds = DbHelper.GetDataSet(where))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                            invoke.Message = dr["MESSAGE"].ToString();
                            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                            invoke.GuidId = dr["GUID_ID"].ToString();
                            invoke.Barcode = dr["BARCODE"].ToString();
                            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                            invoke.MethodName = dr["METHODNAME"].ToString();
                            list.Add(invoke);
                        }
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                    }
                }
               
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private string CombineWhereSql(UpperInterfaceInvokeFilter filter)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(string.Format(" WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.MethodName))
            {
                sqlBuilder.Append(string.Format(" AND METHODNAME LIKE '%{0}%'", filter.MethodName));
            }
            if (!string.IsNullOrEmpty(filter.HandleStatus))
            {
                sqlBuilder.Append(string.Format(" AND INVOKESTATUS={0}", filter.HandleStatus));
            }
            if (!string.IsNullOrEmpty(filter.HandleResult))
            {
                sqlBuilder.Append(string.Format(" AND MESSAGE LIKE '%{0}%'", filter.HandleResult));
            }
            if (!string.IsNullOrEmpty(filter.Barcode))
            {
                sqlBuilder.Append(string.Format(" AND BARCODE LIKE '%{0}%'", filter.Barcode));
            }
            if (!string.IsNullOrEmpty(filter.HandleFromTime) && !string.IsNullOrEmpty(filter.HandleToTime))
            {
                sqlBuilder.Append(string.Format(" AND  ADDDATE between  '{0}' and '{1}'", filter.HandleFromTime, filter.HandleToTime));
            }

            return sqlBuilder.ToString();
        }
        public override long GetTotalCount()
        {
            string sql = string.Format(" SELECT count(1) as Num FROM T_BO_UPPERINTERFACEINVOKE T WHERE T.WH_CODE='{0}'", WareHouseCode);
            string num = DbHelper.ExecuteScalar(sql);
            long count;
            long.TryParse(num, out count);
            return count;
        }

        public override List<UpperInterfaceInvoke> SelectData(UpperInterfaceInvokeFilter filterModel)
        {
            List<UpperInterfaceInvoke> dataPool = new List<UpperInterfaceInvoke>();
            string where = CombineWhereSql(filterModel);
            string selectSql = string.Format(@"select * from T_BO_UPPERINTERFACEINVOKE  WHERE 1=1 and {0} order by ADDDATE limit {1} offset {2} ", where, filterModel.PageSize, filterModel.PageIndex);
            //string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY ADDDATE DESC) AS rownumber,* FROM T_BO_UPPERINTERFACEINVOKE WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filterModel.PageSize, filterModel.PageIndex, where);
            try
            {
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
                            invoke.Message = dr["MESSAGE"].ToString();
                            invoke.AddDateTime = ConvertHepler.ConvertToDateTime(dr["ADDDATE"].ToString());
                            invoke.BusinessName = dr["BUSINESSNAME"].ToString();
                            invoke.CallBackFuncName = dr["CALLBACKFUNC"].ToString();
                            invoke.FirstInvokDateTime = ConvertHepler.ConvertToDateTime(dr["FIRSTINVOKEDATETIME"].ToString());
                            invoke.GuidId = dr["GUID_ID"].ToString();
                            invoke.Barcode = dr["BARCODE"].ToString();
                            invoke.InvokeDelay = ConvertHepler.ConvertToInt(dr["INVOKEDELAY"].ToString());
                            invoke.InvokeFinishDateTime = ConvertHepler.ConvertToDateTime(dr["INVOKEFINISHDATETIME"].ToString());
                            invoke.InvokeStatus = (InvokeStatusMode)Enum.Parse(typeof(InvokeStatusMode), dr["INVOKESTATUS"].ToString());
                            invoke.InvokeTime = ConvertHepler.ConvertToInt(dr["INVOKETIME"].ToString());
                            invoke.MaxTime = ConvertHepler.ConvertToInt(dr["MAXTIME"].ToString());
                            invoke.Parameters = dr["INVOKEPARAMETERS"].ToString();
                            invoke.Result = ConvertHepler.ConvertToInt(dr["RESULT"].ToString());
                            invoke.TimeOut = ConvertHepler.ConvertToInt(dr["INVOKETIMEOUT"].ToString());
                            invoke.MethodName = dr["METHODNAME"].ToString();
                            dataPool.Add(invoke);
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
