using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyPackagePutFinishForOutMode
    {
        private NotifyPackagePutFinishForOutDataMode _data = new NotifyPackagePutFinishForOutDataMode();

        public NotifyPackagePutFinishForOutDataMode DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class NotifyPackagePutFinishForOutDataMode
    {
        public string PalletBarcode { get; set; }
 
        private List<int> _PosIndexs = new List<int>();
        public List<int> PosIndexs
        {
            get { return _PosIndexs; }

            set { _PosIndexs = value; }
        }
    }

    public class NotifyPackagePutFinishNewModel
    {
        public NotifyPackagePutFinishNewModel()
        {
            this.PackageDetailData = new List<PackagePutDetailModel>();
        }
        /// <summary>
        /// 1-入库；2-出库/拣选；3-盘点；4-库存整理
        /// </summary>
        public int BusinessType
        {
            get; set;
        }

        /// <summary>
        /// 垛号
        /// </summary>
        public string PalletBarcode
        {
            get;set;
        }

        /// <summary>
        /// 抓取/起始地址：1-上料位1；2-上料位2；3-码垛位；4-NG台；5-下料位；6-出库码垛位
        /// </summary>
        public string SrcAddr
        {
            get;set;
        }

        /// <summary>
        /// 抓取/起始地址：1-上料位1；2-上料位2；3-码垛位；4-NG台；5-下料位；6-出库码垛位
        /// </summary>
        public string DestAddr
        {
            get; set;
        }
        /// <summary>
        /// 起始是否已空
        /// </summary>
        public bool SrcIsEmpty { get; set; }
        /// <summary>
        ///  目标是否已满
        /// </summary>
        public bool DestIsFull { get; set; }
        /// <summary>
        /// 称重
        /// </summary>
        public int Weight { get; set; }

    /// <summary>
    /// 包装明细
    /// </summary>
    public List<PackagePutDetailModel> PackageDetailData
        {
            get;set;
        }

    }

    public class PackagePutDetailModel
    {
        /// <summary>
        /// 包装条码
        /// </summary>
        public string PackageBarcode
        {
            get;set;
        }

        /// <summary>
        /// 起始/抓取位置号  （扫描-原位 0）
        /// </summary>
        public int SrcPosIndex
        {
            get;set;
        }
        /// <summary>
        /// 目标/放下位置号
        /// </summary>
        public int DestPosIndex
        {
            get;set;
        }

    }
}
