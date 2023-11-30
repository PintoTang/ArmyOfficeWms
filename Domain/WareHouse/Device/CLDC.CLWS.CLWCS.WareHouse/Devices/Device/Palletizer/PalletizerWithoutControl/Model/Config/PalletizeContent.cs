using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config
{
    public class PalletizeContent : IUnique
    {
        public PalletizeContent(int index,string barcode)
        {
            ContentIndex = index;
            ContentBarcode = barcode;
            AddTime = DateTime.Now;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string UniqueCode
        {
            get
            { return ContentIndex.ToString(); }
            set { ContentIndex = int.Parse(value); }
        }

        public string ContentBarcode { get; set; }

        public int ContentIndex { get; set; }

        public DateTime AddTime { get; set; }

    }
}
