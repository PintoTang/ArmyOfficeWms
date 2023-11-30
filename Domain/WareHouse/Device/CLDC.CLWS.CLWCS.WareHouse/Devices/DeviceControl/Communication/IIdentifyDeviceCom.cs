using System;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication
{
    /// <summary>
    /// 条码枪控制接口
    /// </summary>
    public interface IIdentifyDeviceCom<T>
    {
        /// <summary>
        /// 上报信息
        /// </summary>
        MessageReportDelegate MessageReportEvent { get; set; }

        /// <summary>
        /// 上报信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageLevel"></param>
        void MessageReport(string message,EnumLogLevel messageLevel); 

        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 设备名称
        /// </summary>
        DeviceName DeviceName { get; set; }
        int DeviceId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        OperateResult Initialize(int deviceId, DeviceName deviceName);
        /// <summary>
        /// 异步读取条码
        /// </summary>
        /// <param name="para"></param>
        void SendCommandAsync(params object[] para);

        /// <summary>
        /// 同步读取条码
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        OperateResult<T> SendCommand(params object[] para);

        /// <summary>
        /// 接收到条码的事件处理
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="barcodeList"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        event BarcodeCallback<T> OnReceiveBarcode;
    }
    public delegate void BarcodeCallback<T>(DeviceName deviceName, T barcode, params object[] para);
}
