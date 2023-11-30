using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model
{
    public class DeviceFaultRecCmd
    {
		/// <summary>
		/// 故障编码
		/// </summary>
		public string FaultCode
		{
			get;
			set;
		}

		/// <summary>
		/// 故障名称
		/// </summary>
		public string FaultName
		{
			get;
			set;
		}

		/// <summary>
		/// 故障类型
		/// </summary>
		public int? FaultType
		{
			get;
			set;
		}

		/// <summary>
		/// 故障来源
		/// </summary>
		public int? FaultSc
		{
			get;
			set;
		}

		/// <summary>
		/// 故障等级
		/// </summary>
		public int? FaultGrade
		{
			get;
			set;
		}

		/// <summary>
		/// 故障类型
		/// </summary>
		public int? FaultClass
		{
			get;
			set;
		}

		/// <summary>
		/// 故障描述
		/// </summary>
		public string FaultDec
		{
			get;
			set;
		}

		/// <summary>
		/// 设备编号
		/// </summary>
		public int? DeviceID
		{
			get;
			set;
		}

		/// <summary>
		/// 设备名称
		/// </summary>
		public string DeviceName
		{
			get;
			set;
		}

		/// <summary>
		/// 故障产生时间
		/// </summary>
		public DateTime? FaultCreatTime
		{
			get;
			set;
		}

		/// <summary>
		/// 记录时间
		/// </summary>
		public DateTime? RecCeateTime
		{
			get;
			set;
		}
		/// <summary>
		/// 统计分析标记0：未进行统计分析;1:已经统计分析
		/// </summary>
		public int? AnalysisMark
		{
			get;
			set;
		}
		/// <summary>
		/// 设备类型ID
		/// </summary>
		public int? DeviceType
		{
			get;
			set;
		}
	}
}
