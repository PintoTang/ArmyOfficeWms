using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyPalletizerFinishCmdMode
    {
        private PalletizerFinishParaMode _data=new PalletizerFinishParaMode();

        public PalletizerFinishParaMode DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class PalletizerFinishParaMode
    {
        public string ADDR { get; set; }
        public string PALLETBARCODE { get; set; }
        public int Weight { get; set; }
    }
}
