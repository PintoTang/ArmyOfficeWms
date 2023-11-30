using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 库房地址Model
    /// </summary>
    [SugarTable("T_BO_WH_ADDRESS", "")]
    public class WhAddressModel
    {
        public WhAddressModel()
        {
            this.WhCode = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 仓库编号
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string WhCode {
            get;set;
        }
        /// <summary>
        /// Wcs的地址
        /// </summary>
        [SugarColumn(ColumnName = "WCS_ADDR", Length = 255, IsNullable = false, ColumnDescription = "WCS系统的地址编码")]
        public string WcsAddr { get; set; }
        /// <summary>
        /// Wms的地址
        /// </summary>
        [SugarColumn(ColumnName = "UPPER_ADDR", Length = 255, IsNullable = false, ColumnDescription = "上层系统的地址编码")]
        public string UpperAddr { get; set; }
        /// <summary>
        /// 显示名字
        /// </summary>
        [SugarColumn(ColumnName = "SHOW_NAME", Length = 255, IsNullable = false, ColumnDescription = "展示的信息")]
        public string ShowName { get; set; }
        /// <summary>
        /// 底层名字
        /// </summary>
        [SugarColumn(ColumnName = "LOWER_ADDR", Length = 255, IsNullable = true, ColumnDescription = "下层系统地址编码")]
        public string LowerAddr { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public bool IsSelected { get; set; }
    }
    /// <summary>
    /// 库房地址 筛选类
    /// </summary>
    public struct WhAddressDataFilter
    {
        /// <summary>
        /// 页索引
        /// </summary>
        public long PageIndex { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// wcs地址
        /// </summary>
        public string WcsAddress { get; set; }
        /// <summary>
        /// 上层地址
        /// </summary>
        public string UpperAddress { get; set; }
        /// <summary>
        /// 下层地址
        /// </summary>
        public string LowerAddress { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ShowName { get; set; }
    }

}
