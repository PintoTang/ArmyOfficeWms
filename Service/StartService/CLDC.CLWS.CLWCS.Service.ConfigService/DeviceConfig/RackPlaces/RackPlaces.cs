using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService.DeviceConfig.RackPlaces
{
    public class RackPlaces : StateAbstract
    {
        public override void handle(Context context)
        {
            DeviceHelper.Handle(context, "/RackPlaces");
        }
    }
}
