using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Interface
{
    /// <summary>
    /// 具有码/拆跺功能
    /// </summary>
    public interface IPalletize
    {
        /// <summary>
        /// 码垛
        /// </summary>
        /// <returns></returns>
        OperateResult Palletize(string cmd);
        /// <summary>
        /// 拆垛
        /// </summary>
        /// <returns></returns>
        OperateResult DePalletize(string cmd);
        /// <summary>
        /// 拆码盘机任务完成事件
        /// </summary>
        /// <returns></returns>
        PalletizerTaskFinish PalletizerFinishEvent { get; set; }
        
    }

    public delegate OperateResult PalletizerTaskFinish(DeviceBaseAbstract device, StringCharTask task);

}
