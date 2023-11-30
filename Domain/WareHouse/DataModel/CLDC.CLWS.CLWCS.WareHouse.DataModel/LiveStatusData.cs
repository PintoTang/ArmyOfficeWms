using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 实时数据结构类
    /// </summary>
    [SugarTable("T_AC_LIVESTATUS", "")]
    public class LiveStatusData
    {
        public LiveStatusData()
        {
            this.WH_Code = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "DEVICE_ID", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "设备编号")]
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "NAME", Length = 100, IsNullable = true, ColumnDescription = "设备名字")]
        public string Name { get; set; }



        /// <summary>
        /// 使用状态
        /// </summary>
        [SugarColumn(ColumnName = "USESTATE", IsNullable = true, ColumnDescription = "设备使用状态")]
        public int? UseState { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        [SugarColumn(ColumnName = "RUNSTATE", IsNullable = true, ColumnDescription = "设备运行状体")]
        public int? RunState { get; set; }

        /// <summary>
        /// 控制状态
        /// </summary>
        [SugarColumn(ColumnName = "CONTROLSTATE", IsNullable = true, ColumnDescription = "设备控制状态")]
        public int? ControlState { get; set; }

        /// <summary>
        /// 调度状态
        /// </summary>
        [SugarColumn(ColumnName = "DISPATCHSTATE", IsNullable = true, ColumnDescription = "设备调度状态")]
        public int? DispatchState { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [SugarColumn(ColumnName = "ALIAS", Length = 100, IsNullable = true, ColumnDescription = "设备别名")]
        public string Alias { get; set; }

        /// <summary>
        /// 库房编号
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 20, IsNullable = false, ColumnDescription = "库房编号")]
        public string WH_Code { get; set; }



    }
}
