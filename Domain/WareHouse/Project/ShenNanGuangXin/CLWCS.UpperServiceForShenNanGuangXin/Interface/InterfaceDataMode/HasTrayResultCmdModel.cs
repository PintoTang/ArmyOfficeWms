using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class HasTrayResultCmdModel
    {
        public HasTrayResultCmdModel()
        {
            this.PointTrayData = new List<PointTrayDataModel>();
        }
        public List<PointTrayDataModel> PointTrayData { get; set; }
    }

    public class PointTrayDataModel
    {
        public string Addr { get; set; }

        public string PalletBarcode { get; set; }

        public bool HasTray { get; set; }
    }
}
