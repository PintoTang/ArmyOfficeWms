using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.DataModelPckg;
using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public abstract class LiveStatusAbstract : DatabaseBusinessAbstract<LiveStatusData>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;

        public LiveStatusAbstract(IDbHelper dbHelper):base(dbHelper)
        {
        }        

        public abstract OperateResult<LiveStatusData> GetDeviceLiveDataByDeviceId(int Id);

    }
}
