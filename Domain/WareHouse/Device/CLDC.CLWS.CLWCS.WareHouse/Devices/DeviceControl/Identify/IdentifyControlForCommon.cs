using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Identify;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class IdentifyControlForCommon : IdentifyDeviceControlAbstract<List<string>>
    {

        public override OperateResult<List<string>> GetIdentifyMessage(params object[] para)
        {
            return communicate.SendCommand(para);

        }
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


        public override void GetIdentityMessageAsync(params object[] para)
        {
            communicate.SendCommandAsync(para);
        }
    }
}