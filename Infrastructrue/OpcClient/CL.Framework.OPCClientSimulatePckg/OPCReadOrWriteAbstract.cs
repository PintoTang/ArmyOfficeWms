using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CL.Framework.OPCClientAbsPckg;


namespace CL.Framework.OPCClientSimulatePckg
{
	/// <summary>
	/// OPC仿真界面读写接口
	/// </summary>
	public interface OPCReadOrWriteAbstract
	{
		/// <summary>
		/// 单个值的读取
		/// </summary>
		/// <param name="deviceName">组名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		void OPC_PassValue(string deviceName, string opcConnection, string itemName,string readType);

		/// <summary>
		/// 多个值的读取
		/// </summary>
		/// <param name="deviceName">组名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		void OPC_PassValueList(string deviceName, List<DeviceAddressInfo> itemName);

		/// <summary>
		/// 多个值的读取
		/// </summary>
		/// <param name="deviceName">组名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		void OPC_PassValueList(string type, string deviceName, List<DeviceAddressInfo> itemName);
	}
}
