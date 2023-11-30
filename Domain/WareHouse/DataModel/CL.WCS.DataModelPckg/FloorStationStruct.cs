using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 楼层站台信息
    /// </summary>
 	public class FloorStationStruct
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
		/// 站台优先级,用于同侧有多个入库站台时优先哪个站台先接货,值越高,优先级越高,默认为0
		/// </summary>
		public int FSP_PRIORITY { get; set; }
		/// <summary>
		/// 堆垛机楼层站台对应第三方设备厂商的楼层站台的值:例如村田、冈村
		/// </summary>
		public string FSP_LSNO { get; set; }
		/// <summary>
		/// 楼层站台对应的设备名称
		/// </summary>
		public string FSP_DEVICENAME { get; set; }
		/// <summary>
		/// 楼层站台对应的列号,默认为1,两侧站台堆垛机策略使用
		/// </summary>
		public int FSP_BAYNO { get; set; }
		/// <summary>
		/// 楼层站台对应的仓位的列号
		/// </summary>
		public int FSP_CELLBAY { get; set; }

        /// <summary>
        /// 转换成FloorStationStruct格式
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
		public static explicit operator FloorStationStruct(DataRow row)
		{
			FloorStationStruct pFloorStationStruct = new FloorStationStruct();
			pFloorStationStruct.SD_CODE = row["SD_CODE"].ToString();
			pFloorStationStruct.WH_CODE = row["WH_CODE"].ToString();
			pFloorStationStruct.FSP_PRIORITY = int.Parse(row["FSP_PRIORITY"].ToString());
			pFloorStationStruct.FSP_LSNO = row["FSP_LSNO"].ToString();
			return pFloorStationStruct;
		}
	}
}
