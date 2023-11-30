namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.EqLedScreen
{
	public class EqLedStyle
	{
		/// <summary>
		/// 字体样式
		/// </summary>
		public User_FontSet FontStyle = new User_FontSet();
		/// <summary>
		/// wenbrn分区信息
		/// </summary>
		public User_PartInfo PartInfo = new User_PartInfo();
		/// <summary>
		/// 动作方式设置
		/// </summary>
		public User_MoveSet MoveSet = new User_MoveSet();
		/// <summary>
		/// 控制卡编号
		/// </summary>
		public int CardNum;
		/// <summary>
		/// Ip地址
		/// </summary>
		public string IpAddr;
		/// <summary>
		/// 端口
		/// </summary>
		public int ServerPort;
		/// <summary>
		/// 保存路径
		/// </summary>
		public string ReceiveSavePath;
        /// <summary>
        /// biaoti
        /// </summary>
        public User_PartInfo titlePartInfo = new User_PartInfo();
	}
}
