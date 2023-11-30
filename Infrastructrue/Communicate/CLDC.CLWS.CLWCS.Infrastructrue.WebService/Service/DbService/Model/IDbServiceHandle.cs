using System.Data;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model
{

	/// <summary>
	/// 对接外部系统数据的处理接口
	/// </summary>
    public interface IDbServiceHandle
    {
		/// <summary>
		/// 具体业务处理
		/// </summary>
		/// <param name="row"></param>
		/// <param name="isAsync"></param>
		void BusinessProcessData(DataRow row, bool isAsync);



    }
}
