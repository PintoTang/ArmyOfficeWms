using System;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class XYZABBResSucMsg : IResponse
    {
        public int error { get; set; }

        public string error_message { get; set; }
        [JsonIgnore]
        public bool IsSuccess
        {
            get { return error == 0; }
            set
            {
                if (value)
                {
                    error = 0;
                    error_message = "成功";
                }
                else
                {
                    error = 1;
                    error_message = "失败";
                }
            }
        }
        public string ToJsonMsg()
        {
            return this.ToJson();
        }

        public object ToObject(string json)
        {
            return json.ToObject<XYZABBResSucMsg>();
        }
        public static XYZABBResSucMsg CreateSuccessResult()
        {
            return new XYZABBResSucMsg()
            {
                error = 0,
                error_message = "成功"
            };
        }

        public static XYZABBResSucMsg CreateFailedResult(string errorMsg)
        {
            return new XYZABBResSucMsg()
            {
                error = 1,
                error_message = errorMsg
            };
        }
        public static explicit operator XYZABBResSucMsg(string json)
        {
            return json.ToObject<XYZABBResSucMsg>();
        }
        public static XYZABBResSucMsg CreateFailedResult()
        {
            return new XYZABBResSucMsg()
            {
                error = 1,
                error_message = "失败"
            };
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        public string ToSuccessMsg()
        {
            XYZABBResSucMsg successResponse = CreateSuccessResult();
            return successResponse.ToJsonMsg();
        }

        public string ToFailMsg()
        {
            XYZABBResSucMsg successResponse = CreateFailedResult("失败");
            return successResponse.ToJsonMsg();
        }
    }
}
