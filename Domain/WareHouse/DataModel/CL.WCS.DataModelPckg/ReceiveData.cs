using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 接收表数据信息
    /// </summary>
	public class ReceiveData
	{
        /// <summary>
        /// 接收数据ID
        /// </summary>
		public string RD_GUID { get; set; }
        /// <summary>
        /// 库房代码
        /// </summary>
		public string WH_CODE { get; set; }
        /// <summary>
        /// 数据出库状态
        /// </summary>
		public int DHSTATUS_ID { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
		public string RD_METHODNAME { get; set; }
        /// <summary>
        /// json值
        /// </summary>
		public string RD_PARAMVALUE { get; set; }
        /// <summary>
        /// 接收数据时间
        /// </summary>
		public DateTime? RD_HANDLERDATE { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
		public string RD_NOTE { get; set; }
        /// <summary>
        /// 转换成ReceiveData格式
        /// </summary>
		public static explicit operator ReceiveData(DataRow row)
		{
			ReceiveData receiveData = new ReceiveData();
			receiveData.RD_GUID = row["RD_GUID"].ToString();
			receiveData.WH_CODE = row["WH_CODE"].ToString();
			receiveData.DHSTATUS_ID = string.IsNullOrEmpty(row["DHSTATUS_ID"].ToString().Trim())
				? 0 : int.Parse(row["DHSTATUS_ID"].ToString().Trim());
			receiveData.RD_METHODNAME = row["RD_METHODNAME"].ToString();
			receiveData.RD_PARAMVALUE = row["RD_PARAMVALUE"].ToString();
			receiveData.RD_NOTE = row["rd_note"].ToString();
			return receiveData;
		}

	}
}
