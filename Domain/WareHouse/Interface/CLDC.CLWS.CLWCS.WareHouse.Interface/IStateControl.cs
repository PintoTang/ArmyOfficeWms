using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
	/// <summary>
	/// 状态控制接口
	/// </summary>
	public interface IStateControl
	{
		/// <summary>
		/// 当前状态
		/// </summary>
		RunStateMode CurRunState { get; set; }

		/// <summary>
		/// 当前控制状态
		/// </summary>
		ControlStateMode CurControlMode { get; set; }

		/// <summary>
		/// 运行
		/// </summary>
		/// <returns></returns>
		OperateResult Run();
		/// <summary>
		/// 暂停
		/// </summary>
		/// <returns></returns>
		OperateResult Pause();
		/// <summary>
		/// 复位
		/// </summary>
		/// <returns></returns>
		OperateResult Reset();
		
		/// <summary>
		/// 停止
		/// </summary>
		/// <returns></returns>
		OperateResult Stop();

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
	    OperateResult Start();

	}
}
