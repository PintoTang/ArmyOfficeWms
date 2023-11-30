using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.ServiceWorkers.Model
{
    public abstract class ServiceWorkerAbstract : WorkerBaseAbstract
    {
        public ServiceWorkerBusinessAbstract WorkerBusiness { get; set; }
        public override OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business)
        {
            WorkerBusiness = business as ServiceWorkerBusinessAbstract;
            this.Id = id;
            if (WorkerBusiness == null)
            {
                return OperateResult.CreateFailedResult(string.Format("工作者业务类型转换出错，期望类型：ServiceWorkerBusinessAbstract 传入类型：{0}", business.GetType().FullName), 1);
            }
            return OperateResult.CreateSuccessResult();
        }
        public override OperateResult GetWorkerRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetWorkerRealData()
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
