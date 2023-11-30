using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.Common
{
    /// <summary>
    /// 协调调度工作者类
    /// </summary>
    public abstract class DispatchWorkerAbstract<TWorkerBusiness> : OrderWorkerAbstract<TWorkerBusiness> where TWorkerBusiness : OrderWorkerBuinessAbstract
    {

    }
}
