using System;
using System.Collections.Generic;
using System.Text;
using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// Item
    /// </summary>
	public class Item
	{
		/// <summary>
		/// 货物ID
		/// </summary>
		public string ItemId
		{
			set;
			get;
		}
		private string wareHouseId;
		/// <summary>
		/// 仓库ID
		/// </summary>
		[JsonProperty("WARE_ID")]
		public string WarehouseId
		{
			set { wareHouseId = value; }//Webservice需要赋值
			get
			{
				try
				{
					return SystemConfig.Instance.WhCode;
				}
				catch (Exception)
				{
					return wareHouseId;
				}
			}
		}

        /// <summary>
        /// 库区
        /// </summary>
		[JsonProperty("Area")]
		public string Area
		{
			get;
			set;
		}

		/// <summary>
		/// 站台编号
		/// </summary>
		[JsonProperty("STA_ID")]
		public string StationCode
		{
			get;
			set;
		}

		/// <summary>
		/// 传递站台编号
		/// </summary>
		[JsonProperty("WCS_STA_ID")]
		public string TransmitStationId
		{
			get;
			set;
		}

		/// <summary>
		/// 衔接站台编号
		/// </summary>
		[JsonProperty("JOIN_STA_ID")]
		public string TransmitStation
		{
			get;
			set;
		}

		/// <summary>
		/// 出库站台对应的仓库ID
		/// </summary>
		[JsonProperty("Station_WARE_ID")]
		public string StationWareId
		{
			get;
			set;
		}

		/// <summary>
		/// 子托盘ID
		/// </summary>
		[JsonProperty("SUB_TRAY_ID")]
		public string SubTrayId
		{
			get;
			set;
		}

		/// <summary>
		/// 母托盘ID
		/// </summary>
		[JsonProperty("TRAY_ID")]
		public string TrayId
		{
			get;
			set;
		}

		/// <summary>
		/// 货高
		/// </summary>
		[JsonProperty("HIGH_FLAG")]
		public string HighFlag
		{
			get;
			set;
		}

		/// <summary>
		/// 可否入库
		/// </summary>
		[JsonProperty("IN_FLAG")]
		public bool IsAllowIn
		{
			get;
			set;
		}

		/// <summary>
		/// LED显示内容
		/// </summary>
		[JsonProperty("LED_MSG")]
		public string LedMessage
		{
			get;
			set;
		}

		/// <summary>
		/// 巷道号
		/// </summary>
		[JsonProperty("CRA_ID")]
		public string TunnelId
		{
			get;
			set;
		}

		/// <summary>
		/// 仓位
		/// </summary>
		[JsonProperty("LOC_ID")]
		public string Cell
		{
			get;
			set;
		}

		/// <summary>
		/// 异常仓位--用在接口出深浅有
		/// </summary>
		[JsonProperty("ERR_LOC_ID")]
		public string ERR_LOC_ID
		{
			get;
			set;
		}

		/// <summary>
		/// 任务号
		/// </summary>
		[JsonProperty("MIS_ID")]
		public string TaskId
		{
			get;
			set;
		}

		/// <summary>
		/// 优先级
		/// </summary>
		[JsonProperty("PRI_FLAG")]
		public int Priority
		{
			set;
			get;
		}

		/// <summary>
		/// 出库模式
		/// </summary>
		[JsonProperty("MODE_FLAG")]
		public string OutMode
		{
			set;
			get;
		}
		/// <summary>
		/// 校验结果
		/// </summary>
		[JsonProperty("VERIFIED_RESULT")]
		public string VerifyedResult
		{
			set;
			get;
		}

		/// <summary>
		/// 源库位号
		/// </summary>
		[JsonProperty("Source_CellID")]
		public string SourceCellId
		{
			set;
			get;
		}

		/// <summary>
		/// 目的库位号
		/// </summary>
		[JsonProperty("Destination_CellID")]
		public string DestinationCellId
		{
			set;
			get;
		}

		/// <summary>
		/// 越库站台号
		/// </summary>
		[JsonProperty("CD_STA_ID")]
		public string CrossDockStationId
		{
			set;
			get;
		}


		/// <summary>
		/// 数据表GUID
		/// </summary>
	    [JsonProperty("RD_GUID")]
		public string DataSourceID
		{
			set;
			get;
		}

		/// <summary>
		/// 任务区分
		/// (1是全部出，2部分出）
		/// </summary>
		[JsonProperty("BACK_FLAG")]
		public string BackFlag
		{
			set;
			get;
		}

		/// <summary>
		/// 定义Json字符串显示转换为Item对象
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static explicit operator Item(string json)
		{
			return JsonConvert.DeserializeObject<Item>(json);
		}


	}
}
