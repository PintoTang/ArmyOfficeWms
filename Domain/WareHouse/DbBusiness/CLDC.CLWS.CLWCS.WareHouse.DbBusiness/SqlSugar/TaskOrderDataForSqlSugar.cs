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
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class TaskOrderDataForSqlSugar : TaskOrderDataAbstract
    {
        public TaskOrderDataForSqlSugar(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        public override OperateResult IsExist(TaskOrderDataModel data)
        {
            OperateResult isExistResult = OperateResult.CreateFailedResult();
            try
            {
                bool isExists = DbHelper.IsExist<TaskOrderDataModel>(t => t.OrderId == data.OrderId && t.WhCode == WareHouseCode);
                //string[] sql = new string[1];
                //sql[0] = string.Format("SELECT *FROM T_AC_TASKORDER WHERE ORDER_ID={0} AND WH_CODE='{1}'", data.OrderId, data.WhCode);
                //DataSet ds = DbHelper.GetDataSet(sql);
                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                //{
                //    isExistResult.IsSuccess = true;
                //    return isExistResult;
                //}
                isExistResult.IsSuccess = isExists;
                return isExistResult;
            }
            catch (Exception ex)
            {
                isExistResult.IsSuccess = true;
                isExistResult.Message = OperateResult.ConvertException(ex);
            }
            return isExistResult;
        }

        public override OperateResult Update(TaskOrderDataModel data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Update<TaskOrderDataModel>(t => new TaskOrderDataModel
                {
                    TaskCode = data.TaskCode,
                    DeviceTaskCode = data.DeviceTaskCode,
                }, t => t.OrderId == data.OrderId && t.WhCode == WareHouseCode) > 0;

                //var sql = string.Format("UPDATE T_AC_TASKORDER SET TASK_CODE='{0}',DEVICE_TASK_CODE='{1}' WHERE ORDER_ID='{2}' AND WH_CODE='{3}'", 
                //    data.TaskCode,data.DeviceTaskCode, data.OrderId, data.WhCode);
                //int result = DbHelper.ExecuteNonQuery(new[] { sql });
                if (!result)
                {
                    updateResult.IsSuccess = false;
                    updateResult.Message = string.Format("更新数据失败");
                    return updateResult;
                }
                updateResult.IsSuccess = true;
                return updateResult;
            }
            catch (Exception ex)
            {
                updateResult.IsSuccess = false;
                updateResult.Message = OperateResult.ConvertException(ex);
            }
            return updateResult;
        }

        public override OperateResult Insert(TaskOrderDataModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {
                data.WhCode = WareHouseCode;
                data.AddTime = DateTime.Now;
                bool result = DbHelper.Add(data) > 0;
                //string sql = string.Format("INSERT INTO T_AC_TASKORDER(TASK_CODE,DEVICE_TASK_CODE,WH_CODE,ORDER_ID,ADDTIME) VALUES('{0}','{1}','{2}','{3}',{4})", data.TaskCode,data.DeviceTaskCode, data.WhCode, data.OrderId, "current_timestamp");
                //int result = DbHelper.ExecuteNonQuery(new[] { sql });
                if (!result)
                {

                    insertResult.IsSuccess = false;
                    insertResult.Message = "插入任务编号：" + data.TaskCode + "数据库时出错";
                    return insertResult;
                }
                insertResult.IsSuccess = true;
                return insertResult;
            }
            catch (Exception ex)
            {
                insertResult.IsSuccess = false;
                insertResult.Message = OperateResult.ConvertException(ex);

            }
            return insertResult;
        }

        public override OperateResult<TaskOrderDataModel> GetTaskCodeByOrderId(int orderId)
        {
            TaskOrderDataModel taskOrder = new TaskOrderDataModel();
            OperateResult<TaskOrderDataModel> getResult = OperateResult.CreateFailedResult(taskOrder, "无数据");
            try
            {
                taskOrder = DbHelper.Query<TaskOrderDataModel>(t => t.OrderId == orderId && t.WhCode == WareHouseCode);
                //string selectSql =
                //    string.Format(@"SELECT *FROM T_AC_TASKORDER T WHERE T.ORDER_ID='{0}' AND T.WH_CODE='{1}'", orderId, WareHouseCode);
                //using (DataSet ds = DbHelper.GetDataSet(selectSql))
                //{
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //    {
                //        DataRow dr = ds.Tables[0].Rows[0];
                //        taskOrder.OrderId = ConvertHepler.ConvertToInt(dr["ORDER_ID"].ToString());
                //        taskOrder.TaskCode = dr["TASK_CODE"].ToString();
                //        taskOrder.DeviceTaskCode = dr["DEVICE_TASK_CODE"].ToString();
                //        taskOrder.AddTime = ConvertHepler.ConvertToDateTime(dr["ADDTIME"].ToString());
                //        getResult.IsSuccess = true;
                //    }
                //    else
                //    {
                //        getResult.IsSuccess = false;
                //    }
                //}
                getResult.Content = taskOrder;
                getResult.IsSuccess = taskOrder!=null;
            }
            catch (Exception ex)
            {
                getResult.IsSuccess = false;
                getResult.Message = OperateResult.ConvertException(ex);
            }
            return getResult;
        }
    }
}
