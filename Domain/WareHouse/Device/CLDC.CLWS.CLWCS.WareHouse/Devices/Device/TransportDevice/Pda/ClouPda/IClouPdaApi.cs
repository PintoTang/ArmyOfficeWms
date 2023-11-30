using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Pda.CmdModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Pda.ClouPda
{
   public interface IClouPdaApi
   {
       /// <summary>
       /// Pda通知指令完成
       /// </summary>
       /// <param name="cmd"></param>
       /// <returns></returns>
       SyncResReErr NotifyInstructFinish(string cmd);
   }
}
