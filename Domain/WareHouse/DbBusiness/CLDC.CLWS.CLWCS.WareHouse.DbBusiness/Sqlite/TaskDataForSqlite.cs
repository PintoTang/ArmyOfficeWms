using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class TaskDataForSqlite : TaskDataAbstract
    {
        public TaskDataForSqlite(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult IsExist(TaskModel data)
        {
            OperateResult isExistResult = OperateResult.CreateFailedResult();
            try
            {
                string[] sql = new string[1];
                sql[0] = string.Format("SELECT * FROM T_AC_TASK  WHERE TASK_CODE='{0}' AND WH_CODE='{1}'", data.TaskCode, data.WhCode);
                DataSet ds = DbHelper.GetDataSet(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    isExistResult.IsSuccess = true;
                    return isExistResult;
                }
                isExistResult.IsSuccess = false;
                return isExistResult;
            }
            catch (Exception ex)
            {
                isExistResult.IsSuccess = true;
                isExistResult.Message = OperateResult.ConvertException(ex);
            }
            return isExistResult;
        }

        public override OperateResult Update(TaskModel data)
        {
            OperateResult updateResult = OperateResult.CreateFailedResult();
            try
            {
                var sql = string.Format("UPDATE T_AC_TASK SET TASK_PROCESS_STATUS={0} WHERE TASK_CODE='{1}' AND WH_CODE='{2}'", (int)data.TaskProcessStatus, data.TaskCode, data.WhCode);
                int result = DbHelper.ExecuteNonQuery(new[] { sql });
                if (result <= 0)
                {
                    updateResult.IsSuccess = false;
                    updateResult.Message = string.Format("更新数据失败，更新语句：{0}", sql);
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

        public override OperateResult Insert(TaskModel data)
        {
            OperateResult insertResult = OperateResult.CreateFailedResult();
            try
            {

                string sql = string.Format("INSERT INTO T_AC_TASK(TASK_CODE,WH_CODE,TASK_TYPE,TASK_PROCESS_STATUS,ADDTIME) VALUES('{0}','{1}','{2}','{3}',{4})", data.TaskCode, data.WhCode,(int)data.TaskType, (int)data.TaskProcessStatus, "current_timestamp");
               int result = DbHelper.ExecuteNonQuery(new[] { sql });
                if (result != 2)
                {

                    insertResult.IsSuccess = false;
                    insertResult.Message = "插入任务编号：" + data.TaskCode + "数据库时出错,插入语句为：" + string.Join(";", sql);
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
    }
}
