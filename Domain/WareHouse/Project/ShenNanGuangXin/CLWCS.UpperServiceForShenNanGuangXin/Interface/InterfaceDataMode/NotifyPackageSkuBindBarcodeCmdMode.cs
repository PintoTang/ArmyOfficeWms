using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using Newtonsoft.Json;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyPackageSkuBindBarcodeCmdMode
    {
        public NotifyPackageSkuBindBarcodeCmdMode()
        {
            this.DATA = new PackageSkuBindParaMode();
        }
        public int CMD_TRIGGER { get; set; }
        public int CMD_NO { get; set; }
        public int CMD_SEQ { get; set; }
        public PackageSkuBindParaMode DATA { get; set; }
       
        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class PackageSkuBindParaMode
    {
        /// <summary>
        /// 1-入库；2-出库/拣选;3-盘点；4-库存整理
        /// </summary>
        public int BusinessType
        {
            get; set;
        }

        /// <summary>
        /// 托盘号：入库、库存整理取入库码垛位的托盘号；出库/拣选、盘点取下料位托盘号  (传码垛位1008托盘号)
        /// </summary>
        public string PalletBarcode
        {
            get; set;
        }

        /// <summary>
        /// 抓取位置：1号上料位-GetGoodsPort:1_1_1;
        /// 2号上料位-GetGoodsPort:2_1_1;
        /// 入库码垛位-PutGoodsPort:1_1_1；
        /// NG台-NGPort:1_1_1
        /// 出库码垛位-GetGoodsPort:3_1_1
        /// </summary>
        public string SrcAddr
        {
            get; set;
        }

        /// <summary>
        /// 包装条码
        /// </summary>
        public List<PackageCheckDetailModel> PackageCheckData
        {
            get; set;
        }
    }

    /// <summary>
    /// 包装明细数据
    /// </summary>
    public class PackageCheckDetailModel
    {
        /// <summary>
        /// 包装条码
        /// </summary>
        public string PackageBarcode
        {
            get; set;
        }

        /// <summary>
        /// 起始位置号
        /// </summary>
        public int SrcPosIndex
        {
            get; set;
        }
       
    }

    public class NotifyPackageBarcodeCheckResponse
    {
        /// <summary>
        /// 1去目标位置、2去起始位、3放NG
        /// </summary>
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
       
        public override string ToString()
        {
            return this.ToJson();
        }
    }


   

}
