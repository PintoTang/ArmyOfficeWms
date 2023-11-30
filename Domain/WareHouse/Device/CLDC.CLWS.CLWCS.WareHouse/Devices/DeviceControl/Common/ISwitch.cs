using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common
{
    public interface ISwitch
    {
        /// <summary>
        /// 按钮切换
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult Switch(string cmd);
        /// <summary>
        /// 通知按钮切换
        /// </summary>
        NotifySwitchChange SwitchChangeEvent { get; set; }
    }

    public delegate OperateResult NotifySwitchChange(DeviceBaseAbstract device, int value,string addr);
}
