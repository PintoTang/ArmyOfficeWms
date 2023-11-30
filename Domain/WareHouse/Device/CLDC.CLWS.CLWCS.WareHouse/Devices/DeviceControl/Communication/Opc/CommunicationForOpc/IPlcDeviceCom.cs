using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc
{
    /// <summary>
    ///  设备通讯接口
    /// </summary>
    public interface IPlcDeviceCom
    {
        /// <summary>
        /// OPC协议操作元素
        /// </summary>
        OpcElement OpcElement {get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        int DeviceId { get; set; }
        DeviceName DeviceName { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        OperateResult Initialize(int deviceId, DeviceName deviceName);

        OperateResult Write(DataBlockNameEnum datablockNameEnum, object value);
        OperateResult Write(DataBlockNameEnum datablockNameEnum, string value);

        OperateResult Write(DataBlockNameEnum datablockNameEnum, float value);
        OperateResult Write(DataBlockNameEnum datablockNameEnum, bool value);
        OperateResult Write(DataBlockNameEnum datablockNameEnum, int value);

        OperateResult Write(Dictionary<DataBlockNameEnum, int> dictionary);

        OperateResult Write(Dictionary<DataBlockNameEnum, object> dictionary);

        OperateResult<bool> ReadBool(DataBlockNameEnum datablockNameEnum);
        OperateResult<string> ReadString(DataBlockNameEnum datablockNameEnum);

        OperateResult<float> ReadFloat(DataBlockNameEnum datablockNameEnum);

        OperateResult<int> ReadInt(DataBlockNameEnum datablockNameEnum);

        OperateResult RegisterValueChange(DataBlockNameEnum datablockNameEnum,
            CallbackContainOpcValue monitervaluechange);

        OperateResult RegisterNotEqualStartValue(DataBlockNameEnum datablockNameEnum, int startValue, CallbackContainOpcValue monitervaluechange);
        OperateResult RegisterValueChange(DataBlockNameEnum datablockNameEnum, CallbackContainOpcBoolValue monitervaluechange);

        OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum datablockNameEnum, int startValue, int endValue, MonitorSpecifiedOpcValueCallback callback);

        /// <summary>
        /// 更新所有的配置数据值
        /// </summary>
        void RefreshAllData();

        List<bool> ReadBoolByBlockEnums(int opcId, List<DataBlockNameEnum> datablockEnums);
    }
}
