using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLWCS.WareHouse.Device.HeFei.Simulate3D.Model
{
    public class ResponseResult
    {
        public int error { get; set; }

        public string error_message { get; set; }


        public static explicit operator ResponseResult(string json)
        {
            return json.ToObject<ResponseResult>();
        }
        public override string ToString()
        {
            return this.ToJson();
        }

       
    }
}
