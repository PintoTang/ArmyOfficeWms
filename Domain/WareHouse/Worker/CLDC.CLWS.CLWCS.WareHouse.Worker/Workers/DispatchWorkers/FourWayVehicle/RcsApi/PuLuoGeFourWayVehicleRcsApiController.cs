using System.Web.Http;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.RcsApi
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

    public class PLGFourWayVehicleRcsApiController : ApiController
    {
        [HttpPost]
        public string ReportTaskResult(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.ReportTaskResult(cmd);
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
        public string RequestWCSPermit(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.RequestWCSPermit(cmd);
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
        public string ReportWCSPermitFinish(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.ReportWCSPermitFinish(cmd);
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
        public string ReportTaskException(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.ReportTaskException(cmd);
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
        public string ReportAGVStatus(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.ReportAGVStatus(cmd);
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
        public string ReportAGVPara(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.ReportAGVPara(cmd);
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
        public string ReportTrackInfo(string cmd)
        {
            IPuLuoGeFourWayVehicleRcsApi fourWayVehicleApi = DependencyHelper.GetService<IPuLuoGeFourWayVehicleRcsApi>();
            ResponseResult respones = new ResponseResult(1, "失败");
            if (fourWayVehicleApi != null)
            {
                OperateResult result = fourWayVehicleApi.ReportTrackInfo(cmd);
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
