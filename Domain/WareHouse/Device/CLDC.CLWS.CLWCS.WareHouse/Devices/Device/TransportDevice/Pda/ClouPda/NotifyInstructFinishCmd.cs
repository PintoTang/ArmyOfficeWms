using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Pda.ClouPda
{
   public class NotifyInstructFinishCmd
    {
       public int INSTRUCTION_CODE { get; set; }

       public string CURRENT_ADDR { get; set; }

       public static explicit operator NotifyInstructFinishCmd(string json)
       {
           return JsonConvert.DeserializeObject<NotifyInstructFinishCmd>(json);
       }
   }
}
