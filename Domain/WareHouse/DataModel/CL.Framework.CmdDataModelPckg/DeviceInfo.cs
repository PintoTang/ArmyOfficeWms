using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CL.Framework.CmdDataModelPckg
{
	/// <summary>
	/// 设备信息
	/// </summary>
	public class DeviceInfo
	{
		/// <summary>
		/// <para>无效值</para>
		/// </summary>
		public const int INVALID_VALUE = -0x7FFFFFFF;

		/// <summary>
		/// 设备状态
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 工作模式
		/// </summary>
		public int Mode { get; set; }
		/// <summary>
		/// 故障代码
		/// </summary>
		public int ErrorCode { get; set; }

		private int tunnel = INVALID_VALUE;
		/// <summary>
		/// 设备所在巷道
		/// </summary>
		public int Tunnel
		{
			get
			{
				return tunnel;
			}

			set
			{
				tunnel = value;
			}
		}

		private int row = INVALID_VALUE;
		/// <summary>
		/// 设备所在货架层
		/// </summary>
		public int Row 
		{
			get
			{
				return row;
			}

			set
			{
				row = value;
			}
		}
		/// <summary>
		/// 设备名称
		/// </summary>
		public DeviceName DeviceName { get; set; }

		private List<int> finishOrderIdList = new List<int>();
		/// <summary>
		/// 该设备已搬运完成，但尚未通知过MS的指令的ID
		/// </summary>
		public List<int> FinishOrderIdList
		{
			get { return finishOrderIdList; }
			set { finishOrderIdList = value; }
		}

		private List<string> doneInstructPackageBarcodes = new List<string>();
		/// <summary>
		/// 尚未通知过的该设备已搬完的包装条码
		/// </summary>
		public List<string> DoneInstructPackageBarcodes
		{
			get { return doneInstructPackageBarcodes; }
			set { doneInstructPackageBarcodes = value; }
		}
	}
}
