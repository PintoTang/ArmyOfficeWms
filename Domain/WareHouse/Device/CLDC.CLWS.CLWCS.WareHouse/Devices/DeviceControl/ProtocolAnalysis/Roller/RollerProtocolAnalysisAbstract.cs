using System.Collections.Generic;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Roller
{
    public abstract class RollerProtocolAnalysisAbstract : ProtocolAnalysisAbstract
    {
        /// <summary>
        /// 通过搬运信息转换成通讯信息
        /// </summary>
        /// <param name="transportMsg"></param>
        /// <returns></returns>
        public abstract OperateResult<Dictionary<DataBlockNameEnum, object>> ComposeCmd(TransportMessage transportMsg);
    }
}
