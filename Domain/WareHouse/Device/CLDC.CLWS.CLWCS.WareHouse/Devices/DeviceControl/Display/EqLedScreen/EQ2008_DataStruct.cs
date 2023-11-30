using System.Runtime.InteropServices;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.EqLedScreen
{
	/// <summary>
	/// 节目区域参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_PartInfo
	{
		/// <summary>
		/// 窗口的起点X
		/// </summary>
		public int iX;
		/// <summary>
		/// 窗口的起点Y
		/// </summary>            
		public int iY;
		/// <summary>
		/// 窗体的宽度
		/// </summary>
		public int iWidth;
		/// <summary>
		/// 窗体的高度
		/// </summary>
		public int iHeight;
		/// <summary>
		/// 边框的样式
		/// </summary>
		public int iFrameMode;
		/// <summary>
		/// 边框颜色
		/// </summary>
		public int FrameColor;
	}

	/// <summary>
	/// 字体参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_FontSet
	{
		/// <summary>
		/// 字体的名称
		/// </summary>
		public string strFontName;
		/// <summary>
		/// 字体的大小
		/// </summary>
		public int iFontSize;
		/// <summary>
		/// 字体是否加粗
		/// </summary>
		public bool bFontBold;
		/// <summary>
		/// 字体是否是斜体
		/// </summary>
		public bool bFontItaic;
		/// <summary>
		/// 字体是否带下划线
		/// </summary>
		public bool bFontUnderline;
		/// <summary>
		/// 字体的颜色
		/// </summary>
		public int colorFont;
		/// <summary>
		/// 左右对齐方式，0－ 左对齐，1－居中，2－右对齐
		/// </summary>
		public int iAlignStyle;
		/// <summary>
		/// 上下对齐方式，0-顶对齐，1-上下居中，2-底对齐 
		/// </summary>
		public int iVAlignerStyle;
		/// <summary>
		/// 行间距
		/// </summary>
		public int iRowSpace;
	}

	/// <summary>
	/// 动画方式参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_MoveSet
	{
		/// <summary>
		/// 节目变换方式
		/// </summary>
		public int iActionType;
		/// <summary>
		/// 节目的播放速度
		/// </summary>    
		public int iActionSpeed;
		/// <summary>
		/// 是否需要清除背景
		/// </summary>       
		public bool bClear;
		/// <summary>
		/// 在屏幕上停留的时间
		/// </summary> 
		public int iHoldTime;
		/// <summary>
		/// 清除显示屏的速度
		/// </summary>
		public int iClearSpeed;
		/// <summary>
		/// 节目清除的变换方式
		/// </summary>
		public int iClearActionType;
		/// <summary>
		/// 每帧时间
		/// </summary>
		public int iFrameTime;
	}

	/// <summary>
	/// 日期时间区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_DateTime
	{
		/// <summary>
		/// 分区信息
		/// </summary
		public User_PartInfo PartInfo;
		/// <summary>
		/// 背景颜色
		/// </summary
		public int BkColor;
		/// <summary>
		/// 字体设置
		/// </summary
		public User_FontSet FontInfo;
		/// <summary>
		/// 显示风格 0－"3/10/2006 星期六10:20:30",1－"2006-03-10星期六10:20:30",2－"2006年3月10日 星期六10点20分30秒"
		/// </summary
		public int iDisplayType;
		/// <summary>
		/// 添加显示文字
		/// </summary
		public string chTitle;
		/// <summary>
		/// 年份位数0－4,1－2位
		/// </summary
		public bool bYearDisType;
		/// <summary>
		/// 单行还是多行,0－单行1－多行
		/// </summary
		public bool bMulOrSingleLine;
		/// <summary>
		/// 是否显示年
		/// </summary
		public bool bYear;
		/// <summary>
		/// 是否显示月
		/// </summary
		public bool bMouth;
		/// <summary>
		/// 是否显示天
		/// </summary
		public bool bDay;
		/// <summary>
		/// 是否显示星期
		/// </summary
		public bool bWeek;
		/// <summary>
		/// 是否显示小时
		/// </summary
		public bool bHour;
		/// <summary>
		/// 是否显示分钟
		/// </summary
		public bool bMin;
		/// <summary>
		/// 是否显示秒
		/// </summary
		public bool bSec;
	}

	/// <summary>
	/// 单行文本区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_SingleText
	{
		/// <summary>
		/// 显示内容
		/// </summary>
		public string chContent;
		/// <summary>
		/// 分区信息
		/// </summary>
		public User_PartInfo PartInfo;
		/// <summary>
		/// 背景颜色
		/// </summary>
		public int BkColor;
		/// <summary>
		/// 字体设置
		/// </summary>
		public User_FontSet FontInfo;
		/// <summary>
		/// 动作方式设置
		/// </summary>
		public User_MoveSet MoveSet;
	}

	/// <summary>
	/// 文本区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_Text
	{
		/// <summary>
		/// 显示内容
		/// </summary>
		public string chContent;
		/// <summary>
		/// 分区信息
		/// </summary>
		public User_PartInfo PartInfo;
		/// <summary>
		/// 背景颜色
		/// </summary>
		public int BkColor;
		/// <summary>
		/// 字体设置
		/// </summary>
		public User_FontSet FontInfo;
		/// <summary>
		/// 动作方式设置
		/// </summary>
		public User_MoveSet MoveSet;
	}

	/// <summary>
	/// 计时区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_Timer
	{
		public User_PartInfo PartInfo;	//分区信息
		public int BkColor;			    //背景颜色
		public User_FontSet FontInfo;	//字体设置
		public int ReachTimeYear;		//到达年
		public int ReachTimeMonth;	    //到达月
		public int ReachTimeDay;		//到达日
		public int ReachTimeHour;		//到达时
		public int ReachTimeMinute;	    //到达分
		public int ReachTimeSecond;	    //到达秒
		public bool bDay;				//是否显示天 0－不显示 1－显示
		public bool bHour;				//是否显示小时
		public bool bMin;				//是否显示分钟
		public bool bSec;				//是否显示秒
		public bool bMulOrSingleLine;	//单行还是多行
		public string chTitle;			//添加显示文字
	}

	/// <summary>
	/// 温度区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_Temperature
	{
		/// <summary>
		/// 分区信息
		/// </summary>
		public User_PartInfo PartInfo;
		/// <summary>
		/// 背景颜色
		/// </summary>
		public int BkColor;
		/// <summary>
		/// 字体设置
		/// </summary>
		public User_FontSet FontInfo;
		/// <summary>
		/// 标题
		/// </summary>
		public string chTitle;
		/// <summary>
		/// 显示格式：0－度 1－C
		/// </summary>
		public int DisplayType;
	}

	/// <summary>
	/// 图文区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_Bmp
	{
		/// <summary>
		/// 分区信息
		/// </summary>
		public User_PartInfo PartInfo;
	}

	/// <summary>
	/// RTF文件区参数
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct User_RTF
	{
		/// <summary>
		/// RTF文件名
		/// </summary>
		public string strFileName;
		/// <summary>
		/// 分区信息
		/// </summary>
		public User_PartInfo PartInfo;
		/// <summary>
		/// 动作方式设置
		/// </summary>
		public User_MoveSet MoveSet;
	}
}
