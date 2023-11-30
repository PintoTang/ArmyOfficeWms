using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using Newtonsoft.Json;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    /// <summary>
    /// 条码扫描 类
    /// </summary>
    public class NotifyPackageSkuBindBarcodeCmdModeNew
    {
        public NotifyPackageSkuBindBarcodeCmdModeNew()
        {
            this.DATA = new PackageSkuBindParaModeNew();
        }
        public int CMD_TRIGGER { get; set; }
        public int CMD_NO { get; set; }
        public int CMD_SEQ { get; set; }
        public PackageSkuBindParaModeNew DATA { get; set; }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class PackageSkuBindParaModeNew
    {
        /// <summary>
        /// 1-入库；2-出库/拣选;3-盘点；4-库存整理
        /// </summary>
        public int BusinessType { get; set; }

        /// <summary>
        /// 托盘号：入库、库存整理取入库码垛位的托盘号；出库/拣选、盘点取下料位托盘号  (传码垛位1008托盘号)
        /// </summary>
        public string PalletBarcode { get; set; }
        
        /// <summary>
        /// 抓取位置：1号上料位-GetGoodsPort:1_1_1;
        /// 2号上料位-GetGoodsPort:2_1_1;
        /// 入库码垛位-PutGoodsPort:1_1_1；
        /// NG台-NGPort:1_1_1
        /// 出库码垛位-GetGoodsPort:3_1_1
        /// </summary>
        public string SrcAddr { get; set; }
       

        /// <summary>
        /// 包装条码
        /// </summary>
        public List<PackageCheckDetailModelNew> PackageCheckData
        {
            get; set;
        }
    }

    /// <summary>
    /// 包装明细数据
    /// </summary>
    public class PackageCheckDetailModelNew
    {
        /// <summary>
        /// 包装条码
        /// </summary>
        public string PackageBarcode { get; set; }
       
        /// <summary>
        /// 起始位置号
        /// </summary>
        public int SrcPosIndex { get; set; }

    }

    public class NotifyPackageBarcodeCheckNewResponse
    {
        /// <summary>
        /// 结果:0-命令解析失败;1-成功;-1-命令接收失败 ,
        /// </summary>
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }

        public CmdPackageCheckResultNewModel DATA;
        public override string ToString()
        {
            return this.ToJson();
        }

    }

    /// <summary>
    /// 包装检查结果数据
    /// </summary>
    public class CmdPackageCheckResultNewModel
    {
        /// <summary>
        /// 目标位置：1-验证成功，放码垛位;2：原位置；3-NG台；4需要放走，回原位
        /// </summary>
        [JsonProperty("DestAddrType")]
        public int DestAddrType { get; set; }

        /// <summary>
        /// 包装验证明细数据
        /// </summary>
        public List<PackageCheckDetailResultModel> CheckDetailData { get; set; }
       
    }

    /// <summary>
    /// 数据明细
    /// </summary>
    public class PackageCheckDetailResultModel
    {
        /// <summary>
        /// 包装条码
        /// </summary>
        [JsonProperty("PackageBarcode")]
        public string PackageBarcode { get; set; }
       
        /// <summary>
        /// 物料号
        /// </summary>
        [JsonProperty("MaterialCode")]
        public string MaterialCode { get; set; }
       
        /// <summary>
        /// 批次号
        /// </summary>
        [JsonProperty("BatchNO")]
        public string BatchNO { get; set; }
       
    }

}
