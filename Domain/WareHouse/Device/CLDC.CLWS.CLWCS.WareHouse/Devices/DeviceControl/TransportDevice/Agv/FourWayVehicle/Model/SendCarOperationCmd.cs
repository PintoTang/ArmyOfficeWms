namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    public class SendCarOperationCmd
    {
        /// <summary>
        /// 小车编号
        /// </summary>
        public string carNo { get; set; }
        /// <summary>
        /// 操作指令 0为继续，1为暂停
        /// </summary>
        public string operationFlag { get; set; }
    }
}
