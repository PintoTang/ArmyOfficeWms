using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common
{
    public abstract class WhAddressDataAbstract : DatabaseBusinessAbstract<WhAddressModel>
    {
        protected readonly string WareHouseCode = SystemConfig.Instance.WhCode;
        public WhAddressDataAbstract(IDbHelper dbHelper) : base(dbHelper)
        {
        }

        public abstract OperateResult<List<WhAddressModel>> GetAllData();

        /// <summary>
        /// 根据Wcs地址获取上层地址
        /// </summary>
        /// <param name="wcsAddr"></param>
        /// <returns></returns>
        public abstract OperateResult<string> GetUpperAddrByWcsAddr(string wcsAddr);
        /// <summary>
        /// 根据Wcs地址获取下层地址
        /// </summary>
        /// <param name="wcsAddr"></param>
        /// <returns></returns>
        public abstract OperateResult<string> GetLowerAddrByWcsAddr(string wcsAddr);
        /// <summary>
        /// 根据上层地址获取WCS地址
        /// </summary>
        /// <param name="upperAddr"></param>
        /// <returns></returns>
        public abstract OperateResult<string> GetWcsAddrByUpperAddr(string upperAddr);
        /// <summary>
        /// 根据WCS地址获取显示名字
        /// </summary>
        /// <param name="wcsAddr"></param>
        /// <returns></returns>
        public abstract OperateResult<string> GetShowNameByWcsAddr(string wcsAddr);
        /// <summary>
        /// 根据上层地址获取显示名字
        /// </summary>
        /// <param name="upperAddr"></param>
        /// <returns></returns>
        public abstract OperateResult<string> GetShowNameByUpperAddr(string upperAddr);

        public abstract List<WhAddressModel> SelectData(WhAddressDataFilter filterModel,out int totalCount);
        public abstract long GetTotalCount();
        public abstract OperateResult Delete(WhAddressModel selectedData);
    }
}
