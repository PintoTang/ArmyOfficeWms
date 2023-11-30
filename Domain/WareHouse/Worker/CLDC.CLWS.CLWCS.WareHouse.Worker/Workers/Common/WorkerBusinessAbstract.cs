using System;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common
{
   public abstract   class WorkerBusinessAbstract
   {
       
        public int WorkerId { get; set; }

        public string Name { get; set; }

        public string NameSpace { get; set; }

        public string ClassName { get; set; }

        public DeviceName WorkerName { get; set; }
        public Action<string, EnumLogLevel, bool> LogMessageAction { get; set; }
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            if (LogMessageAction != null)
            {
                LogMessageAction(msg, level, isNotifyUi);
            }
        }
        internal OperateResult Initialize(int workerId, DeviceName workerName)
        {
            this.WorkerId = workerId;
            this.WorkerName = workerName;

            #region 必须是初始化配置再初始化业务
            OperateResult initConfigResult = InitConfig();
            if (!initConfigResult.IsSuccess)
            {
                return initConfigResult;
            }
            #endregion

            #region 必须是初始化配置再初始化业务
            OperateResult initParticularResult = ParticularInitlize();
            if (!initParticularResult.IsSuccess)
            {
                return initParticularResult;
            }
            #endregion

            return OperateResult.CreateSuccessResult();
        }

        protected abstract OperateResult ParticularInitlize();
       

        public  OperateResult InitConfig()
        {
            return ParticularConfig();
        }
        protected abstract OperateResult ParticularConfig();

    }
}
