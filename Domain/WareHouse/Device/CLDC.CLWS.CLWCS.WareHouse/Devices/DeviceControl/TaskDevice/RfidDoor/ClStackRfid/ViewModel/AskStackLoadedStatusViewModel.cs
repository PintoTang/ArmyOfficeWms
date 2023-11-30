using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.ViewModel
{
    public class AskStackLoadedStatusViewModel : ViewModelBase, IInvokeCmd
    {
        public AskStackLoadedStatusCmd DataModel { get; set; }
        public AskStackLoadedStatusViewModel(AskStackLoadedStatusCmd dataModel)
       {
           DataModel = dataModel;
       }
        public string GetCmd()
        {
            return DataModel.ToJson();
        }


    }
}
