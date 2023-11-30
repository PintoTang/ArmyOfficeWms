using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public interface ITaskDevice<TTask> where TTask : IDeviceTaskContent
    {
        OperateResult<TTask> FinishTask(string taskCode);
    }
}
