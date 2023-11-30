using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel
{
   public class OrderWorkerConfigViewModel
    {
       public OrderWorkerConfigProperty DataModel { get; set; }
       public OrderWorkerConfigViewModel(OrderWorkerConfigProperty model)
       {
           DataModel = model;
           AssistantViewModel = new AssistantDeviceViewModel(model.Devices);
           AddrPrefixsViewModel=new AddrPrefixsViewModel(model.AddrPrefixs);
       }
       public AssistantDeviceViewModel AssistantViewModel { get; set; }

       public AddrPrefixsViewModel AddrPrefixsViewModel { get; set; }

    }
}
