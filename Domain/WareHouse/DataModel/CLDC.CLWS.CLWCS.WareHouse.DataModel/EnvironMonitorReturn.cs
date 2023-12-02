using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    public class EnvironMonitorReturn : IResponse
    {
        /// <summary>
        /// 处理结果
        /// </summary>
        public HandleResult RESULT { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string MESSAGE { get; set; }

        public List<EnvironMonitorModel> data { get; set; }

        [JsonIgnore]
        public bool IsSuccess
        {
            get { return RESULT == HandleResult.Success; }
            set
            {
                if (value)
                {
                    RESULT = HandleResult.Success;
                    MESSAGE = "成功";
                }
                else
                {
                    RESULT = HandleResult.Failed;
                    MESSAGE = "失败";
                }
            }
        }

        public string ToFailMsg()
        {
            throw new System.NotImplementedException();
        }

        public string ToJsonMsg()
        {
            throw new System.NotImplementedException();
        }

        public object ToObject(string json)
        {
            throw new System.NotImplementedException();
        }

        public string ToSuccessMsg()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// json转换SyncResReMsg对象
        /// </summary>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static explicit operator EnvironMonitorReturn(string json)
        {
            return json.ToObject<EnvironMonitorReturn>();
        }

        /// <summary>
        /// 转json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToJson();
        }

    }

    public class EnvironMonitorModel
    {
        /// <summary>
        /// 组编号
        /// </summary>
        public string groupId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceKey { get; set; }
        /// <summary>
        /// 设备地址
        /// </summary>
        public int deviceAddr { get; set; }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int nodeID { get; set; }
        /// <summary>
        /// 节点类型 1:模拟量 1 启用;2:模拟量 2 启用;3:同时启用
        /// </summary>
        public int nodeType { get; set; }
        /// <summary>
        /// 停用状态:true-停用
        /// </summary>
        public bool deviceDisabled { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public float lat { get; set; }
        /// <summary>
        /// 设备运行状态，0 未运行，1 离线，2 在线
        /// </summary>
        public int deviceStatus { get; set; }
        /// <summary>
        /// 实时数据
        /// </summary>
        public List<RealTimeData> realTimeData { get; set; }
    }

    /// <summary>
    /// 实时数据
    /// </summary>
    public class RealTimeData
    {
        /// <summary>
        /// 模拟量名称
        /// </summary>
        public string dataName { get; set; }
        /// <summary>
        /// 实时数据
        /// </summary>
        public string dataValue { get; set; }
        /// <summary>
        /// 是否报警
        /// </summary>
        public bool isAlarm { get; set; }
        /// <summary>
        /// 报警信息
        /// </summary>
        public string alarmMsg { get; set; }
    }

}
