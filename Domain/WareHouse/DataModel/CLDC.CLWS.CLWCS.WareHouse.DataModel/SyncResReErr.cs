using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{

    /// <summary>
    /// 返回结果类
    /// </summary>
    public class SyncResReErr : IResponse
    {
        public SyncResReErr()
        {
            
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="result">HandleResult</param>
        /// <param name="msg">信息</param>
        public SyncResReErr(HandleResult result, string msg)
        {
            this.RESULT = result;
            this.ERR_MSG = msg;
        }
        /// <summary>
        /// 结果
        /// </summary>
        public HandleResult RESULT { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string ERR_MSG { get; set; }

        public object DATA
        {
            get; set;
        }
        /// <summary>
        /// 反序列化json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>

        public static explicit operator SyncResReErr(string json)
        {
            return JsonConvert.DeserializeObject<SyncResReErr>(json);
        }
        /// <summary>
        /// 创建成功信息
        /// </summary>
        /// <returns></returns>
        public static SyncResReErr CreateSuccessResult()
        {
            return new SyncResReErr()
            {
                RESULT = HandleResult.Success,
                ERR_MSG = "成功"
            };
        }
        /// <summary>
        /// 创建失败信息
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static SyncResReErr CreateFailedResult(string errorMsg)
        {
            return new SyncResReErr()
            {
                RESULT = HandleResult.Failed,
                ERR_MSG = errorMsg
            };
        }
        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <returns></returns>
        public static SyncResReErr CreateFailedResult()
        {
            return new SyncResReErr()
            {
                RESULT = HandleResult.Failed,
                ERR_MSG = "失败"
            };
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess
        {
            get { return RESULT == HandleResult.Success; }
        }

        /// <summary>
        /// 转换json
        /// </summary>
        /// <returns></returns>
        public string ToJsonMsg()
        {
            return this.ToJson();
        }
        /// <summary>
        /// 转换object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object ToObject(string json)
        {
            return json.ToObject<SyncResReErr>();
        }
        /// <summary>
        /// 成功消息json
        /// </summary>
        /// <returns></returns>
        public string ToSuccessMsg()
        {
           SyncResReErr successResponse=new SyncResReErr(HandleResult.Success,"成功");
            return successResponse.ToJsonMsg();
        }
        /// <summary>
        /// 失败消息json
        /// </summary>
        /// <returns></returns>
        public string ToFailMsg()
        {
            SyncResReErr successResponse = new SyncResReErr( HandleResult.Failed, "失败");
            return successResponse.ToJsonMsg();
        }


        public override string ToString()
        {
            return ToJsonMsg();
        }
    }
}
