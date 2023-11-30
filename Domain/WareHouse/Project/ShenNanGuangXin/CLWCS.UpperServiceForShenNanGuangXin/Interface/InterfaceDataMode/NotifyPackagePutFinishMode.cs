using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyPackagePutFinishMode
    {
        private NotifyPackagePutFinishDataMode _data = new NotifyPackagePutFinishDataMode();

        public NotifyPackagePutFinishDataMode DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class NotifyPackagePutFinishDataMode
    {
        public int PackageId { get; set; }
        public string Addr { get; set; }
        public int PosIndex { get; set; }

    }
}
