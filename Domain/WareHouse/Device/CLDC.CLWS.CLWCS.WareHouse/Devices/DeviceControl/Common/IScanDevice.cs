using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common
{
    public interface IScanDevice
    {
        /// <summary>
        /// 扫描物资
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult Scan(string cmd);
    }
}
