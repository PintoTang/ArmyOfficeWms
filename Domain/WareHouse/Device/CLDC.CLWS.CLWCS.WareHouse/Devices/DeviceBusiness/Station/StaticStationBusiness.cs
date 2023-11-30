using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Station.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 静态站台的业务处理，如入库口 出库口
    /// </summary>
    public class StaticStationBusiness : StationDeviceBusinessAbstract
    {

        public override OperateResult IsCanChangeMode(DeviceModeEnum destMode)
        {
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult ChangeMode(DeviceModeEnum destMode)
        {
            throw new NotImplementedException();
        }

        public override bool CheckMode(DeviceModeEnum destMode)
        {
            if (CurMode.Equals(destMode))
            {
                return true;
            }
            else
            {
                return false;

            }
        }
        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool IsNeedHandleOrderValue(int newValue)
        {
            CurOrderValue = newValue;
            if (newValue.Equals(-1000))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool IsNeedHandleDestValue(int newValue)
        {
            if (newValue.Equals(CurDestValue))
            {
                return false;
            }
            else
            {
                CurDestValue = newValue;
                return true;
            }
        }
    }
}
