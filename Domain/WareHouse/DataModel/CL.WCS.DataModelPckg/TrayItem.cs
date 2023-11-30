using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 托盘信息
    /// </summary>
	public class TrayItem : Item
	{
		/// <summary>
		/// 空托盘数量
		/// </summary>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// 托盘类型：1100mm托盘,1400mm托盘
		/// </summary>
		[JsonProperty("TRAY_TYPE")]
		public TrayType TrayType
		{
			get;
			set;
		}
	}
}
