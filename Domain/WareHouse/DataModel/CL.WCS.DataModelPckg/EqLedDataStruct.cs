using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// LED信息
    /// </summary>
	public class EqLedDataStruct
	{
        /// <summary>
        /// LED 对应的设备编号
        /// </summary>
        public int LED_DEVICEID { get; set; }
		/// <summary>
		/// LED屏幕的GUID
		/// </summary>
		public string LED_Guid { get; set; }
		/// <summary>
		/// 样式ID
		/// </summary>
		public int SHOWSTYLE_ID { get; set; }
		/// <summary>
		/// 控制卡号
		/// </summary>
		public int LED_CardNo{ get; set; }
		/// <summary>
		/// 控制卡类型（EQ3002-I=0、EQ3002-II=1、EQ3002-III=2、EQ2008-I/II=3;#EQ2010-I=4、EQ2008-IE=5、EQ2011=7、EQ2012=8、EQ2008-M=9 #EQ2013=21、EQ2023=22、EQ2033=23）
		/// </summary>
		public int LED_CardType { get; set; }
		/// <summary>
		/// 控制卡通讯模式，串口通讯=0、网路通讯=1
		/// </summary>
		public int LED_CommunicationMode { get; set; }
		/// <summary>
		/// LED屏幕的宽度，取值为：8的倍数
		/// </summary>
		public int LED_ScreenWidth { get; set; }
		/// <summary>
		/// LED屏幕的高度，取值为：8的倍数
		/// </summary>
		public int LED_ScreenHigh { get; set; }
		/// <summary>
		/// 串口波特率
		/// </summary>
		public int LED_SerialBaud { get; set; }
		/// <summary>
		/// 串口号
		/// </summary>
		public int LED_SerialNum { get; set; }
		/// <summary>
		/// LED屏对应的IP地址
		/// </summary>
		public string LED_IpAddress { get; set; }
		/// <summary>
		/// LED屏对应的端口，必须为5005
		/// </summary>
		public int LED_NetPort { get; set; }
		/// <summary>
		/// 显示屏颜色类型:0--单色屏，1--双色屏
		/// </summary>
		public int LED_ColorStyle { get; set; }
		/// <summary>
		/// 超时时间，单位毫秒
		/// </summary>
		public int LED_TimeOut { get; set; }
		/// <summary>
		/// 访问密码
		/// </summary>
		public string LED_PassWord { get; set; } 
		/// <summary>
		/// 是否启用
		/// </summary>
		public bool LED_IsEnabled { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string led_note { get; set; }
        /// <summary>
        /// 库房代码
        /// </summary>
		public string WH_CODE { get; set; }
		/// <summary>
		/// 根据数据库的数据进行格式化成对象
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public static explicit operator EqLedDataStruct (DataRow row)
		{
			EqLedDataStruct pLEDDataStruct = new EqLedDataStruct();
			pLEDDataStruct.LED_Guid = row["led_guid"].ToString();
			pLEDDataStruct.WH_CODE=row["wh_code"].ToString();
			pLEDDataStruct.SHOWSTYLE_ID = string.IsNullOrEmpty(row["showstyle_id"].ToString().Trim())
				? 0 : int.Parse(row["showstyle_id"].ToString().Trim());
			pLEDDataStruct.LED_CardNo = string.IsNullOrEmpty(row["led_cardno"].ToString().Trim())
				? 0 : int.Parse(row["led_cardno"].ToString().Trim());
			pLEDDataStruct.LED_CardType =  string.IsNullOrEmpty(row["led_cardtype"].ToString().Trim())
				? 0 : int.Parse(row["led_cardtype"].ToString().Trim());
			pLEDDataStruct.LED_CommunicationMode = string.IsNullOrEmpty(row["led_communicationmode"].ToString().Trim())
				? 0 : int.Parse(row["led_communicationmode"].ToString().Trim());
			pLEDDataStruct.LED_ScreenWidth = string.IsNullOrEmpty(row["led_screemwidth"].ToString().Trim())
				? 0 : int.Parse(row["led_screemwidth"].ToString().Trim());
			pLEDDataStruct.LED_ScreenHigh = string.IsNullOrEmpty(row["led_screemheight"].ToString().Trim())
				? 0 : int.Parse(row["led_screemheight"].ToString().Trim());
			pLEDDataStruct.LED_SerialBaud = string.IsNullOrEmpty(row["led_serialbaud"].ToString().Trim())
				? 0 : int.Parse(row["led_serialbaud"].ToString().Trim());
			pLEDDataStruct.LED_SerialNum = string.IsNullOrEmpty(row["led_serialnum"].ToString().Trim())
				? 0 : int.Parse(row["led_serialnum"].ToString().Trim());
			pLEDDataStruct.LED_IpAddress = row["led_ipaddress"].ToString();
			pLEDDataStruct.LED_NetPort = string.IsNullOrEmpty(row["led_netport"].ToString().Trim())
				? 0 : int.Parse(row["led_netport"].ToString().Trim());
			pLEDDataStruct.LED_ColorStyle = string.IsNullOrEmpty(row["led_colorstyle"].ToString().Trim())
				? 0 : int.Parse(row["led_colorstyle"].ToString().Trim());
			pLEDDataStruct.LED_TimeOut = string.IsNullOrEmpty(row["led_timeout"].ToString().Trim())
				? 0 : int.Parse(row["led_timeout"].ToString().Trim());
			pLEDDataStruct.LED_PassWord = row["led_password"].ToString();
			pLEDDataStruct.LED_IsEnabled = row["led_isenabled"].ToString() == "0" ? false : true;
			pLEDDataStruct.led_note = row["led_note"].ToString();

			return pLEDDataStruct;
		}
	}
	
}
