using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.SqlServer
{

    public sealed class WhAddressDataForSqlServer : WhAddressDataAbstract
    {
        public WhAddressDataForSqlServer(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult IsExist(WhAddressModel data)
        {
            string sql = string.Format("SELECT count(1) as Num FROM T_BO_WH_ADDRESS T WHERE T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", data.WcsAddr, data.WhCode);
            string num = DbHelper.ExecuteScalar(sql);
            int count;
            int.TryParse(num, out count);
            if (count > 0)
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
                string updateSql = string.Format(
            @"UPDATE T_BO_WH_ADDRESS SET UPPER_ADDR='{0}', SHOW_NAME='{1}', LOWER_ADDR='{2}' WHERE (WCS_ADDR='{3}') AND (WH_CODE='{4}')",
            data.UpperAddr, data.ShowName, data.LowerAddr, data.WcsAddr, data.WhCode);
                updateResult.IsSuccess = DbHelper.ExecuteNonQuery(updateSql);
                updateResult.Message = "更新成功";
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
                string insertSql =
                    string.Format(
                    @"INSERT INTO T_BO_WH_ADDRESS (WCS_ADDR, UPPER_ADDR, SHOW_NAME, LOWER_ADDR, WH_CODE) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                     data.WcsAddr, data.UpperAddr, data.ShowName, data.LowerAddr, data.WhCode);
                insertResult.IsSuccess = DbHelper.ExecuteNonQuery(insertSql);
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
                string sql = string.Format("SELECT *FROM T_BO_WH_ADDRESS T WHERE T.WH_CODE='{0}'", WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        whAddressList.AddRange(from DataRow dr in ds.Tables[0].Rows
                                               select new WhAddressModel
                                               {
                                                   WcsAddr = dr["WCS_ADDR"].ToString(),
                                                   UpperAddr = dr["UPPER_ADDR"].ToString(),
                                                   LowerAddr = dr["LOWER_ADDR"].ToString(),
                                                   ShowName = dr["SHOW_NAME"].ToString(),
                                               });
                    }
                }

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
                string sql = string.Format("SELECT TOP(1) UPPER_ADDR FROM T_BO_WH_ADDRESS T WHERE  T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", wcsAddr, WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result.Content = ds.Tables[0].Rows[0]["UPPER_ADDR"].ToString();
                        result.IsSuccess = true;
                        result.Message = "获取数据成功";
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

        public override OperateResult<string> GetLowerAddrByWcsAddr(string wcsAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                string sql = string.Format("SELECT TOP(1) LOWER_ADDR FROM T_BO_WH_ADDRESS T WHERE  T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", wcsAddr, WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result.Content = ds.Tables[0].Rows[0]["LOWER_ADDR"].ToString();
                        result.IsSuccess = true;
                        result.Message = "获取数据成功";
                    }
                }
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
                string sql = string.Format("SELECT TOP(1) WCS_ADDR FROM T_BO_WH_ADDRESS T WHERE  T.UPPER_ADDR='{0}' AND T.WH_CODE='{1}'", upperAddr, WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result.Content = ds.Tables[0].Rows[0]["WCS_ADDR"].ToString();
                        result.IsSuccess = true;
                        result.Message = "获取数据成功";
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

        public override OperateResult<string> GetShowNameByWcsAddr(string wcsAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                string sql = string.Format("SELECT TOP(1) SHOW_NAME FROM T_BO_WH_ADDRESS T WHERE  T.WCS_ADDR='{0}' AND T.WH_CODE='{1}'", wcsAddr, WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result.Content = ds.Tables[0].Rows[0]["SHOW_NAME"].ToString();
                        result.IsSuccess = true;
                        result.Message = "获取数据成功";
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

        public override OperateResult<string> GetShowNameByUpperAddr(string upperAddr)
        {
            OperateResult<string> result = OperateResult.CreateSuccessResult<string>("无数据");
            try
            {
                string sql = string.Format("SELECT TOP(1) SHOW_NAME FROM T_BO_WH_ADDRESS T WHERE  T.UPPER_ADDR='{0}' AND T.WH_CODE='{1}'", upperAddr, WareHouseCode);
                using (DataSet ds = DbHelper.GetDataSet(sql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        result.Content = ds.Tables[0].Rows[0]["SHOW_NAME"].ToString();
                        result.IsSuccess = true;
                        result.Message = "获取数据成功";
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
        private string CombineWhereSql(WhAddressDataFilter filter)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(string.Format(" WH_CODE ='{0}' ", WareHouseCode));
            if (!string.IsNullOrEmpty(filter.LowerAddress))
            {
                sqlBuilder.Append(string.Format(" AND LOWER_ADDR LIKE '%{0}%'", filter.LowerAddress));
            }
            if (!string.IsNullOrEmpty(filter.ShowName))
            {
                sqlBuilder.Append(string.Format(" AND SHOW_NAME LIKE '%{0}%'", filter.ShowName));
            }
            if (!string.IsNullOrEmpty(filter.WcsAddress))
            {
                sqlBuilder.Append(string.Format(" AND WCS_ADDR LIKE '%{0}%'", filter.WcsAddress));
            }
            if (!string.IsNullOrEmpty(filter.UpperAddress))
            {
                sqlBuilder.Append(string.Format(" AND UPPER_ADDR LIKE '%{0}%'", filter.UpperAddress));
            }

            return sqlBuilder.ToString();
        }
        public override List<WhAddressModel> SelectData(WhAddressDataFilter filterModel)
        {
            List<WhAddressModel> dataPool = new List<WhAddressModel>();
            string where = CombineWhereSql(filterModel);
            string selectSql = string.Format(@"SELECT TOP {0} * FROM(SELECT row_number() OVER(ORDER BY WCS_ADDR DESC) AS rownumber,* FROM T_BO_WH_ADDRESS WHERE 1=1 AND {2}) temp_row WHERE rownumber>({1}-1)*{0}", filterModel.PageSize, filterModel.PageIndex, where);
            try
            {
                using (DataSet ds = DbHelper.GetDataSet(selectSql))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            WhAddressModel task = new WhAddressModel
                            {
                                LowerAddr = dr["LOWER_ADDR"].ToString(),
                                ShowName = dr["SHOW_NAME"].ToString(),
                                UpperAddr = dr["UPPER_ADDR"].ToString(),
                                WcsAddr = dr["WCS_ADDR"].ToString()
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
            string sql = string.Format(" SELECT count(1) as Num FROM T_BO_WH_ADDRESS T WHERE T.WH_CODE='{0}'", WareHouseCode);
            string num = DbHelper.ExecuteScalar(sql);
            long count;
            long.TryParse(num, out count);
            return count;
        }

        public override OperateResult Delete(WhAddressModel selectedData)
        {
            OperateResult result=OperateResult.CreateFailedResult();
            try
            {
                string sql = string.Format("DELETE FROM T_BO_WH_ADDRESS  WHERE WCS_ADDR='{0}' AND WH_CODE='{1}'", selectedData.WcsAddr, selectedData.WhCode);
                bool done = DbHelper.ExecuteNonQuery(sql);
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
