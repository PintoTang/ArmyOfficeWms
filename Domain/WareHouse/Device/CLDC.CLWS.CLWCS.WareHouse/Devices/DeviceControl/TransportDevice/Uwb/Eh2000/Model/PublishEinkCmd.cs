using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.Model
{
    public class PublishEinkCmd
    {
        public PublishEinkCmd()
        {
        }

        public PublishEinkCmd(string cardId, string message, EinkNotifyTypeEnum notifyType, EinkVoiceTypeEnum voiceType, EinkShakeTypeEnum shakeType)
        {
            card_ids = cardId;
            this.message = message;
            this.type = notifyType;
            this.voice = voiceType;
            this.shake = shakeType;
        }
        public string card_ids { get; set; }
        public string message { get; set; }
        public EinkNotifyTypeEnum type { get; set; }

        public double time
        {
            get
            {
                TimeSpan ts1 = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 30, 0);
                return Math.Round(ts1.TotalSeconds);
            }
        }

        public EinkVoiceTypeEnum voice { get; set; }

        public EinkShakeTypeEnum shake { get; set; }

        public override string ToString()
        {
            //Hashtable paraTable=new Hashtable();
            //paraTable.Add("message","Test");
            //paraTable.Add("card_ids", "44579");
            //paraTable.Add("type",0);
            //paraTable.Add("time", 1598859988);
            //paraTable.Add("voice",0);
            //paraTable.Add("shake",0);
            string formCmd = string.Format("message={0}&card_ids={1}&type={2}&time={3}&voice={4}&shake={5}",
                this.message, this.card_ids, (int)this.type, time, (int)voice, (int)shake);
            return formCmd;
        }
    }

    public enum EinkNotifyTypeEnum
    {
        [Description("通知")]
        Notice = 0,
        [Description("报警")]
        Alarm = 1,
    }

    public enum EinkVoiceTypeEnum
    {
        [Description("无声音")]
        NoVoice = 0,
        [Description("有声音")]
        HasVoice = 1
    }

    public enum EinkShakeTypeEnum
    {
        [Description("无震动")]
        NoShake = 0,
        [Description("有震动")]
        HasShake = 1
    }

    public class ResponseResult : IResponse
    {
       public ResponseResult()
        {
            
        }

       public ResponseResult(int result, string msg)
        {
            this.result = result;
            this.message = msg;
        }
        public string message { get; set; }
        public int result { get; set; }

        [JsonIgnore]
        public bool IsSuccess
        {
            get { return result == 1; }
        }

        public string ToJsonMsg()
        {
            return this.ToJson();
        }

        public object ToObject(string json)
        {
            return json.ToObject<ResponseResult>();
        }

        public string ToSuccessMsg()
        {
            ResponseResult successResponse = new ResponseResult(1,"成功");
            return successResponse.ToJsonMsg();
        }

        public string ToFailMsg()
        {
            ResponseResult failResponse = new ResponseResult(1, "失败");
            return failResponse.ToJsonMsg();
        }
    }


}
