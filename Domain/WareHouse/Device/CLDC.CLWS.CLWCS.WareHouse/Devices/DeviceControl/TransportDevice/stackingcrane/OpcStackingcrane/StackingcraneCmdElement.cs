namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.stackingcrane.OpcStackingcrane
{
    /// <summary>
    /// 堆垛机的指令因素
    /// </summary>
    public class StackingcraneCmdElement
    {
        public int CmdNum { get; set; }
        public CmdTypeMode CmdType { get; set; }
        /// <summary>
        ///源站台   出库时为0，入库时为货物所在站台（输送机入库站台号）
        /// </summary>
        public int SourceStationNum { get; set; }
        /// <summary>
        /// 源站台排 入库使用
        /// </summary>
        public int SourceStationRow { get; set; }
        /// <summary>
        /// 源站台层 入库使用
        /// </summary>
        public int SourceStationLayer { get; set; }
        /// <summary>
        /// 源站台列 入库使用
        /// </summary>
        public int SourceStationColumn { get; set; }
        /// <summary>
        /// 源仓位排 出库 移库
        /// </summary>
        public int SourceCellRow { get; set; }
        /// <summary>
        /// 源仓位层 出库 移库
        /// </summary>
        public int SourceCellLayer { get; set; }
        /// <summary>
        /// 源仓位列 出库 移库
        /// </summary>
        public int SourceCellColumn { get; set; }
        /// <summary>
        /// 目的站台 入库时为0，放货时所对应的站台（输送机出库站台号）
        /// </summary>
        public int DestStationNum { get; set; }
        /// <summary>
        /// 目标站台排 出库使用
        /// </summary>
        public int DestStationRow { get; set; }
        /// <summary>
        /// 目标站体层 出库使用
        /// </summary>
        public int DestStationLayer { get; set; }
        /// <summary>
        /// 目标列 出库使用
        /// </summary>
        public int DestStationColumn { get; set; }
        /// <summary>
        /// 目标仓位排 入库使用
        /// </summary>
        public int DestCellRow { get; set; }
        /// <summary>
        /// 目标仓位层 入库使用
        /// </summary>
        public int DestCellLayer { get; set; }
        /// <summary>
        /// 目标仓位列 入库使用
        /// </summary>
        public int DestCellColumn { get; set; }
        /// <summary>
        /// 检验码
        /// </summary>
        public int VerificationCode { get; set; }
        ///// <summary>
        ///// 盘点列
        ///// </summary>
        //public int PickCellColumn { get; set; }
        ///// <summary>
        ///// 盘点层
        ///// </summary>
        //public int PickPickCellLayer { get; set; }
        /// <summary>
        /// 命令标识
        /// </summary>
        public int SendCmdCode { get; set; }

        public int TrayType { get; set; }
    }

    public enum CmdTypeMode
    {
        /// <summary>
        /// 无工作命令
        /// </summary>
        None = 0,
        /// <summary>
        /// 入库作业
        /// </summary>
        In = 1,
        /// <summary>
        /// 出库作业
        /// </summary>
        Out = 2,
        /// <summary>
        /// 直接出库，站台到站台
        /// </summary>
        StationToStation = 3,
        /// <summary>
        /// 移库
        /// </summary>
        Move = 4,
        /// <summary>
        /// 更改仓位
        /// </summary>
        Change = 5,
        /// <summary>
        /// 盘点作业
        /// </summary>
        Pick = 6
    }

}
