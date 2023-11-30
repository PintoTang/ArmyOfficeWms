using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;


namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.ViewModel
{
    public class SelectTaskInfoCmdViewModel
    {
        public SelectTaskInfoCmd DataModel { get; set; }

        public SelectTaskInfoCmdViewModel(SelectTaskInfoCmd dataModel)
        {
            DataModel = dataModel;
        }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
