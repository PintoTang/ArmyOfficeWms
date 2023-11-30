using System.Web.Http;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.RcsApi
{
    public class ResponseResult
    {
        public ResponseResult(int result, string message)
        {
            RESULT = result;
            MESSAGE = message;
        }
        /// <summary>
        /// 信息
        /// </summary>
        public string MESSAGE { get; set; }
        /// <summary>
        /// 结果 0成功
        /// </summary>
        public int RESULT { get; set; }
        public static explicit operator ResponseResult(string json)
        {
            return json.ToObject<ResponseResult>();
        }
        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class ClouRcsAgvApiController : ApiController
    {
        [HttpGet]
        [HttpPost]
        public string ReportTaskException(ReportTaskExceptionMode cmd)
        {
            IClouRcsAgvApi clouRcsAgvApi = DependencyHelper.GetService<IClouRcsAgvApi>();
            OperateResult result = clouRcsAgvApi.ReportTaskException(cmd);

            ResponseResult rtResult = new ResponseResult(0, "失败");
            rtResult.RESULT = result.IsSuccess ? 1 : 0;
            rtResult.MESSAGE = result.Message;
            return rtResult.ToString();
            //return JsonConvert.SerializeObject(rtResult);
        }

        [HttpPost]
        [HttpGet]
        public string ReportTaskResult(ReportTaskResultMode cmd)
        {
            IClouRcsAgvApi clouRcsAgvApi = DependencyHelper.GetService<IClouRcsAgvApi>();
            OperateResult result = clouRcsAgvApi.ReportTaskResult(cmd);

            ResponseResult rtResult = new ResponseResult(0, "失败");
            rtResult.RESULT = result.IsSuccess ? 1 : 0;
            rtResult.MESSAGE = result.Message;
            //return JsonConvert.SerializeObject(rtResult);
            return rtResult.ToString();
        }
    }
}
