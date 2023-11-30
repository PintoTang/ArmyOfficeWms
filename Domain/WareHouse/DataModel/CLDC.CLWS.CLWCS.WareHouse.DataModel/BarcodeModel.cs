using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.DataModel.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 条码Model
    /// </summary>
    public class BarcodeModel : IFilterable
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// 比对
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>bool</returns>
        public bool Filter(string condition)
        {
            if (string.IsNullOrEmpty(Barcode))
            {
                return false;
            }
            return Barcode.Contains(condition);
        }
    }
}
