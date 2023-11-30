using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using System.Threading;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Identify;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class IdentifyDeviceBusiness : IdentifyDeviceBusinessAbstract<List<string>>
    {
        public  override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public  override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public  override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        internal override OperateResult AfterNotifyIdetifyMsg(List<string> identifyMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        internal override OperateResult<List<string>> BeforeNotifyIdetifyMsg(List<string> identifyMsg)
        {
            OperateResult<List<string>> result = OperateResult.CreateSuccessResult(identifyMsg);
            return result;
        }

        internal override OperateResult<List<string>> IdentifyMsgFilter(List<string> identifyMsg)
        {
            OperateResult<List<string>> result = OperateResult.CreateSuccessResult(identifyMsg);
            return result;
        }
    }
}
