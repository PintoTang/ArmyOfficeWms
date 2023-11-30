using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.TransportDevice.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class HangChaAgvBusiness:OrderDeviceBusinessAbstract
    {
        public override bool Accessible(Addr startAddr, Addr destAddr)
        {
            bool isStartEnable = StartAddressLst.Select(addr => addr.IsContain(startAddr)).Any(isContain => isContain);
            bool isDestEnable = DestAddressLst.Select(addr => addr.IsContain(destAddr)).Any(isContain => isContain);
            if (isStartEnable && isDestEnable)
            {
                return true;
            }
            return false;
        }

        public override OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult DoJob(TransportMessage transMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool IsNeedHanldeOrderValue(int value)
        {
            throw new NotImplementedException();
        }

        public override OperateResult ParticularInitlize()
        {
            return  OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
