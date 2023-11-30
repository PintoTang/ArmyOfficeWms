using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.DataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 搬运信息的数据库信息模型
    /// </summary>
    [SugarTable("T_BO_TRANSPORTDATA", "")]
    public class TransportMsgModel
    {
        public TransportMsgModel()
        {
            this.WhCode = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 搬运信息的ID
        /// </summary>
        [SugarColumn(ColumnName = "TRANSPORTID", IsNullable = true, ColumnDescription = "")]
        public int? TransportId { get; set; }

        /// <summary>
        /// 开始设备的ID
        /// </summary>
        [SugarColumn(ColumnName = "STARTID", IsNullable = true, ColumnDescription = "")]
        public int? StartId { get; set; }

        /// <summary>
        /// 结束设备的ID
        /// </summary>
        [SugarColumn(ColumnName = "DESTID", IsNullable = true, ColumnDescription = "")]
        public int? DestId { get; set; }

        /// <summary>
        /// 搬运指令的ID
        /// </summary>
        [SugarColumn(ColumnName = "EXORDERID", IsNullable = true, ColumnDescription = "")]
        public int? ExOrderId { get; set; }

        /// <summary>
        /// 开始地址
        /// </summary>
        [SugarColumn(ColumnName = "STARTADDR", Length = 255, IsNullable = true, ColumnDescription = "")]
        public string StartAddr { get; set; }

        /// <summary>
        /// 当前地址
        /// </summary>
        [SugarColumn(ColumnName = "CURADDR", Length = 255, IsNullable = true, ColumnDescription = "")]
        public string CurAddr { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        [SugarColumn(ColumnName = "DESTADDR", Length = 255, IsNullable = true, ColumnDescription = "")]
        public string DestAddr { get; set; }
        /// <summary>
        /// 垛号
        /// </summary>
        [SugarColumn(ColumnName = "PILENO", Length = 510, IsNullable = true, ColumnDescription = "跺号")] 
        public string PileNo { get; set; }

        /// <summary>
        /// 拥有者的名称
        /// </summary>
        [SugarColumn(ColumnName = "OWNERID", Length = 255, IsNullable = false, ColumnDescription = "拥有者ID")]
        public int? OwnerId { get; set; }

        /// <summary>
        /// 搬运结果状态
        /// </summary>
        [SugarColumn(ColumnName = "STATUS", IsNullable = true, ColumnDescription = "")]
        public TransportResultEnum TransportStatus { get; set; }
        /// <summary>
        /// 搬运信息完成的方式
        /// </summary>
        [SugarColumn(ColumnName = "FINISHTYPE", IsNullable = true, ColumnDescription = "完成类型")]
        public FinishType TransportFinishType { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "GUID", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string Guid { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [SugarColumn(ColumnName = "ADDDATETIME", IsNullable = true, ColumnDescription = "")]
        public DateTime? AddDateTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [SugarColumn(ColumnName = "UPDATEDATETIME", IsNullable = true, ColumnDescription = "")]
        public DateTime? UpdateDateTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string WhCode { get; set; }

        /// <summary>
        /// 搬运信息完成的方式
        /// </summary>
        [SugarColumn(ColumnName = "TrayType", IsNullable = true, ColumnDescription = "托盘类型")]
        public int? TrayType { get; set; }
    }
}
