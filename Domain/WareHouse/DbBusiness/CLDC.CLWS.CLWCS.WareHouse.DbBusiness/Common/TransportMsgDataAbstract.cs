using CL.WCS.SystemConfigPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using CL.WCS.SystemConfigPckg.Model;
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    /// <summary>
    /// 搬运信息数据操作虚类
    /// </summary>
    public abstract class TransportMsgDataAbstract : DatabaseBusinessAbstract<TransportMsgModel>
    {
        protected TransportMsgDataAbstract(IDbHelper dbHelper)
            : base(dbHelper)
        {

        }
        protected string WareHouseCode = SystemConfig.Instance.WhCode;

        /// <summary>
        /// 通过拥有者进行获取搬运信息
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public abstract OperateResult<List<TransportMsgModel>> GetTransportDataByOwner(int ownerId);

        public abstract OperateResult<List<TransportMsgModel>> GetTransportData(int ownerId, List<TransportResultEnum> status);

        public abstract OperateResult<List<TransportMsgModel>> GetTransportData(int pageIndex, int pageSize, string orderBy, Expression<Func<TransportMsgModel, bool>> whereLambda,out int totalCount);

        public abstract OperateResult<List<TransportMsgModel>> GetTransportDataByStatus(List<TransportResultEnum> status);


    }
}
