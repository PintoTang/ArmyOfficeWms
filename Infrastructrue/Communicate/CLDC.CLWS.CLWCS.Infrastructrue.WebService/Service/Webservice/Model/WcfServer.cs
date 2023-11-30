using System.ServiceModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Webservice.Model
{
    public abstract class WcfServer<T> : ServiceHost
    {
        protected WcfServer()
            : base(typeof(T))
        {

        }
        protected abstract OperateResult ParticularInitConfig();
        protected abstract OperateResult ParticularInitlize();

        public OperateResult Initailize()
        {
            OperateResult particularInitlize = ParticularInitlize();
            if (!particularInitlize.IsSuccess)
            {
                return particularInitlize;
            }

            OperateResult initConfig = InitConfig();
            if (!initConfig.IsSuccess)
            {
                return initConfig;
            }
            return OperateResult.CreateSuccessResult();

        }

        private OperateResult InitConfig()
        {
            return ParticularInitConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract OperateResult StartService(string uri, string type);

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult StopService();
    }
}
