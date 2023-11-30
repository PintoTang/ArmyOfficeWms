using System.Collections.Generic;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.ViewModel
{
    public class IdentityWorkerViewModel : WorkerViewModelAbstract<InCheckWorkerAbstractForT<List<string>>>
    {
        public IdentityWorkerViewModel(InCheckWorkerAbstractForT<List<string>> worker)
            : base(worker)
        {
          
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {

        }
    }
}
