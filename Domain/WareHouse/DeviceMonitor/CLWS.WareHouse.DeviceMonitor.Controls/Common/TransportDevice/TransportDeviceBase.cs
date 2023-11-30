using System.Windows;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common;

namespace WHSE.Monitor.Framework.UserControls
{
    public class TransportDeviceBase : DeviceSimulation
    {


        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", typeof(TransportDirectionEnum), typeof(TransportDeviceBase), new PropertyMetadata(default(TransportDirectionEnum)));

        /// <summary>
        /// 搬运设备的方向属性
        /// </summary>
        public TransportDirectionEnum Direction
        {
            get { return (TransportDirectionEnum)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

    }
}
