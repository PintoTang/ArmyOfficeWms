using CLDC.CLWS.CLWCS.Framework;
using Newtonsoft.Json;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyCrossDoorModel
    {
        public NotifyCrossDoorModel()
        {
            this.DATA = new NotifyCrossDoorCmdModel();
        }
        public int CMD_TRIGGER { get; set; }
        public int CMD_NO { get; set; }
        public int CMD_SEQ { get; set; }
        public NotifyCrossDoorCmdModel DATA { get; set; }

        public override string ToString()
        {
            return this.ToJson();
        }

    }
    public class NotifyCrossDoorCmdModel
    {
        /// <summary>
        /// Wms指令ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 区域ID 01，入库  02出库
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string PalletBarcode { get; set; }
    }
}
