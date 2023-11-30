using System.Collections.Generic;
using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public abstract class LiveDataAbstract : DatabaseBusinessAbstract<LiveData>
    {
        protected string WareHouseCode = SystemConfig.Instance.WhCode;

        public LiveDataAbstract(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        public abstract bool IsExistLiveData(LiveData data);

        /// <summary>
        /// 根据设备编号获取所有的实时数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public abstract OperateResult<List<LiveData>> GetAllLiveData(int deviceId);
        /// <summary>
        /// 删除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract OperateResult DeleteLiveData(LiveData data);


        public abstract OperateResult ClearLiveData(int deviceId);


    }
}
