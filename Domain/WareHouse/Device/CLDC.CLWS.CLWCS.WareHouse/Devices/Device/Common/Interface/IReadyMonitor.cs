using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Interface
{
    /// <summary>
    /// 具有监听就绪的功能
    /// </summary>
    public interface IReadyMonitor
    {
        /// <summary>
        /// 注册就绪值变换
        /// </summary>
        /// <param name="dbNameEnum"></param>
        /// <param name="valueChangeCallBack"></param>
        /// <returns></returns>
        OperateResult RegisterReadyValueChange(DataBlockNameEnum dbNameEnum, CallbackContainOpcValue valueChangeCallBack);
        /// <summary>
        /// 就绪值变化的事件
        /// </summary>
        ReadyValueChangeDelegate ReadyValueChangeEvent { get; set; }
        /// <summary>
        /// 是否需要处理就绪值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsNeedHandleReadyValue(int value);

    }

    public delegate void ReadyValueChangeDelegate(DeviceName deviceName, int value);

}
