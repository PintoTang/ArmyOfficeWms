using CL.WCS.DataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc
{
    public interface IPlcItemCalculate
    {
        string CaculatePlcAddress(int opcId, DataBlockNameEnum datablockEnum);
    }
}
