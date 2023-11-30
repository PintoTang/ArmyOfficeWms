using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
	/// <summary>
	/// 数据查询
	/// </summary>
	public class RobotReadyRecordQueryInput: BaseQueryInput<RobotReadyRecord>
	{

		/// <summary>
		/// Id主键
		/// </summary>
		public int? Id
		{
			get;
			set;
		}

		/// <summary>
		/// 当前任务号
		/// </summary>
		public string OrderNO
		{
			get;
			set;
		}

		/// <summary>
		/// 容器1条码
		/// </summary>
		public string ContainerNO1
		{
			get;
			set;
		}

		/// <summary>
		/// 地址1
		/// </summary>
		public string Addr1
		{
			get;
			set;
		}

		/// <summary>
		/// 地址1是否就绪：0-未就绪；1-已就绪
		/// </summary>
		public int? Ready1
		{
			get;
			set;
		}

		/// <summary>
		/// 就绪时间
		/// </summary>
		public DateTime? ReadyTime1
		{
			get;
			set;
		}

		/// <summary>
		/// 地址2
		/// </summary>
		public string Addr2
		{
			get;
			set;
		}

		/// <summary>
		/// 地址2是否就绪：0-未就绪；1-已就绪
		/// </summary>
		public int? Ready2
		{
			get;
			set;
		}

		/// <summary>
		/// 容器2条码
		/// </summary>
		public string ContainerNO2
		{
			get;
			set;
		}

		/// <summary>
		/// 就绪时间
		/// </summary>
		public DateTime? ReadyTime2
		{
			get;
			set;
		}

		/// <summary>
		/// 状态：0-未开始；1-执行中；2-已完成
		/// </summary>
		public int? Status
		{
			get;
			set;
		}
	}
}


