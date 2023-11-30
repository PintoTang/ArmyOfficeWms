using System.ComponentModel;
using System.Windows;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    /// <summary>
    /// 设备类型
    /// </summary>
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public enum DeviceTypeEnum
    {
        /// <summary>
        /// 承载设备
        /// </summary>
        [Description("承载设备")]
        LoadDevice = 0,
        /// <summary>
        /// 识别设备
        /// </summary>
        [Description("识别设备")]
        IdentityDevice,
        /// <summary>
        /// 显示设备
        /// </summary>
        [Description("显示设备")]
        DisplayDevice,
        /// <summary>
        /// 搬运设备
        /// </summary>
        [Description("搬运设备")]
        TransportDevice,
        /// <summary>
        /// 货架设备
        /// </summary>
        [Description("货架设备")]
        RackPlace,
        /// <summary>
        /// 按钮设备
        /// </summary>
        [Description("开关设备")]
        SwitchDevice,
        /// <summary>
        /// 拆码盘机
        /// </summary>
        [Description("拆码盘机")]
        PalletizerDevice
    }
}
