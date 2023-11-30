using System;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class XYZABBResSucMsg2 : IResponse
    {
        public string result { get; set; }

        public string message { get; set; }
        [JsonIgnore]
        public bool IsSuccess
        {
            get { return result.Equals(0); }
            set
            {
                if (value)
                {
                    result = "1";
                    message = "成功";
                }
                else
                {
                    result = "0";
                    message = "失败";
                }
            }
        }
        public string ToJsonMsg()
        {
            return this.ToJson();
        }

        public object ToObject(string json)
        {
            return json.ToObject<XYZABBResSucMsg2>();
        }
        public static XYZABBResSucMsg2 CreateSuccessResult()
        {
            return new XYZABBResSucMsg2()
            {
                result = "1",
                message = "成功"
            };
        }

        public static XYZABBResSucMsg2 CreateFailedResult(string resultMsg)
        {
            return new XYZABBResSucMsg2()
            {
                result = "0",
                message = resultMsg
            };
        }
        public static explicit operator XYZABBResSucMsg2(string json)
        {
            return json.ToObject<XYZABBResSucMsg2>();
        }
        public static XYZABBResSucMsg2 CreateFailedResult()
        {
            return new XYZABBResSucMsg2()
            {
                result = "0",
                message = "失败"
            };
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        public string ToSuccessMsg()
        {
            XYZABBResSucMsg2 successResponse = CreateSuccessResult();
            return successResponse.ToJsonMsg();
        }

        public string ToFailMsg()
        {
            XYZABBResSucMsg2 successResponse = CreateFailedResult("失败");
            return successResponse.ToJsonMsg();
        }
    }
}
