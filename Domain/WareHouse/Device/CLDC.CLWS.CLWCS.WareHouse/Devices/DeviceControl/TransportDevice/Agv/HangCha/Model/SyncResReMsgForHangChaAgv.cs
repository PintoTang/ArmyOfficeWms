using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model
{
   
    public class SyncResReMsgForHangChaAgv : IResponse
    {
        public SyncResReMsgForHangChaAgv()
        {
            
        }
        public int code { get; set; }

        public string msg { get; set; }


        public static explicit operator SyncResReMsgForHangChaAgv(string json)
        {
            return json.ToObject<SyncResReMsgForHangChaAgv>();
        }

        public static SyncResReMsgForHangChaAgv CreateSuccessResult()
        {
            return new SyncResReMsgForHangChaAgv()
            {
                code = 0,
                msg = "成功"
            };
        }

        public static SyncResReMsgForHangChaAgv CreateFailedResult(string errorMsg)
        {
            return new SyncResReMsgForHangChaAgv()
            {
                code = 1,
                msg = errorMsg
            };
        }

        public static SyncResReMsgForHangChaAgv CreateFailedResult()
        {
            return new SyncResReMsgForHangChaAgv()
            {
                code = 1,
                msg = "失败"
            };
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        [JsonIgnore]
        public bool IsSuccess
        {
            get { return code == 0; }
            set
            {
                if (value)
                {
                    code = 0;
                    msg = "成功";
                }
                else
                {
                    code = 1;
                    msg = "失败";
                }
            }
        }

        public string ToJsonMsg()
        {
           return this.ToJson();
        }

        public object ToObject(string json)
        {
            return json.ToObject<SyncResReMsgForHangChaAgv>();
        }

        public string ToSuccessMsg()
        {
            SyncResReMsgForHangChaAgv successResponse = CreateSuccessResult();
            return successResponse.ToJsonMsg();
        }

        public string ToFailMsg()
        {
            SyncResReMsgForHangChaAgv successResponse = CreateFailedResult("失败");
            return successResponse.ToJsonMsg();
        }

    }
}
