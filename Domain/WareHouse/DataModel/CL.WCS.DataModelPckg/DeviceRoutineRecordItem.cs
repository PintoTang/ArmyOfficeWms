using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 设备运行状态信息
    /// </summary>
    public class DeviceRoutineRecordItem
    {
        /// <summary>
        /// 设备日常记录ID
        /// </summary>
       public string  Guid
       {
           get;
           set;
       }
        /// <summary>
        /// 设备代码
        /// </summary>
        public string  DeviceName
       {
           get;
           set;
       }
        /// <summary>
        /// 库房代码
        /// </summary>
        public string  WareHouseCode
       {
           get;
           set;
       }
        /// <summary>
        /// 设备日常工作类型类型
        /// </summary>
        public int  DeviceRoutineType
       {
           get;
           set;
       }
        /// <summary>
        /// 记录开始时间
        /// </summary>
       public string  DeviceRoutineStartTime
       {
           get;
           set;
       }
        /// <summary>
        /// 记录结束时间
        /// </summary>
       public string DeviceRoutineEndTime
       {
           get;
           set;
       }
        /// <summary>
        /// 产生记录的工作编号
        /// </summary>
        public string  WordNO
       {
           get;
           set;
       }
        /// <summary>
        /// 详细记录设备信息
        /// </summary>
        public string  Sunmary
       {
           get;
           set;
       }   
        /// <summary>
        /// 异常代码
        /// </summary>
        public string  ErrorCode
       {
           get;
           set;
       }
        /// <summary>
        /// 异常描述信息
        /// </summary>
        public string  ErrorText
       {
           get;
           set;
       }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string DeviceRoutineCreationDate
       {
           get;
           set;
       }
        /// <summary>
        /// 转换成DeviceRoutineRecordItem格式
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static explicit operator DeviceRoutineRecordItem(DataRow dr)
        {
            DeviceRoutineRecordItem deviceRoutineRecordItem = new DeviceRoutineRecordItem();
            deviceRoutineRecordItem.Guid = dr["DRR_GUID"].ToString();
            deviceRoutineRecordItem.DeviceName=dr["DD_Code"].ToString();
            deviceRoutineRecordItem.WareHouseCode = dr["WH_Code"].ToString();
            deviceRoutineRecordItem.DeviceRoutineType = int.Parse(dr["DRT_ID"].ToString());
            return deviceRoutineRecordItem;
        }
    }
}
