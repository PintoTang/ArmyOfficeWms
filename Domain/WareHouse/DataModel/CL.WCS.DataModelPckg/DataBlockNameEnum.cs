using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 地址块名称枚举
    /// </summary>
    public enum DataBlockNameEnum
    {
        /// <summary>
        /// 叠盘机读箱数量的地址块
        /// </summary>
        PalletizerBoxCountDataBlock,

        /// <summary>
        /// 满垛数量
        /// </summary>
        FullAmount,

        /// <summary>
        /// 缓冲区序列号
        /// </summary>
        CacheOPCSerialNo,

        /// <summary>
        /// 是子母托盘
        /// </summary>
        IsContainSubTrayDataBlock,

        /// <summary>
        /// 货高类型
        /// </summary>
        GoodHeightType,

        /// <summary>
        /// 货物异常
        /// </summary>
        GoodException,

        /// <summary>
        /// OPC读订单的地址块
        /// </summary>
        OPCOrderIdDataBlock,

        /// <summary>
        /// WMS指令 ID（用于3D仿真）
        /// </summary>
        WMSOPCOrderIdDataBlock,

        /// <summary>
        /// OPC读条码的地址块
        /// </summary>
        OPCBarcodeDataBlock,

        /// <summary>
        /// OPC读取重量的地址块
        /// </summary>
        OPCWeightDataBlock,

        /// <summary>
        /// 变更站台出入模式
        /// </summary>
        WriteInOutModeDataBlock,

        /// <summary>
        /// 模式切换允许信号地址
        /// </summary>
        AllowModeSwitchDataBlock,

        /// <summary>
        /// 写orderId的地址块
        /// </summary>
        WriteOrderIDDataBlock,

        /// <summary>
        /// 写目的地址的地址块
        /// </summary>
        DestinationDataBlock,

        /// <summary>
        /// 验证失败放行的下一步地址编号（异常口）
        /// </summary>
        ErrorDestinationValue,

        /// <summary>
        /// 验证成功放行的下一步地址编号
        /// </summary>
        SuccessDestinationValue,

        /// <summary>
        /// 叠盘机序列号
        /// </summary>
        PalletizerSerialNo,

        /// <summary>
        /// 缓存区尾端序列号
        /// </summary>
        CacheTailSerialNo,

        /// <summary>
        /// 叠盘机写设备类型地址块
        /// </summary>
        WriteDeviceTypeDataBlock,

        /// <summary>
        /// OPC 序列号
        /// </summary>
        OPCSerialNo,

        /// <summary>
        /// 叠盘机读托盘号（哈尔滨托盘库）
        /// </summary>
        TrayBarcodeDataBlock,

        /// <summary>
        /// 写方向号的地址块
        /// </summary>
        WriteDirectionDataBlock,

        /// <summary>
        /// 是否可放行的地址块
        /// </summary>
        IsReleaseDataBlock,

        /// <summary>
        /// 写层号的地址块
        /// </summary>
        WriteShelfIDDataBlock,

        /// <summary>
        /// 异常回退时写的包号
        /// </summary>
        OrderIdWhenRollBack,

        ///<summary>
        ///OPC读托盘规格的地址块
        ///</summary>
        OPCTrayType,

        /// <summary>
        /// 消防联动监控PLC信号地址
        /// </summary>
        MonitorFireControlDataBlock,

        /// <summary>
        /// 消防联动通知PLC完成地址
        /// </summary>
        FinishFireControlDataBlock,

        /// <summary>
        /// 是否等待入库地址
        /// </summary>
        IsWaitInWarehouseDataBlock,

        /// <summary>
        /// 读方向号DB块
        /// </summary>
        ReadDirectionDataBlock,

        /// <summary>
        /// 机械手抓取From位置
        /// </summary>
        ManipulatorFrom,

        /// <summary>
        /// 机械手放置目标TO位置
        /// </summary>
        ManipulatorTo,

        /// <summary>
        /// 机械手抓取完成
        /// </summary>
        FinishDatablock,

        /// <summary>
        /// 设备运行状态
        /// </summary>
        DeviceStatus,

        /// <summary>
        /// 数量
        /// </summary>
        CountDataBlock,

        /// <summary>
        /// 拆盘数量(福沙)
        /// </summary>
        DePalletizeCountDataBlock,
        /// <summary>
        /// 起始地址
        /// </summary>
        StartAddressDataBlock,

        /// <summary>
        /// OPC注册的监听类型
        /// </summary>
        OPCMonitorRegisterType,

        /// <summary>
        /// OPC监听的DB块
        /// </summary>
        OPCMonitorDataBlock,

        /// <summary>
        /// OPC监听变化的起始值
        /// </summary>
        OPCMonitorStartValue,

        /// <summary>
        /// OPC监听变化的终止值
        /// </summary>
        OPCMonitorEndValue,

        /// <summary>
        /// OPC监听变化的指定值
        /// 1、 “+” ：正数
        /// 2、“-”：负数
        /// 3、”!0“：非0
        /// 4、指定值
        /// </summary>
        OPCMonitorSpecifiedValue,

#region  福沙出入口 PLC定义
        /// <summary>
        /// 允许进入
        /// </summary>
        IsAllowInDataBlock,
        /// <summary>
        /// 入完成
        /// </summary>
        IsAllowInCompleteDataBlock,

        /// <summary>
        /// 允许出
        /// </summary>
        IsAllowOutDataBlock,
        /// <summary>
        /// 出完成
        /// </summary>
        IsAllowOutCompleteDataBlock,


        /// <summary>
        /// AGV请求入
        /// </summary>
        IsRequestInDataBlock,
        /// <summary>
        /// AGV请求入完成
        /// </summary>
        IsRequestCompleteDataBlock,

        /// <summary>
        /// AGV请求出
        /// </summary>
        IsRequestOutDataBlock,
        /// <summary>
        /// AGV请求出完成
        /// </summary>
        IsRequestOutCompleteDataBlock,

        

        #endregion

        #region 堆垛机
        /// <summary>
        /// OPC堆垛机的命令代码
        /// </summary>
        CommadType,

        /// <summary>
        /// 源站台编号
        /// </summary>
        SourceStationNum,
        /// <summary>
        /// 目的站台编号
        /// </summary>
        DestStationNum,

        /// <summary>
        /// OPC堆垛机源站台排号
        /// </summary>
        SourceStationRow,

        /// <summary>
        /// OPC堆垛机源站台层号
        /// </summary>
        SourceStationLayer,

        /// <summary>
        /// 命令标识
        /// </summary>
        SendCmdCode,

        /// <summary>
        /// OPC堆垛机源站台列号
        /// </summary>
        SourceStationColumn,

        /// <summary>
        /// OPC堆垛机源仓位排号
        /// </summary>
        SourceCellRow,
        /// <summary>
        /// OPC堆垛机源仓位层号
        /// </summary>
        SourceCellLayer,
        /// <summary>
        /// OPC堆垛机源仓位列号
        /// </summary>
        SourceCellColumn,

        /// <summary>
        /// OPC堆垛机源站台排号
        /// </summary>
        DestStationRow,

        /// <summary>
        /// OPC堆垛机源站台层号
        /// </summary>
        DestStationLayer,

        /// <summary>
        /// OPC堆垛机源站台列号
        /// </summary>
        DestStationColumn,

        /// <summary>
        /// OPC堆垛机源仓位排号
        /// </summary>
        DestCellRow,
        /// <summary>
        /// OPC堆垛机源仓位层号
        /// </summary>
        DestCellLayer,
        /// <summary>
        /// OPC堆垛机源仓位列号
        /// </summary>
        DestCellColumn,

        /// <summary>
        /// OPC堆垛机校验码
        /// </summary>
        VerificationCode,
        /// <summary>
        /// OPC堆垛机校验结果
        /// </summary>
        VerificationResult,

        /// <summary>
        /// OPC堆垛机作业完成标识
        /// </summary>
        OrderFinishFlag,

        /// <summary>
        /// OPC堆垛机正在处理的任务编号
        /// </summary>
        InProgressOrder,

        /// <summary>
        /// PLC序列号
        /// </summary>
        OpcSerialNumber,

        /// <summary>
        /// 设备状态
        /// </summary>
        DeviceWorkStatus,

        /// <summary>
        /// 设备行程状态，接获
        /// </summary>
        DeviceActionPick,

        /// <summary>
        /// 设备行程状态，行走
        /// </summary>
        DeviceActionGoing,
        /// <summary>
        /// 设备行程状态，到位
        /// </summary>
        DeviceActionReady,
        /// <summary>
        /// 设备行程状态，放货
        /// </summary>
        DeviceActionPut,

        /// <summary>
        /// 设备当前排号
        /// </summary>
        DeviceCurRow,

        /// <summary>
        /// 设备当前层号
        /// </summary>
        DeviceCurLayer,

        /// <summary>
        /// 设备当前列号
        /// </summary>
        DeviceCurColumn,

        /// <summary>
        /// 设备连接状态
        /// </summary>
        DeviceConnectStatus,

        /// <summary>
        /// 设备故障代码
        /// </summary>
        DeviceFaultCode,

        /// <summary>
        /// 设备已执行完成的任务号
        /// </summary>
        DeviceFinishedOrder,

        /// <summary>
        /// 分拣就绪地址
        /// </summary>
        PickingReadyDataBlock,

        /// <summary>
        /// 任务类型
        /// </summary>
        OrderTypeDataBlock,

        /// <summary>
        /// 外观状态
        /// </summary>
        SizeStatusDataBlock,

        /// <summary>
        /// 高度状态
        /// </summary>
        HeightStatusDataBlock,

        /// <summary>
        /// 前宽度
        /// </summary>
        FrontWidthStatusDataBlock,

        /// <summary>
        /// 后宽度
        /// </summary>
        BehindWidthStatusDataBlock,

        /// <summary>
        /// 左超长
        /// </summary>
        LeftWidthStatusDataBlock,
        /// <summary>
        /// 右超长
        /// </summary>
        RightWidthStatusDataBlock,

        /// <summary>
        /// 重量状态
        /// </summary>
        WeightStatusDataBlock,

        #endregion

        #region 设备状态
        /// <summary>
        /// 移动总故障
        /// </summary>
        [Description("移动总故障")]
        IsTranslationAllFault,

        /// <summary>
        /// 负载
        /// </summary>
        [Description("负载")]
        IsLoaded,
        /// <summary>
        /// 正转
        /// </summary>
        [Description("正转")]
        IsTranslationFw,
        /// <summary>
        /// 反转
        /// </summary>
        [Description("反转")]
        IsTranslationRv,
        #endregion
        #region  佛山顺德立库 PLC定义
        /// <summary>
        /// 1005请求入
        /// </summary>
        [Description("1005请求入")]
        PLC1005RequesIn,
        /// <summary>
        /// RGV允许1005入
        /// </summary>
        [Description("RGV允许1005入")]
        RGV_AllowIn1005,
        /// <summary>
        /// RGV允许1005入完成
        /// </summary>
        [Description("RGV允许1005入完成")]
        RGV_InDone1005,
        /// <summary>
        /// 1005收到入库完成
        /// </summary>
        [Description("1005收到入库完成")]
        PLC1005ReceivedInDone,

        /// <summary>
        /// RGV请求1005出库
        /// </summary>
        [Description("RGV请求1005出库")]
        RGV_RequesOut1005,
        /// <summary>
        /// 1005允许出
        /// </summary>
        [Description("1005允许出")]
        PLC1005AllowOut,
        /// <summary>
        /// 1005出完成
        /// </summary>
        [Description("1005出完成")]
        PLC1005OutDone,
        /// <summary>
        /// RGV收到1005出库完成
        /// </summary>
        [Description("RGV收到1005出库完成")]
        RGV_ReceivedOutDone1005,

        /// <summary>
        /// 1014请求入
        /// </summary>
        [Description("1014请求入")]
        PLC1014RequesIn,
        /// <summary>
        /// RGV允许1014入
        /// </summary>
        [Description("RGV允许1014入")]
        RGV_AllowIn1014,
        /// <summary>
        /// RGV允许1014入完成
        /// </summary>
        [Description("RGV允许1014入完成")]
        RGV_InDone1014,
        /// <summary>
        /// 1014收到入库完成
        /// </summary>
        [Description("1014收到入库完成")]
        PLC1014ReceivedInDone,

        /// <summary>
        /// RGV请求1014出
        /// </summary>
        [Description("RGV请求1014出")]
        RGV_RequesOut1014,
        /// <summary>
        /// 1014允许RGV出
        /// </summary>
        [Description("1014允许RGV出")]
        PLC1014AllowOut,
        /// <summary>
        /// 1014出完成
        /// </summary>
        [Description("1014出完成")]
        PLC1014OutDone,
        /// <summary>
        /// RGV请求1014出完成
        /// </summary>
        [Description("RGV请求1014出完成")]
        RGV_ReceivedOutDone1014,
        #endregion

        #region 深南电路
        /// <summary>
        ///允许1
        /// </summary>
        [Description("允许1")]
        OpcDoorIsAllowed1,

        /// <summary>
        ///允许2
        /// </summary>
        [Description("允许2")]
        OpcDoorIsAllowed2,

        /// <summary>
        ///允许3
        /// </summary>
        [Description("允许3")]
        OpcDoorIsAllowed3,

        /// <summary>
        ///允许4
        /// </summary>
        [Description("允许4")]
        OpcDoorIsAllowed4,

        /// <summary>
        ///允许5
        /// </summary>
        [Description("允许5")]
        OpcDoorIsAllowed5,

        /// <summary>
        ///允许6
        /// </summary>
        [Description("允许6")]
        OpcDoorIsAllowed6,


        [Description("开到位1")]
        OpcDoorOpen1,
        [Description("开到位2")]
        OpcDoorOpen2,
        [Description("开到位3")]
        OpcDoorOpen3,
        [Description("开到位4")]
        OpcDoorOpen4,
        [Description("开到位5")]
        OpcDoorOpen5,
        [Description("开到位6")]
        OpcDoorOpen6,


        [Description("关到位1")]
        OpcDoorClose1,
        [Description("关到位2")]
        OpcDoorClose2,
        [Description("关到位3")]
        OpcDoorClose3,
        [Description("关到位4")]
        OpcDoorClose4,
        [Description("关到位5")]
        OpcDoorClose5,
        [Description("关到位6")]
        OpcDoorClose6,
        [Description("出库门3个位置报警提醒")]
        NotifyOrClearWarningOut,
        [Description("入库位置报警提醒")]
        NotifyOrClearWarningIn,
        [Description("出库位置异常开门报警提醒")]
        NotifyTrespassOpenDoor,
        [Description("入库有箱子要退")]
        NotifyOpcBackInException,

        [Description("入库是否自动")]
        IsPlcWareHouseInAuto,

        [Description("出库是否自动")]
        IsPlcWareHouseOutAuto,
        [Description("扩展数据 条码1|位置号1@条码2|位置号2")]
        OPCBarcodeDataBlock2,

        [Description("扩展数据 条码位置|条码|位置号")]
        OPCBarcodeDataBlock21,
        [Description("扩展数据 条码位置|条码|位置号")]
        OPCBarcodeDataBlock22,

        [Description("是否有货托盘退回1")]
        OpcTrayIsBackOut1,
        [Description("是否有货托盘退回2")]
        OpcTrayIsBackOut2,
        [Description("异常代码")]
        ErrorCode,
        /// <summary>
        /// 托盘类型
        /// </summary>
        [Description("托盘类型")]
        TrayType,

        [Description("平移驱动故障/变频器故障")]
        IsTranslationDriverFault,

        [Description("平移超时")]
        IsTranslationTimeOut,

        [Description("平移空气开关断开")]
        IsTranslationSwitchFault,

        [Description("急停")]
        IsCrashStop,

        [Description("平移传感器异常")]
        IsTranslationSensorFault,

        [Description("升降总故障")]
        IsUpDownFault,

        [Description("升降驱动故障")]
        IsUpDownDriverFault,

        [Description("升降超时")]
        IsUpDownTimeOut,

        [Description("升降空气开关断开")]
        IsUpDownSwitchFault,
        #endregion
    }
}
