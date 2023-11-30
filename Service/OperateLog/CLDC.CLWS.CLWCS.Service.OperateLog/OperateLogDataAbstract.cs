using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog.Model;

namespace CLDC.CLWS.CLWCS.Service.OperateLog
{
    public abstract class OperateLogDataAbstract : DatabaseBusinessAbstract<OperateLogModel>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;
        public override OperateResult IsExist(OperateLogModel data)
        {
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult Update(OperateLogModel data)
        {
            return OperateResult.CreateSuccessResult();
        }

        public OperateLogDataAbstract(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        public abstract long GetTotalCount();
        public abstract List<OperateLogModel> SelectData(OperateLogFilter filterModel,out int totalCount);
    }
}
