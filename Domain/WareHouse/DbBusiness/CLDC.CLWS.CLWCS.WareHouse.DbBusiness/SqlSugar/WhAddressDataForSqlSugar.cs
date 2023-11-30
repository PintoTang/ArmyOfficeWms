using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{

    public sealed class WhAddressDataForSqlSugar : WhAddressDataAbstract
    {
        public WhAddressDataForSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult IsExist(WhAddressModel data)
        {
            bool isExists = DbHelper.IsExist<WhAddressModel>(t => t.WcsAddr == data.WcsAddr && t.WhCode == WareHouseCode);
           
            if (isExists)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult Update(WhAddressModel data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Update<WhAddressModel>(t => new WhAddressModel
                {
                    UpperAddr = data.UpperAddr,
                    ShowName = data.ShowName,
                    LowerAddr = data.LowerAddr,
                }, t => t.WcsAddr == data.WcsAddr && t.WhCode == WareHouseCode) > 0;
                //    string updateSql = string.Format(
                //@"UPDATE T_BO_WH_ADDRESS SET UPPER_ADDR='{0}', SHOW_NAME='{1}', LOWER_ADDR='{2}' WHERE (WCS_ADDR='{3}') AND (WH_CODE='{4}')",
                //data.UpperAddr, data.ShowName, data.LowerAddr, data.WcsAddr, data.WhCode);
                updateResult.IsSuccess = result;//DbHelper.ExecuteNonQuery(updateSql);
                updateResult.Message = result?"更新成功":"更新失败";
            }
            catch (Exception ex)
            {
                updateResult.Message = OperateResult.ConvertException(ex);
                updateResult.IsSuccess = false;
            }
            return updateResult;
        }

        public override OperateResult Insert(WhAddressModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                data.WhCode = WareHouseCode;
                bool result = DbHelper.Add(data) > 0;
                //string insertSql =
                //    string.Format(
                //    @"INSERT INTO T_BO_WH_ADDRESS (WCS_ADDR, UPPER_ADDR, SHOW_NAME, LOWER_ADDR, WH_CODE) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                //     data.WcsAddr, data.UpperAddr, data.ShowName, data.LowerAddr, data.WhCode);
                insertResult.IsSuccess = result;// DbHelper.ExecuteNonQuery(insertSql);
            }
            catch (Exception ex)
            {
                insertResult.Message = OperateResult.ConvertException(ex);
                insertResult.IsSuccess = false;

            }
            return insertResult;
        }

        public override OperateResult<List<WhAddressModel>> GetAllData()
        {
            List<WhAddressModel> whAddressList = new List<WhAddressModel>();
            OperateResult<List<WhAddressModel>> result = OperateResult.CreateSuccessResult(whAddressList);
            try
            {
                whAddressList=DbHelper.QueryList<WhAddressModel>(t => t.WhCode == WareHouseCode);
                result.Content = whAddressList;
                result.IsSuccess = whAddressList != null && whAddressList.Count > 0;
                //string sql = string.Format("SELECT *FROM T_BO_WH_ADDRESS T WHERE T.WH_CODE='{0}'", WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        whAddressList.AddRange(from DataRow dr in ds.Tables[0].Rows
                //                               select new WhAddressModel
                //                               {
                //                                   WcsAddr = dr["WCS_ADDR"].ToString(),
                //                                   UpperAddr = dr["UPPER_ADDR"].ToString(),
                //                                   LowerAddr = dr["LOWER_ADDR"].ToString(),
                //                                   ShowName = dr["SHOW_NAME"].ToString(),
                //                               });
                //    }
                //}

            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult<string> GetUpperAddrByWcsAddr(string wcsAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                var item = DbHelper.Query<WhAddressModel>(t => t.WcsAddr== wcsAddr && t.WhCode == WareHouseCode);
                if (item != null)
                {
                    result.Content = item.UpperAddr;
                    result.IsSuccess = true;
                    result.Message = "获取数据成功";
                }
                //string sql = string.Format("SELECT TOP(1) UPPER_ADDR FROM T_BO_WH_ADDRESS T WHERE  T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", wcsAddr, WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        result.Content = ds.Tables[0].Rows[0]["UPPER_ADDR"].ToString();
                //        result.IsSuccess = true;
                //        result.Message = "获取数据成功";
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

        public override OperateResult<string> GetLowerAddrByWcsAddr(string wcsAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                var item = DbHelper.Query<WhAddressModel>(t => t.WcsAddr == wcsAddr && t.WhCode == WareHouseCode);
                if (item != null)
                {
                    result.Content = item.LowerAddr;
                    result.IsSuccess = true;
                    result.Message = "获取数据成功";
                }
                //string sql = string.Format("SELECT TOP(1) LOWER_ADDR FROM T_BO_WH_ADDRESS T WHERE  T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", wcsAddr, WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        result.Content = ds.Tables[0].Rows[0]["LOWER_ADDR"].ToString();
                //        result.IsSuccess = true;
                //        result.Message = "获取数据成功";
                //    }
                //}
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult<string> GetWcsAddrByUpperAddr(string upperAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                var item = DbHelper.Query<WhAddressModel>(t => t.UpperAddr == upperAddr && t.WhCode == WareHouseCode);
                if (item != null)
                {
                    result.Content = item.LowerAddr;
                    result.IsSuccess = true;
                    result.Message = "获取数据成功";
                }
                //string sql = string.Format("SELECT TOP(1) WCS_ADDR FROM T_BO_WH_ADDRESS T WHERE  T.UPPER_ADDR='{0}' AND T.WH_CODE='{1}'", upperAddr, WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        result.Content = ds.Tables[0].Rows[0]["WCS_ADDR"].ToString();
                //        result.IsSuccess = true;
                //        result.Message = "获取数据成功";
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

        public override OperateResult<string> GetShowNameByWcsAddr(string wcsAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                var item = DbHelper.Query<WhAddressModel>(t => t.WcsAddr == wcsAddr && t.WhCode == WareHouseCode);
                if (item != null)
                {
                    result.Content = item.ShowName;
                    result.IsSuccess = true;
                    result.Message = "获取数据成功";
                }
                //string sql = string.Format("SELECT TOP(1) SHOW_NAME FROM T_BO_WH_ADDRESS T WHERE  T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", wcsAddr, WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        result.Content = ds.Tables[0].Rows[0]["SHOW_NAME"].ToString();
                //        result.IsSuccess = true;
                //        result.Message = "获取数据成功";
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

        public override OperateResult<string> GetShowNameByUpperAddr(string upperAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                var item = DbHelper.Query<WhAddressModel>(t => t.UpperAddr == upperAddr && t.WhCode == WareHouseCode);
                if (item != null)
                {
                    result.Content = item.ShowName;
                    result.IsSuccess = true;
                    result.Message = "获取数据成功";
                }
                //string sql = string.Format("SELECT TOP(1) SHOW_NAME FROM T_BO_WH_ADDRESS T WHERE  T.UPPER_ADDR='{0}' AND T.WH_CODE='{1}'", upperAddr, WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(sql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        result.Content = ds.Tables[0].Rows[0]["SHOW_NAME"].ToString();
                //        result.IsSuccess = true;
                //        result.Message = "获取数据成功";
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
        private Expression<Func<WhAddressModel, bool>> CombineWhereSql(WhAddressDataFilter filter)
        {
            Expression<Func<WhAddressModel, bool>> whereLambda = t => t.WhCode == WareHouseCode;
            //StringBuilder sqlBuilder = new StringBuilder();
            //sqlBuilder.Append(string.Format(" WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.LowerAddress))
            {
                whereLambda = whereLambda.AndAlso(t => t.LowerAddr.Contains(filter.LowerAddress));
                //sqlBuilder.Append(string.Format(" AND LOWER_ADDR LIKE '%{0}%'", filter.LowerAddress));
            }
            if (!string.IsNullOrEmpty(filter.ShowName))
            {
                whereLambda = whereLambda.AndAlso(t => t.ShowName.Contains(filter.ShowName));
                //sqlBuilder.Append(string.Format(" AND SHOW_NAME LIKE '%{0}%'", filter.ShowName));
            }
            if (!string.IsNullOrEmpty(filter.WcsAddress))
            {
                whereLambda = whereLambda.AndAlso(t => t.WcsAddr.Contains(filter.WcsAddress));
                //sqlBuilder.Append(string.Format(" AND WCS_ADDR LIKE '%{0}%'", filter.WcsAddress));
            }
            if (!string.IsNullOrEmpty(filter.UpperAddress))
            {
                whereLambda = whereLambda.AndAlso(t => t.UpperAddr.Contains(filter.UpperAddress));
                //sqlBuilder.Append(string.Format(" AND UPPER_ADDR LIKE '%{0}%'", filter.UpperAddress));
            }

            return whereLambda;
        }
        public override List<WhAddressModel> SelectData(WhAddressDataFilter filterModel,out int totalCount)
        {
            totalCount = 0;
            List<WhAddressModel> dataPool = new List<WhAddressModel>();
            var where = CombineWhereSql(filterModel);
            //string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY WCS_ADDR DESC) AS rownumber,* FROM T_BO_WH_ADDRESS WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filterModel.PageSize, filterModel.PageIndex, where);
            try
            {
                dataPool = DbHelper.QueryPageList((int)filterModel.PageIndex,(int)filterModel.PageSize, "WCS_ADDR DESC",out totalCount,where);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            WhAddressModel task = new WhAddressModel
                //            {
                //                LowerAddr = dr["LOWER_ADDR"].ToString(),
                //                ShowName = dr["SHOW_NAME"].ToString(),
                //                UpperAddr = dr["UPPER_ADDR"].ToString(),
                //                WcsAddr = dr["WCS_ADDR"].ToString()
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
            int count = DbHelper.QueryCount<WhAddressModel>(t => t.WhCode == WareHouseCode);
            //string sql = string.Format(" SELECT count(1) as Num FROM T_BO_WH_ADDRESS T WHERE T.WH_CODE='{0}'", WareHouseCode);
            //string num = DbHelper.ExecuteScalar(sql);
            //long count;
            //long.TryParse(num, out count);
            return count;
        }

        public override OperateResult Delete(WhAddressModel selectedData)
        {
            OperateResult result=OperateResult.CreateFailedResult();
            try
            {
                bool done=DbHelper.Delete<WhAddressModel>(t => t.WcsAddr == selectedData.WcsAddr && t.WhCode == WareHouseCode)>0;
                //string sql = string.Format("DELETE FROM T_BO_WH_ADDRESS  WHERE WCS_ADDR='{0}' AND WH_CODE='{1}'", selectedData.WcsAddr, selectedData.WhCode);
                //bool done = DbHelper.ExecuteNonQuery(sql);
                result.IsSuccess = done;
                result.Message = "操作成功";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }

            return result;
        }
    }
}
