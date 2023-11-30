using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public sealed class RobotReadyRecordForSqlSugar : RobotReadyRecordDataAbstract
    {
        public RobotReadyRecordForSqlSugar(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        #region 私有方法
        private Expression<Func<RobotReadyRecord, bool>> KeyToWhere(string key)
        {
            return t => t.OrderNO==key;
        }
        #endregion
        public override RobotReadyRecord GetRobotReadyRecord(RobotReadyRecordQueryInput query)
        {
            var where = query.FilterWhereLambda(query.WhereLambda, query.Key, KeyToWhere);

            RobotReadyRecord item= DbHelper.Query(where);
            return item;
        }

        public override List<RobotReadyRecord> GetRobotReadyRecordList(RobotReadyRecordQueryInput query)
        {
            var where = query.FilterWhereLambda(query.WhereLambda, query.Key, KeyToWhere);

            List<RobotReadyRecord> items = DbHelper.QueryList(where,query.FullSortFields);
            return items;
        }

        public override OperateResult Insert(RobotReadyRecord data)
        {
            int id=DbHelper.AddReturnIdentity(data);
            if (id == 0)
            {
                return OperateResult.CreateFailedResult("保存失败");
            }
            data.Id = id;
            return OperateResult.CreateSuccessResult("保存成功");
        }

        public override OperateResult IsExist(RobotReadyRecord data)
        {
            return OperateResult.CreateFailedResult("");
        }

        public override OperateResult Update(RobotReadyRecord data)
        {
            var update=data.ToUpdateLambda();
            bool res=DbHelper.Update(update, t => t.Id == data.Id)>0;
            if (!res)
            {
                return OperateResult.CreateFailedResult("修改失败");
            }
            return OperateResult.CreateSuccessResult("修改成功");
        }

        public override int GetRobotReadyRecordsCount(RobotReadyRecordQueryInput query)
        {
            var where = query.FilterWhereLambda(query.WhereLambda,query.Key, KeyToWhere);
            int count = DbHelper.QueryCount(where);
            return count;
        }
    }
}
