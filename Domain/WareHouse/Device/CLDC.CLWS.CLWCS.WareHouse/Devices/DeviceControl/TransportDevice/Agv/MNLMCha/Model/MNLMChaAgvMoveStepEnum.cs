using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.MNLMCha.Model
{
    public enum MNLMChaAgvMoveStepEnum
    {

        [Description("任务开始")]
        Executing = 1,

        [Description("取货完成（取货）")]
        PickFinish = 2,

        [Description("带货行走")]
        MoveWithLoaded = 3,

        [Description("卸货完成（卸货）")]
        PutFinish = 4,

        [Description("任务完成")]
        Finish = 5,

        [Description("任务执行异常")]
        Exception = 6
    }
}
