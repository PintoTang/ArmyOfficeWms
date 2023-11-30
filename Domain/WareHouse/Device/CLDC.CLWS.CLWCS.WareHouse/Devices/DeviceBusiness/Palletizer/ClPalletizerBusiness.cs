using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClPalletizerBusiness : PalletizerBusinessAbstract
    {
        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }
        internal override bool IsNeedHandleNeedSignValue(bool newValue)
        {
            if (newValue.Equals(true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
