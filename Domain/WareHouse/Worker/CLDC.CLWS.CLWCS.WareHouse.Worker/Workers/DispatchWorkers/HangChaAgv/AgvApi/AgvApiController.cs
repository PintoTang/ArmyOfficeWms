using System.Web.Http;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.AgvApi
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
    }

    public class AgvApiController : ApiController
    {
        [HttpGet]
        [HttpPost]
        public string NotifyDeviceStatus(string cmd)
        {
            IHangChaAgvApi hangChaApi = DependencyHelper.GetService<IHangChaAgvApi>();
            ResponseResult respones=new ResponseResult(1,"失败");
            if (hangChaApi != null)
            {
                OperateResult result = hangChaApi.NotifyDeviceStatus(cmd);
                if (!result.IsSuccess)
                {
                    respones.RESULT = 0;
                    respones.MESSAGE = result.Message;
                    return JsonConvert.SerializeObject(respones);
                }
                respones.RESULT = 1;
                respones.MESSAGE = "成功";
                return JsonConvert.SerializeObject(respones);
            }
            respones.MESSAGE = "接口未实现";
            return JsonConvert.SerializeObject(respones);

        }

        [HttpPost]
        [HttpGet]
        public string NotifyExeResult(string cmd)
        {
            IHangChaAgvApi hangChaApi = DependencyHelper.GetService<IHangChaAgvApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (hangChaApi != null)
            {
                OperateResult result = hangChaApi.NotifyExeResult(cmd);
                if (!result.IsSuccess)
                {
                    respones.RESULT = 0;
                    respones.MESSAGE = result.Message;
                    return JsonConvert.SerializeObject(respones);
                }
                respones.RESULT = 1;
                respones.MESSAGE = "成功";
                return JsonConvert.SerializeObject(respones);
            }
            respones.MESSAGE = "接口未实现";
            return JsonConvert.SerializeObject(respones);
        }
    }
}
