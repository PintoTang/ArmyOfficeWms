using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService.DeviceConfig.PalletizerDevices
{
    public class PalletizerDevices : StateAbstract
    {
        public override void handle(Context context)
        {
            DeviceHelper.Handle(context, "/PalletizerDevices");
        }
    }
}
