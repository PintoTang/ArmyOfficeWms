using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class CmdNotifyBarcodeModel
    {
        /// <summary>
        /// 读到条码的设备所在的地址
        /// </summary>
        [JsonProperty("ADDR")]
        public string Addr
        {
            get; set;
        }

        /// <summary>
        /// 条码号
        /// </summary>
        [JsonProperty("BARCODE")]
        public string Barcode
        {
            get; set;
        }
    }
}
