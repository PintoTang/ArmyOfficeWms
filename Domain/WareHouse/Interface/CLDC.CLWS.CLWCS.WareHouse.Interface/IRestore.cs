using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
    /// <summary>
    /// 断点恢复
    /// </summary>
    public interface IRestore
    {
        /// <summary>
        /// 恢复设备状态和数据
        /// </summary>
        /// <returns></returns>
        OperateResult Restore();
        /// <summary>
        /// 处理恢复设备的状态和数据
        /// </summary>
        /// <returns></returns>
        OperateResult HandleRestoreData();
    }
}
