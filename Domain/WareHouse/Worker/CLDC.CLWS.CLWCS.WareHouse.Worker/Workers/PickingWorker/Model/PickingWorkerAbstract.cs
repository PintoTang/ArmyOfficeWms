using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PickingWorker.Model
{
    public abstract class PickingWorkerAbstract<TWorkerBusiness> : WorkerBaseAbstract where TWorkerBusiness : PickingWorkerBusinessAbstract
    {
        protected TWorkerBusiness WorkerBusiness;
        protected abstract OperateResult SwithValueChangeHandle(object sender,int newValue);

        public override OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business)
        {
            WorkerBusiness = business as TWorkerBusiness;
            if (WorkerBusiness == null)
            {
                string msg = string.Format("协助者类转换出错，期望类型：PickingWorkerBusinessAbstract 实际类型：{0}", business.GetType());
                return OperateResult.CreateFailedResult(msg, 1);
            }
            return OperateResult.CreateSuccessResult();
        }
    }
}
