using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyOutFinishCmdMode
    {
        private NotifyOutFinishParaMode _data = new NotifyOutFinishParaMode();

        public NotifyOutFinishParaMode DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class NotifyOutFinishParaMode
    {
        public string Addr { get; set; }
        public string PalletBarcode { get; set; }
        public int WEIGHT { get; set; }
    }
}
