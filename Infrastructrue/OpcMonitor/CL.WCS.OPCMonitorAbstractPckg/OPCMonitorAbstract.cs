
using CL.Framework.OPCClientAbsPckg;

namespace CL.WCS.OPCMonitorAbstractPckg
{
	public delegate void MonitorSpecifiedOpcValueCallback();
	public delegate void CallbackContainOpcValue(int value);
	public delegate void CallbackContainOpcBoolValue(bool value);
	public delegate void CallbackContainOpcValueAndAddress(int value, Datablock datablock);
	public delegate void CallbackContainOpcValueAndAddressAndDeviceName(int value, Datablock datablock, string deviceName);
	public interface OPCMonitorAbstract
	{
		/// <summary>
		/// 当OPC读到固定值回调一次解注册
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		/// <param name="endStatus">目的INT值</param>
		/// <param name="callbackcontainopcvalue">回调函数</param>
		void RegisterReadEndValueOnceCallBack(string deviceName, Datablock datablock, int endStatus, CallbackContainOpcValue callbackcontainopcvalue);
		/// <summary>
		/// 当OPC监控器读到从开始状态变化到终止状态时回调委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// /<param name="datablock">OPC地址块</param>
		/// <param name="startStatus">开始INT值</param>
		/// <param name="endStatus">目的INT值</param>
		/// <param name="readopc">设备回调函数</param>
		void RegisterFromStartToEndStatus(string deviceName, Datablock datablock, int startStatus, int endStatus, MonitorSpecifiedOpcValueCallback readopc);

		/// <summary>
		/// 当OPC监控器读到从开始状态变化到终止状态时回调委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// /<param name="datablock">OPC地址块</param>
		/// <param name="startStatus">开始BOOL值</param>
		/// <param name="endStatus">目的BOOL值</param>
		/// <param name="readopc">设备回调函数</param>
		void RegisterFromStartToEndStatus(string deviceName, Datablock datablock, bool startStatus, bool endStatus, MonitorSpecifiedOpcValueCallback readopc);

		/// <summary>
		/// 当OPC监控器读到到终止状态时回调委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		/// <param name="endStatus">设备终止状态。默认为0，代表设备空闲</param>
		/// <param name="readopc">设备回调函数</param>
		void RegisterReadEndStatus(string deviceName, Datablock datablock, int endStatus, MonitorSpecifiedOpcValueCallback readopc);

		/// <summary>
		/// 当OPC监控器读到到终止状态时回调委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="Addr">OPC地址块</param>
		/// <param name="endStatus">设备终止状态。默认为0，代表设备空闲</param>
		/// <param name="readopc">设备回调函数</param>
		void RegisterReadEndStatus(string deviceName, Datablock datablock, int endStatus, CallbackContainOpcValueAndAddressAndDeviceName vad);
		/// <summary>
		/// 当OPC监控器读取到两个值不一致时,回调包含读取到的OPC的值委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		/// <param name="value">注册时的值</param>
		/// <param name="monitervaluechange">设备回调函数</param>
		void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcValue monitervaluechange);

		/// <summary>
		///  当OPC监控器读取到两个值不一致时,回调包含读取到的OPC的值和DB块地址的委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcValueAndAddress monitervaluechange);

		/// <summary>
		///  当OPC监控器读取到两个值不一致时,回调包含读取到的OPC的值和DB块地址的委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcValueAndAddressAndDeviceName monitervaluechange);


		/// <summary>
		/// 当OPC监控器读取到两个bool值不一致时,回调包含读取到的OPC的值委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="Addr">OPC地址块</param>
		/// <param name="value">注册时的值</param>
		/// <param name="monitervaluechange">设备回调函数</param>
		void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcBoolValue monitervaluechange);


		/// <summary>
		/// 当OPC监控器读取到的值不等于初始值时，并且上一个值为初始值时才回调委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		/// <param name="startValue">需要注册的值</param>
		/// <param name="monitervaluechange">设备回调函数，包含OPC读取到的值</param>
		void RegisterNotEqualStartValue(string deviceName, Datablock datablock, int startValue, CallbackContainOpcValue monitervaluechange);

		/// <summary>
		/// 当OPC监控器读取到的值不等于初始值时，并且上一个值为初始值时才回调委托,回调包含读取到的OPC的值和DB块地址的委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		/// <param name="startValue">需要注册的值</param>
		/// <param name="monitervaluechange">设备回调函数，回调包含读取到的OPC的值和DB块地址的委托</param>
		void RegisterNotEqualStartValue(string deviceName, Datablock datablock, int startValue, CallbackContainOpcValueAndAddress monitervaluechange);

		/// <summary>
		/// 当OPC监控器读取到的值不等于初始值时，并且上一个值为初始值时才回调委托,回调包含读取到的OPC的值和DB块地址的委托
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="datablock">OPC地址块</param>
		/// <param name="startValue">需要注册的值</param>
		/// <param name="monitervaluechange">设备回调函数，回调包含读取到的OPC的值和DB块地址的委托</param>
		void RegisterNotEqualStartValue(string deviceName, Datablock datablock, int startValue, CallbackContainOpcValueAndAddressAndDeviceName monitervaluechange);
		/// <summary>
		/// 对OPC注册表  解除注册表，适用于设备禁用
		/// </summary>
		/// <param name="deviceName"></param>
		/// <param name="datablock"></param>
		void ReleaseRegisterList(string deviceName, Datablock datablock);

		/// <summary>
		/// 对OPC注册表  解除注册表，适用于设备禁用
		/// </summary>
		/// <param name="deviceName"></param>
		/// <param name="datablock"></param>
		void ReleaseRegisterBoolValueList(string deviceName, Datablock datablock);

		/// <summary>
		/// 解除设备注册
		/// </summary>
		/// <param name="deviceName"></param>
		void ReleaseDeviceRegister(string deviceName);

		/// <summary>
		/// OPCMonitor开始监听
		/// </summary>
		/// <param name="threadCountOfBusinessHandle">业务处理的线程数</param>
		/// <param name="monitorIntervalTime">监控的间隔时间</param>
		void StartMonitor(int threadCountOfBusinessHandle = 10, int monitorIntervalTime=200);

        /// <summary>
        /// 停止监听
        /// </summary>
        void StopMonitor();
	}
}
