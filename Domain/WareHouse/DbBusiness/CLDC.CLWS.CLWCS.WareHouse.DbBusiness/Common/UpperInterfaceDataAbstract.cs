using System.Collections.Generic;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

using System;
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
	/// <summary>
	/// 上层接口服务调用数据库操作类
	/// </summary>
    public abstract class UpperInterfaceDataAbstract : DatabaseBusinessAbstract<UpperInterfaceInvoke>
	{
        protected string WareHouseCode = SystemConfig.Instance.WhCode;
        public UpperInterfaceDataAbstract(IDbHelper dbHelper)
            : base(dbHelper)
	    {
	        
	    } 

		/// <summary>
		/// 根据关键信息获取数据
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
        public abstract OperateResult<UpperInterfaceInvoke> GetNotifyElement(string guid);

		/// <summary>
		/// 根据状态获取接口调用信息
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
        public abstract OperateResult<List<UpperInterfaceInvoke>> GetNotifyElement(InvokeStatusMode status);

        public abstract OperateResult<List<UpperInterfaceInvoke>> GetNotifyElement(List<InvokeStatusMode> statusList);

		/// <summary>
		/// 通过接口名获取接口调用信息
		/// </summary>
		/// <param name="methodName"></param>
		/// <returns></returns>
        public abstract OperateResult<List<UpperInterfaceInvoke>> GetNotifyElementByMethodName(string methodName);

        /// <summary>
        /// 通过sql语句获取接口调用信息
        /// </summary>
        /// <param name="where">sql</param>
        /// <returns></returns>
        public abstract OperateResult<List<UpperInterfaceInvoke>> GetNotifyElementByWhere(Expression<Func<UpperInterfaceInvoke, bool>> whereLambda);

	    public abstract long GetTotalCount();
	    public abstract List<UpperInterfaceInvoke> SelectData(UpperInterfaceInvokeFilter filterModel,out int totalCount);
	}
}
