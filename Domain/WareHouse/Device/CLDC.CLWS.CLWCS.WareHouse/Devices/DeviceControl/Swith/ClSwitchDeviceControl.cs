using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Swith.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClSwitchDeviceControl : SwitchDeviceControlAbstract
    {


        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetPropertyView()
        {
            return null;
        }
    }
}
