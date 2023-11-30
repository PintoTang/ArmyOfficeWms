using System;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.Model
{
    public class ClouRcsResSucMsg : IResponse
    {
        public bool success { get; set; }

        public string message { get; set; }
        [JsonIgnore]
        public bool IsSuccess
        {
            get { return success; }
            set
            {
                if (value)
                {
                    success = true;
                    message = "成功";
                }
                else
                {
                    success = false;
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
            return json.ToObject<ClouRcsResSucMsg>();
        }
        public static ClouRcsResSucMsg CreateSuccessResult()
        {
            return new ClouRcsResSucMsg()
            {
                success = true,
                message = "成功"
            };
        }

        public static ClouRcsResSucMsg CreateFailedResult(string errorMsg)
        {
            return new ClouRcsResSucMsg()
            {
                success = false,
                message = errorMsg
            };
        }
        public static explicit operator ClouRcsResSucMsg(string json)
        {
            return json.ToObject<ClouRcsResSucMsg>();
        }
        public static ClouRcsResSucMsg CreateFailedResult()
        {
            return new ClouRcsResSucMsg()
            {
                success = true,
                message = "失败"
            };
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        public string ToSuccessMsg()
        {
            ClouRcsResSucMsg successResponse = CreateSuccessResult();
            return successResponse.ToJsonMsg();
        }

        public string ToFailMsg()
        {
            ClouRcsResSucMsg successResponse = CreateFailedResult("失败");
            return successResponse.ToJsonMsg();
        }
    }
}
