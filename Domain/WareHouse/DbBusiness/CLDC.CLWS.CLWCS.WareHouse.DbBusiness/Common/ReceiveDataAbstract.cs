using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public abstract class ReceiveDataAbstract : DatabaseBusinessAbstract<ReceiveDataModel>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;
        public ReceiveDataAbstract(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public bool IsExistDataBase(ReceiveDataModel data)
        {
            bool isExists = DbHelper.IsExist<ReceiveDataModel>(t => t.HandleStatus == ReceiveDataHandleStatus.UnHandle && t.MethodName == data.MethodName && t.WhCode == WareHouseCode && t.MethodParamValue.ToString()==data.MethodParamValue);
            //List<int> instructIds = data.InstructIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            //foreach (var instructId in instructIds)
            //{
            //    //bool isExists=DbHelper.IsExist<ExtOrder>
            //    //string instructIdStr = instructId.ToString() + ",";
            //    //bool isExists = DbHelper.IsExist<ReceiveDataModel>(t => t.HandleStatus == ReceiveDataHandleStatus.UnHandle && t.MethodName == data.MethodName && t.WhCode == WareHouseCode && t.InstructIds.Contains(instructIdStr));
            //    //if (isExists)
            //    //{
            //    //    return true;
            //    //}
            //}
            return isExists;
            //string sql = string.Format("select count(1) from t_bo_receivedata a where a.dhstatus_id = 1 and a.rd_methodname = '{0}' and a.wh_code = '{1}'  and a.rd_paramvalue ='{2}'", data.MethodName, WareHouseCode, data.MethodParamValue);
            //int count = Convert.ToInt32(DbHelper.QuerySqlScalar(sql));
            //return count > 0;
        }

        public void UpdateStatusWhenProcessReceiveData(string dataSourceID, string errorMsg, int intStatusId)
        {
            bool reInfo = DbHelper.Update<ReceiveDataModel>(t => new ReceiveDataModel
            {
                HandleStatus = (ReceiveDataHandleStatus)intStatusId,
                Note=errorMsg,
            }, t => t.GuidId == dataSourceID)>0;
            //String strSql = string.Format("update t_bo_receivedata set dhstatus_id={2} ,rd_note='{1}' where rd_guid='{0}'", dataSourceID, errorMsg, intStatusId);
            //bool reInfo = DbHelper.ExecuteNonQuery(strSql);
            if (!reInfo)
            {
                throw (new Exception("更新数据库的表t_bo_receivedata时出错。"));
            }
        }

        public abstract List<ReceiveDataModel> SelectData(ReceiveDataFilter filter,out int totalCount);

        public abstract long GetTotalCount();

    }
}
