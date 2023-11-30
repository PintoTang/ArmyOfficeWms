using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CL.WCS.DataModelPckg
{
	/// <summary>
	///  搬运设备详细信息包含堆垛机，RGV，提升机
	/// </summary>
	public class T_BD_DeviceData : DeviceInfo
	{
		/// <summary>
		/// 设备代码
		/// </summary>
		 string DD_Code { get; set; }
		/// <summary>
		/// 仓库代码
		/// </summary>
		public string WH_Code { get; set; }
		/// <summary>
		/// 设备名称 
		/// </summary>
		public string DD_Name { get; set; }
		/// <summary>
		/// 设备类型.CRA:堆垛机，STA:站台等
		/// </summary>
		public string DT_Code { get; set; }
		/// <summary>
		///  生效：ON。等同于基类的设备状态属性Status
		/// </summary>
		 string SS_Code { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
		public string DD_Note { get; set; }
		/// <summary>
		/// 判断是否故障的DB地址
		/// </summary>
		public string DD_ErrorDBAddress { get; set; }
		/// <summary>
		/// 厂家的堆垛机编号
		/// </summary>
		public string DD_MuratecCraneNO { get; set; }
		/// <summary>
		/// 堆垛机通信端口
		/// </summary>
		public int DD_MuratecCranePort { get; set; }
		/// <summary>
		/// 堆垛机TS服务器IP
		/// </summary>
		public string DD_MuratecCraneIP { get; set; }
        /// <summary>
        /// 堆垛机远程通信端口
        /// </summary>
        public int DD_MuratecCraneRemotePort { get; set; }
        /// <summary>
        /// 根据数据库的数据进行格式化成对象
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
		public static explicit operator T_BD_DeviceData(DataRow row)
		{
			T_BD_DeviceData info = new T_BD_DeviceData();
			info.DD_Code = row["DD_Code"].ToString();
			info.WH_Code = row["WH_Code"].ToString();
			info.DD_Name = row["DD_Name"].ToString();
			info.DT_Code = row["DT_Code"].ToString();
			info.SS_Code = row["SS_Code"].ToString();
			info.DD_Note = row["DD_Note"].ToString();
			info.DD_ErrorDBAddress = row["DD_ErrorDBAddress"].ToString();
			info.DD_MuratecCraneNO = row["DD_MuratecCraneNO"].ToString();
			info.DD_MuratecCraneIP = row["DD_MuratecCraneIP"].ToString();
			int port;
			int.TryParse(row["DD_MuratecCranePort"].ToString(), out port);
			info.DD_MuratecCranePort = port;
            int remoteport;
			int.TryParse(row["DD_MuratecCraneRemotePort"].ToString(), out remoteport);
            info.DD_MuratecCraneRemotePort = remoteport;
			//-----------DeviceStatusEnum
			info.DeviceName = new DeviceName(info.DD_Code);
			info.Status = info.SS_Code == "ON" ? 0 : 1;
			return info;
		}
	}
}
