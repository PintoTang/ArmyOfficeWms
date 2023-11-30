using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model
{
    public enum HandleResult
    {
        [Description("处理失败")]
        Failed = 0,
        [Description("处理成功")]
        Success = 1
    }
   public class ResponseResult
    {
        public HandleResult RESULT { get; set; }

        public string MESSAGE { get; set; }


        public static explicit operator ResponseResult(string json)
        {
            return json.ToObject<ResponseResult>();
        }

        public static ResponseResult CreateSuccessResult()
        {
            return new ResponseResult()
            {
                RESULT = HandleResult.Success,
                MESSAGE = "成功"
            };
        }

        public static ResponseResult CreateFailedResult(string errorMsg)
        {
            return new ResponseResult()
            {
                RESULT = HandleResult.Failed,
                MESSAGE = errorMsg
            };
        }

        public static ResponseResult CreateFailedResult()
        {
            return new ResponseResult()
            {
                RESULT = HandleResult.Failed,
                MESSAGE = "失败"
            };
        }

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
    }
}
