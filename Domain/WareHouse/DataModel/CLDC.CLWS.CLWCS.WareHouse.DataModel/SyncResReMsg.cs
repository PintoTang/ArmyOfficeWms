using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 处理结果枚举
    /// </summary>
    public enum HandleResult
    {
        [Description("处理失败")]
        Failed = 0,
        [Description("处理成功")]
        Success = 1
    }
    /// <summary>
    /// 接收消息类
    /// </summary>
    public class SyncResReMsg : IResponse
    {
        public SyncResReMsg()
        {
            
        }
        /// <summary>
        /// 处理结果
        /// </summary>
        public HandleResult RESULT { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string MESSAGE { get; set; }
        /// <summary>
        /// json转换SyncResReMsg对象
        /// </summary>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static explicit operator SyncResReMsg(string json)
        {
            return json.ToObject<SyncResReMsg>();
        }
        /// <summary>
        /// 创建成功对象
        /// </summary>
        /// <returns></returns>
        public static SyncResReMsg CreateSuccessResult()
        {
            return new SyncResReMsg()
            {
                RESULT = HandleResult.Success,
                MESSAGE = "成功"
            };
        }
        /// <summary>
        /// 创建失败对象
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static SyncResReMsg CreateFailedResult(string errorMsg)
        {
            return new SyncResReMsg()
            {
                RESULT = HandleResult.Failed,
                MESSAGE = errorMsg
            };
        }
        /// <summary>
        /// 创建失败对象
        /// </summary>
        /// <returns></returns>
        public static SyncResReMsg CreateFailedResult()
        {
            return new SyncResReMsg()
            {
                RESULT = HandleResult.Failed,
                MESSAGE = "失败"
            };
        }
        /// <summary>
        /// 转json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToJson();
        }

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
        /// <summary>
        /// 对象转json信息
        /// </summary>
        /// <returns></returns>
        public string ToJsonMsg()
        {
           return this.ToJson();
        }
        /// <summary>
        /// json转换成SyncResReMsg对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object ToObject(string json)
        {
            return json.ToObject<SyncResReMsg>();
        }
        /// <summary>
        /// 生成成功消息
        /// </summary>
        /// <returns>json</returns>
        public string ToSuccessMsg()
        {
            SyncResReMsg successResponse = CreateSuccessResult();
            return successResponse.ToJsonMsg();
        }
        /// <summary>
        /// 生成失败消息
        /// </summary>
        /// <returns>json</returns>
        public string ToFailMsg()
        {
            SyncResReMsg successResponse = CreateFailedResult("失败");
            return successResponse.ToJsonMsg();
        }

    }
}
