using System;
using System.Collections.Generic;

namespace CL.Framework.OPCClientImpPckg
{
	/// <summary>
	/// OPC通信接口
	/// </summary>
	public interface IOpcCommunicationAbstract:IDisposable
	{
		/// <summary>
		/// 关闭服务对象连接
		/// </summary>
		void Close();

		/// <summary>
		/// 添加组
		/// </summary>
		/// <param name="groupName">组名称</param>
		void AddGroup(string groupName);

		/// <summary>
		/// 添加单个项
		/// </summary>
		/// <param name="groupName">组名称</param>
		/// <param name="itemName">项名称</param>
		void AddItem(string groupName, string itemName);

		/// <summary>
		/// 单个值的读取
		/// </summary>
		/// <param name="groupName">组名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		DataItem Read(string groupName, string itemName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="itemNameList"></param>
		/// <returns></returns>
		List<DataItem> Read(string groupName, List<string> itemNameList);

		/// <summary>
		/// 单个值的写入
		/// </summary>
		/// <param name="groupName">组名称</param>
		/// <param name="itemName">项名称</param>
		/// <param name="value">修改后的值</param>
		void Write(string groupName, string itemName, object value);

		/// <summary>
		/// 多个值写入
		/// </summary>
		/// <param name="groupName">组名称</param>
		/// <param name="itemValueDic">项值字典</param>
		void Write(string groupName, Dictionary<string, object> itemValueDic);

        /// <summary>
        /// 多个值写入
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemValueDic">项值字典</param>
        void Write(string groupName, Dictionary<string, int> itemValueDic);
	}
}
