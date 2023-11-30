using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class LoadBusiness : DeviceBusinessBaseAbstract
    {
        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public  override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
