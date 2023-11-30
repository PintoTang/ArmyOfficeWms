using CL.Framework.OPCClientAbsPckg;
using CL.WCS.OPCMonitorAbstractPckg;

namespace CL.WCS.OPCMonitorImpPckg
{
	internal class RegisterInfo
	{
		public int IntStartStatus { get; set; }
		public int IntEndStatus { get; set; }
		public int IntopcValue { get; set; }
		public int IntLastOpcReadValue { get; set; }

		public bool BoolEndStatus { get; set; }
		public bool BoolStartStatus { get; set; }
		public bool BoolOpcValue { get; set; }
		public bool BoolLastOpcReadValue { get; set; }

		public Datablock Datablock { get; set; }
		public MonitorType EnumMoniter { get; set; }
		public MonitorSpecifiedOpcValueCallback MonitorSpecifiedOpcValueCallback { get; set; }
		public CallbackContainOpcValue CallbackContainOpcValue { get; set; }
		public CallbackContainOpcValueAndAddress CallbackContainOpcValueAndAddress { get; set; }
		public CallbackContainOpcValueAndAddressAndDeviceName CallbackContainOpcValueAndAddressAndDeviceName { get; set; }
		public CallbackContainOpcBoolValue CallbackContainOpcBoolValue { get; set; }
	}

	internal enum MonitorType
	{
		/// <summary>
		/// OPC监控器读到到终止状态
		/// </summary>
		MonitorOpcReadEndStatusCallback,
		/// <summary>
		/// OPC监控器读到从开始状态变化到终止状态
		/// </summary>
		MonitorFromStartToEndStatusCallback,
		/// <summary>
		/// OPC监控器读取到两个值不一致
		/// </summary>
		MonitorValueChange,
		/// <summary>
		/// OPC监控器读取到的值不等于初始值
		/// </summary>
		MonitorNotEqualStartValueCallback,
		/// <summary>
		/// 当OPC读到固定值回调一次解注册
		/// </summary>
		MonitorReadEndValueOnceCallBack
	}

	internal struct DeviceRegisterInfo
	{
		public RegisterInfo Item;
		public int IntValue;
		public bool BoolValue;
		public string DeviceName;
	}
}
