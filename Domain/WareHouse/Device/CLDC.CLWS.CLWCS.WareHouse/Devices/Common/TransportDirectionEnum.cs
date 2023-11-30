using System.Windows;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common
{
     [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public enum TransportDirectionEnum
    {
        /// <summary>
        /// 往前
        /// </summary>
        Forward=0,
        /// <summary>
        /// 往后
        /// </summary>
        Backward=1,
        /// <summary>
        /// 双向
        /// </summary>
        Both=2,
         /// <summary>
         /// 无方向
         /// </summary>
         None
    }
}
