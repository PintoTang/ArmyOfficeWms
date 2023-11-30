using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common
{
    public interface IStackDevice
    {
        OperateResult Stack(string taskNo,string stackCode, int pileNo,int stackNo,int boxType);
        
        OperateResult UnStack(string taskNo, string stackCode, int pileNo, int unStackNo, int boxType);
    }
}
