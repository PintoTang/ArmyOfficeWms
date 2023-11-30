using System.Collections.Generic;
using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using Newtonsoft.Json;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class SyncNewResReMsg 
    {
  
        public List<SyncNewResReMsgData> SyncNewResReMsgDataList { get; set; }

        public static explicit operator SyncNewResReMsg(string json)
        {
            return json.ToObject<SyncNewResReMsg>();
        }

        public override string ToString()
        {
            if (SyncNewResReMsgDataList == null)
            {
                SyncNewResReMsgDataList=new List<SyncNewResReMsgData>();
               return  SyncNewResReMsgDataList.ToJson();
            }
            else
            {
                return SyncNewResReMsgDataList.ToJson();
                //return this.ToJson();
            }
         
        }
        public string ToJsonMsg()
        {
           return this.ToJson();
        }

        public object ToObject(string json)
        {
            return json.ToObject<SyncNewResReMsg>();
        }
    }

    public class SyncNewResReMsgData
    {
        public string INANDOUTPORT { get; set; }
        public HandleResult RESULT { get; set; }

        public string MESSAGE { get; set; }
    }
}
