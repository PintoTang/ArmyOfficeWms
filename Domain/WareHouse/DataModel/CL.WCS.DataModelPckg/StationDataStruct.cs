using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 站台信息
    /// </summary>
	public class StationDataStruct
	{
		/// <summary>
		/// 站台代码，规则：站台类型名：排_层_列_库区
		/// </summary>
		public string SD_CODE { get; set; }
		/// <summary>
		/// 仓库代码
		/// </summary>
		public string WH_CODE { get; set; }
		/// <summary>
		/// 站台类型代码
		/// </summary>
		public StationType ST_CODE { get; set; }
		/// <summary>
		/// 站台的功能类型ID
		/// </summary>
		public StationFunctionType SFT_ID { get; set; }
		/// <summary>
		/// LED屏幕的GUID，为空则不存在LED
		/// </summary>
		public string LED_GUID { get; set; }
		/// <summary>
		/// 是否对接生产线，默认为0
		/// </summary>
		public string SD_ISLINKPRODUCTION { get; set; }
		/// <summary>
		/// 所在的物理楼层
		/// </summary>
		public string SD_FLOOR { get; set; }
		/// <summary>
		/// 功能描述
		/// </summary>
		public string SD_FUNCTIONDESCRIPTION { get; set; }
		/// <summary>
		/// 位置描述
		/// </summary>
		public string SD_LOCATIONDESCRIPTION { get; set; }
		/// <summary>
		/// 楼层站台对应村田的楼层站台编号，只针对楼层站台，其他站台类型为空
		/// </summary>
		public string SD_MuratecLSNO { get; set; }

		/// <summary>
		/// 根据数据库的数据进行格式化成对象
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public static explicit operator StationDataStruct(DataRow row)
		{
			StationDataStruct pStationDataStruct = new StationDataStruct();
			pStationDataStruct.LED_GUID = row["LED_GUID"].ToString();
			pStationDataStruct.SD_CODE = row["SD_CODE"].ToString();
			pStationDataStruct.SD_FLOOR = row["SD_FLOOR"].ToString();
			pStationDataStruct.SD_FUNCTIONDESCRIPTION = row["SD_FUNCTIONDESCRIPTION"].ToString();
			pStationDataStruct.SD_ISLINKPRODUCTION = row["SD_ISLINKPRODUCTION"].ToString();
			pStationDataStruct.SD_LOCATIONDESCRIPTION = row["SD_LOCATIONDESCRIPTION"].ToString();
			pStationDataStruct.SD_MuratecLSNO = row["SD_MuratecLSNO"].ToString();
			pStationDataStruct.SFT_ID = (StationFunctionType)Enum.ToObject(typeof(StationFunctionType), Int32.Parse(row["SFT_ID"].ToString()));
			pStationDataStruct.ST_CODE = (StationType)Enum.Parse(typeof(StationType), row["ST_CODE"].ToString());
			pStationDataStruct.WH_CODE = row["WH_CODE"].ToString();
			return pStationDataStruct;
		}

	}

	/// <summary>
	/// 站台的功能类型：出库、入库、出入库、拣选、缓存、拆码盘机
	/// </summary>
	public enum StationFunctionType
	{
		/// <summary>
		/// 入库
		/// </summary>
		InWH = 1,
		/// <summary>
		/// 出库
		/// </summary>
		OutWH = 2,
		/// <summary>
		/// 即可做入库也可以做出库
		/// </summary>
		InAndOutWH = 3,
		/// <summary>
		/// 拣选
		/// </summary>
		Picking = 4,
		/// <summary>
		/// 缓存
		/// </summary>
		CacheStation = 5,
		/// <summary>
		/// 拆码盘机
		/// </summary>
		PalletizingStation = 6
	}

	/// <summary>
	/// 站台类型，如楼层站台，出入库站台，拣选站台，出口站台，关键节点站台等
	/// </summary>
	public enum StationType
	{
		/// <summary>
		/// 入库站台Entrance
		/// </summary>
		A,
		/// <summary>
		/// 出口站台Exit
		/// </summary>
		B,
		/// <summary>
		/// 出入库站台InAndOutStation
		/// </summary>
		C,
		/// <summary>
		/// 楼层站台FloorStation
		/// </summary>
		D,
		/// <summary>
		/// 关键节点站台KeyNodeStation
		/// </summary>
		E,
		/// <summary>
		/// 拣选站台PickingStation
		/// </summary>
		F,
		/// <summary>
		/// 缓存工位站台CacheStation
		/// </summary>
		G,
		/// <summary>
		/// 拆码盘机工位PalletizingStation
		/// </summary>
		H,
		/// <summary>
		/// 分配站台AllocateStation
		/// </summary>
		I,
        /// <summary>
        /// 开箱机工位
        /// </summary>
        J,
        /// <summary>
        /// 货架站台
        /// </summary>
        K
	}

	/// <summary>
	/// 托盘类型：1100mm托盘,1400mm托盘
	/// </summary>
	public enum TrayType
	{
		/// <summary>
		/// 1100mm托盘
		/// </summary>
		_1100mm = 1100,

	    /// <summary>
		/// 1400mm托盘
		/// </summary>
		_1400mm = 1400
	}

}
