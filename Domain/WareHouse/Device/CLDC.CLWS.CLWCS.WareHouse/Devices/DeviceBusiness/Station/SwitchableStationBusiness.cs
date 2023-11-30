using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Station.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 科陆控制的可入可出站台业务逻辑
    /// </summary>
    public class SwitchableStationBusiness : StationDeviceBusinessAbstract
    {
        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeMode(DeviceModeEnum destMode)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ChangeMode(DeviceModeEnum destMode)
        {
            CurMode = destMode;
            return OperateResult.CreateSuccessResult();
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
            if (newValue.Equals(CurOrderValue)||newValue<=0)
            {
                CurOrderValue = newValue;
                return false;
            }
            else
            {
                CurOrderValue = newValue;
                return true;
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
