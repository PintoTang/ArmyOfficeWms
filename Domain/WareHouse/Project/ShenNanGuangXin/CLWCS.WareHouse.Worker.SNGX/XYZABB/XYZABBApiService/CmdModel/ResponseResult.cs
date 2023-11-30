using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
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
    public class ResponseEmptyResult
    {
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
