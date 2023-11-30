using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyStocktakingResultCmdMode
    {
        private NotifyStocktakingDataMode _data=new NotifyStocktakingDataMode();

        public NotifyStocktakingDataMode DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class NotifyStocktakingDataMode
    {
        public string DST_ADDR { get; set; }
        public string PALLETBARCODE { get; set; }
        public int WEIGHT { get; set; }

       
    }
}
