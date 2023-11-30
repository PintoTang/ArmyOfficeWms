using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Pda.CmdModel
{
    public class SendInstructCmd
    {
        public int INSTRUCTION_CODE { get; set; }
        public int PRI { get; set; }
        public string SRC_ADDR { get; set; }
        public string DST_ADDR { get; set; }
        public string BARCODE { get; set; }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
