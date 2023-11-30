using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLWCS.WareHouse.Device.HeFei.Simulate3D.Model
{
    public class ResponseResult2
    {
        public string result { get; set; }

        public string message { get; set; }


        public static explicit operator ResponseResult2(string json)
        {
            return json.ToObject<ResponseResult2>();
        }
        public override string ToString()
        {
            return this.ToJson();
        }


    }
}
