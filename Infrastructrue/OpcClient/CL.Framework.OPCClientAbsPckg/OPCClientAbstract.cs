using System.Collections.Generic;

namespace CL.Framework.OPCClientAbsPckg
{
	/// <summary>
	/// OPC单个写命令上报
	/// </summary>
	/// <param name="deviceName">设备名</param>
	/// <param name="itemName">项名</param>
	/// <param name="value"></param>
	public delegate void OPCWriteBoardSingle(string deviceName, string itemName, string value);
	/// <summary>
	/// OPC多个写命令上报
	/// </summary>
	/// <param name="deviceName"></param>
	/// <param name="Inv"></param>
	public delegate void OPCWriteBoardMulti(string deviceName, Dictionary<string, object> Inv);
	/// <summary>
	/// OPC单个读命令上报
	/// </summary>
	/// <param name="deviceName"></param>
	/// <param name="opcReadAddress"></param>
	/// <param name="opcValue"></param>
	public delegate void OPCReadBoardSingle(string deviceName, string opcReadAddress, string opcValue);
	/// <summary>
	/// OPC多个读命令上报
	/// </summary>
	/// <param name="opcReadList"></param>
	/// <param name="opcReturnValue"></param>
	public delegate void OPCReadBoardMtlti(List<string> opcReadList, List<int> opcReturnValue);

	/// <summary>
	/// OPCClient接口
	/// </summary>
	public abstract class OPCClientAbstract
	{
		/// <summary>
		/// 无效值
		/// </summary>
		public const int INVALID_VALUE = -0x7FFFFFFF;

		protected bool isPrintRealTimeReadLog = true;

		/// <summary>
		/// 是否打印实时读日志
		/// </summary>
		public bool IsPrintRealTimeReadLog
		{
			set { isPrintRealTimeReadLog = value; }
		}

		/// <summary>
		/// OPC单个写命令上报事件
		/// </summary>
		public abstract event OPCWriteBoardSingle OPCWriteBoardSingleEvent;
		/// <summary>
		/// OPC多个写命令上报事件
		/// </summary>
		public abstract event OPCWriteBoardMulti OPCWriteBoardMultiEvent;
		/// <summary>
		/// OPC单个读命令上报事件
		/// </summary>
		public abstract event OPCReadBoardSingle OPCReadBoardSingleEvent;
		/// <summary>
		/// OPC多个读命令上报事件
		/// </summary>
		public abstract event OPCReadBoardMtlti OPCReadBoardMultiEvent;

		/// <summary>
		/// 字符串的读取
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		public abstract string ReadString(string deviceName, string itemName);

        /// <summary>
        /// 浮点型的写入
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns></returns>
        public abstract float ReadFloat(string deviceName,string itemName);

		/// <summary>
		/// bool类型的读取
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		public abstract bool ReadBool(string deviceName, string itemName);

		/// <summary>
		/// 单个值的读取
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemName">项名称</param>
		/// <returns>返回读取结果</returns>
		public abstract int Read(string deviceName, string itemName);

		/// <summary>
		/// 多个值的读取
		/// </summary>
		/// <param name="deviceInfoList"></param>
		/// <returns>返回读取结果</returns>
		public abstract List<int> Read(List<DeviceAddressInfo> deviceInfoList);

		/// <summary>
		/// 多个值的读取
		/// </summary>
		/// <param name="deviceInfoList"></param>
		/// <returns>返回读取结果</returns>
		public abstract List<bool> ReadBoolList(List<DeviceAddressInfo> deviceInfoList);

		/// <summary>
		/// 单个值的写入
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemName">项名称</param>
		/// <param name="value">修改后的值</param>
		public abstract void Write(string deviceName, string itemName, int value);

		/// <summary>
		/// 多个值写入
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemValueDic">项值集合</param>
		public abstract void Write(string deviceName, Dictionary<string, int> itemValueDic);

		/// <summary>
		/// 多个值写入
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemValueDic">项值集合</param>
		public abstract void Write(string deviceName, Dictionary<string, object> itemValueDic);



		/// <summary>
		///  bool类型的写入
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemName">项名称</param>
		/// <param name="value">修改后的值</param>
		public abstract void Write(string deviceName, string itemName, bool value);

        /// <summary>
        /// string类型的写入
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">修改后的值</param>
        public abstract void Write(string deviceName, string itemName, string value);

        /// <summary>
        /// float类型的写入
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">修改后的值</param>
        /// 
        public abstract void Write(string deviceName, string itemName, float value);


        /// <summary>
        ///新增 object类型的写入 by zhangxing 2010-04-24 (底层本支持Value为 Object类型)
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">修改后的值</param>
        /// 
        public abstract void Write(string deviceName, string itemName, object value);

		/// <summary>
		/// 暂停OPC服务
		/// </summary>
		public abstract void PauseOpcService();
		/// <summary>
		/// 恢复OPC服务
		/// </summary>
		public abstract void RecoveryOpcService();

        /// <summary>
        /// 获取OpcClient的工厂
        /// </summary>
        /// <returns></returns>
	    public abstract IOpcClientFactory GetFactory();
	}

}
