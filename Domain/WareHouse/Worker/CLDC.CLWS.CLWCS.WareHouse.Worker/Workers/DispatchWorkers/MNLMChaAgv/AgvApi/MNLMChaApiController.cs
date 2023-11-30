using System.Web.Http;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.AgvApi
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

    public class MNLMChaApiController : ApiController
    {
        
        [HttpGet]
        [HttpPost]
        public string NotifyDeviceStatus(MNLMNotifyDeviceStatusMode cmd)
        {
            string strJson = JsonConvert.SerializeObject(cmd);
            LogHelper.WriteLog("木牛流马AGV调用WCS接口", "NotifyDeviceStatus:" + strJson, EnumLogLevel.Info);
            IMNLMChaAgvApi xChaApi = DependencyHelper.GetService<IMNLMChaAgvApi>();
            ResponseResult respones=new ResponseResult(1,"失败");
            if (xChaApi != null)
            {
                OperateResult result = xChaApi.NotifyDeviceStatus(cmd);
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
        public string NotifyExeResult(MNLMExeResultMode cmd)
        {
            string strJson = JsonConvert.SerializeObject(cmd);
            LogHelper.WriteLog("木牛流马AGV调用WCS接口","NotifyExeResult:"+ strJson,EnumLogLevel.Info);

            IMNLMChaAgvApi xChaApi = DependencyHelper.GetService<IMNLMChaAgvApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (xChaApi != null)
            {
                OperateResult result = xChaApi.NotifyExeResult(cmd);
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
        public string NotifyDeviceInfo(MNLMNotifyDeviceInfoMode cmd)
        {
            string strJson = JsonConvert.SerializeObject(cmd);
            LogHelper.WriteLog("木牛流马AGV调用WCS接口", "NotifyDeviceInfo:" + strJson, EnumLogLevel.Info);

            IMNLMChaAgvApi xChaApi = DependencyHelper.GetService<IMNLMChaAgvApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (xChaApi != null)
            {
                OperateResult result = xChaApi.NotifyDeviceInfo(cmd);
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
