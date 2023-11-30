using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common
{
   public interface IDoorDevice
   {
       /// <summary>
       /// 关门
       /// </summary>
       /// <param name="cmd"></param>
       /// <returns></returns>
       OperateResult CloseDoor(string cmd);
       /// <summary>
       /// 开门
       /// </summary>
       /// <param name="cmd"></param>
       /// <returns></returns>
       OperateResult OpenDoor(string cmd);
   }
}
