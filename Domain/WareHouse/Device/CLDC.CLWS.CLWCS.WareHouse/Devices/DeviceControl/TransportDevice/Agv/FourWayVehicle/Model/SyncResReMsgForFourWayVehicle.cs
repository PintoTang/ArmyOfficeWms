using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
   
    public class SyncResReMsgForFourWayVehicle : IResponse
    {
        public SyncResReMsgForFourWayVehicle()
        {
            
        }
        public int code { get; set; }

        public string msg { get; set; }


        public static explicit operator SyncResReMsgForFourWayVehicle(string json)
        {
            return json.ToObject<SyncResReMsgForFourWayVehicle>();
        }

        public static SyncResReMsgForFourWayVehicle CreateSuccessResult()
        {
            return new SyncResReMsgForFourWayVehicle()
            {
                code = 0,
                msg = "成功"
            };
        }

        public static SyncResReMsgForFourWayVehicle CreateFailedResult(string errorMsg)
        {
            return new SyncResReMsgForFourWayVehicle()
            {
                code = 1,
                msg = errorMsg
            };
        }

        public static SyncResReMsgForFourWayVehicle CreateFailedResult()
        {
            return new SyncResReMsgForFourWayVehicle()
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
            return json.ToObject<SyncResReMsgForFourWayVehicle>();
        }

        public string ToSuccessMsg()
        {
            SyncResReMsgForFourWayVehicle successResponse = CreateSuccessResult();
            return successResponse.ToJsonMsg();
        }

        public string ToFailMsg()
        {
            SyncResReMsgForFourWayVehicle successResponse = CreateFailedResult("失败");
            return successResponse.ToJsonMsg();
        }

    }
}
