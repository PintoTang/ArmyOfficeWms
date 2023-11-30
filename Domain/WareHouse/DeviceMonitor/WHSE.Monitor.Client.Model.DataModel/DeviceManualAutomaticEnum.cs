using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.Model.DataModel
{
	public enum DeviceManualAutomaticEnum
	{
		
		/// <summary>
		/// 自动
		/// </summary>
		Automatic = 0,	//必须定义default值，BaseDataPool类中可能会被使用

		/// <summary>
		/// 手动
		/// </summary>
		Manual

	}
}
