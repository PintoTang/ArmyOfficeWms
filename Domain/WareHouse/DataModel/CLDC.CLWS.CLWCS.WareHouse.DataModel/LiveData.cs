using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.DataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    [SugarTable("T_AC_LIVESDATA", "")]
    public sealed class LiveData:IUnique,IDisposable
    {
        public LiveData()
        {
            WH_Code = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "DEVICE_ID", IsNullable = true, ColumnDescription = "")]
        public int? DeviceId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "DEVICE_NAME", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string Name { get; set; }

        /// <summary>
        /// 库房编号
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string WH_Code { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [SugarColumn(ColumnName = "ALIAS", Length = 255, IsNullable = true, ColumnDescription = "")]
        public string Alias { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [SugarColumn(ColumnName = "DATAVALUE", Length = 255, IsNullable = true, ColumnDescription = "")]
        public string DataValue { get; set; }

        /// <summary>
        /// 值序号
        /// </summary>
        [SugarColumn(ColumnName = "DATA_INDEX", IsNullable = true, ColumnDescription = "")]
        public int? Index { get; set; }
        /// <summary>
        /// 数据的处理状态
        /// </summary>
        [SugarColumn(ColumnName = "HANDLESTATUS", IsNullable = true, ColumnDescription = "")]
        public HandleStatusEnum? HandleStatus { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [SugarColumn(IsIgnore =true)]
        public string UniqueCode {
            get { return (DeviceId + Index).ToString(); }
            set { } }


        public override bool Equals(object obj)
        {
            if (obj is LiveData)
            {
                LiveData liveData = obj as LiveData;
                if (liveData.UniqueCode.Equals(this.UniqueCode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            return;
        }
    }
    /// <summary>
    /// 处理状态 枚举
    /// </summary>
    public enum HandleStatusEnum
    {
        /// <summary>
        /// 未处理完成
        /// </summary>
        UnFinished=0,
        /// <summary>
        /// 处理完成
        /// </summary>
        Finished=1
    }

}
