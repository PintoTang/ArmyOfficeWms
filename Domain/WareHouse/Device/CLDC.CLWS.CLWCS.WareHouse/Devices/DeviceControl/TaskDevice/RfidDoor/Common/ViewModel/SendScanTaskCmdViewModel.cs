using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.ViewModel
{
    public class SendScanTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {
       public SendScanTaskCmd DataModel { get; set; }
       public SendScanTaskCmdViewModel(SendScanTaskCmd dataModel)
       {
           DataModel = dataModel;
       }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
