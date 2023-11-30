using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.ViewModel
{
    public class DeleteTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {
        public DeleteTaskCmd DataModel { get; set; }

        public DeleteTaskCmdViewModel(DeleteTaskCmd dataModel)
        {
            DataModel = dataModel;
        }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
