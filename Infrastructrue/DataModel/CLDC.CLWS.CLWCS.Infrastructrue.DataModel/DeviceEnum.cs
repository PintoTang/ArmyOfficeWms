using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{


	public enum ControlStateMode
	{
		[Description("自动")]
		Auto=0,
		[Description("手动")]
		Manual
	}


	public enum RunStateMode
	{
		[Description("运行")]
		Run=0,
		[Description("暂停")]
		Pause,
		[Description("停止")]
		Stop,
		[Description("复位")]
		Reset,
		[Description("恢复")]
		Restore
	}

	public enum UseStateMode
	{
		[Description("停用")]
		Disable=0,
		[Description("启用")]
		Enable
	}

	public enum ConnectState
	{
		[Description("在线")]
		Online=0,
		[Description("离线")]
		Offline
	}

	public enum DispatchState
	{
		[Description("可以调度")]
		On=0,
		[Description("不可调度")]
		Off
	}

	/// <summary>
	/// 工作状态
	/// </summary>
	public enum WorkStateMode
	{
		[Description("空闲")]
		Free=0,
		[Description("工作中")]
		Working,
		[Description("忙碌中")]
		Busy,
		[Description("充电")]
		Charging
	}


}
