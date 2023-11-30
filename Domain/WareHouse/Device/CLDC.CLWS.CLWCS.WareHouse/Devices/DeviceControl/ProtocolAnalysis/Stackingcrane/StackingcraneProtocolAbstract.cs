using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.stackingcrane.OpcStackingcrane;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Stackingcrane
{
    public abstract class StackingcraneProtocolAbstract : ProtocolAnalysisAbstract
    {
        public abstract OperateResult<StackingcraneCmdElement> TransportChangeToCmdElement(TransportMessage transportMsg);

        public abstract TaskExcuteStatusType FaultCodeChangeToExceptionType(int faultCode);
    }
}
