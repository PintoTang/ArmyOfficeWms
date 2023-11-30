using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.Model.DataModel
{
	public enum SysStateEnum
	{
		/// <summary>
		/// 离线状态
		/// </summary>
		OFFLINE = 0,	//必须定义default值，BaseDataPool类中可能会被使用
		/// <summary>
		/// 运行状态
		/// </summary>
		RUNNING,
		/// <summary>
		/// 暂停状态
		/// </summary>
		PAUSE
	}
}
