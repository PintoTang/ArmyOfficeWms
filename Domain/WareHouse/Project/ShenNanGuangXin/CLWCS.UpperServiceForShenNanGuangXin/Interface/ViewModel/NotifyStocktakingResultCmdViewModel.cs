using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;

namespace CLWCS.UpperServiceForHeFei.Interface.ViewModel
{
    public class NotifyStocktakingResultCmdViewModel : IInvokeCmd
    {
        public NotifyStocktakingResultCmdMode DataModel { get; set; }
        public NotifyStocktakingResultCmdViewModel(NotifyStocktakingResultCmdMode dataModel)
        {
            DataModel = dataModel;
            BarcodeList = new List<string>();
            InitBarcodeList();
        }
        public string GetCmd()
        {
            DataModel.DATA.PALLETBARCODE="";
            foreach (string barcodeMode in BarcodeList)
            {
                DataModel.DATA.PALLETBARCODE=barcodeMode;
            }
            return DataModel.ToString();
        }
        private void InitBarcodeList()
        {
            //foreach (string barcode in DataModel.DATA.BARCODES)
            //{
                BarcodeList.Add(DataModel.DATA.PALLETBARCODE);
            //}
        }

        public List<string> BarcodeList { get; set; }
    }
}
