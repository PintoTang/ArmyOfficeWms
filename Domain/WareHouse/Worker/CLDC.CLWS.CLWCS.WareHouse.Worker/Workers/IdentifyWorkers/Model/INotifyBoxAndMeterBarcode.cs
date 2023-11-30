using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model
{
    public interface INotifyBoxAndMeterBarcode
    {
        bool NotifyBoxAndMeterBarcode(DeviceBaseAbstract device, string boxBarcode, List<string> meterBarcode, string lastBoxBarcode);
    }
}
