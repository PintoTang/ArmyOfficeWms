using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel
{
    public class NoPrefixsWorkerConfigViewModel : NotifyObject
    {
        public NoPrefixsWorkerConfigProperty DataModel { get; set; }
        public NoPrefixsWorkerConfigViewModel(NoPrefixsWorkerConfigProperty model)
        {
            DataModel = model;
            AssistantViewModel=new AssistantDeviceViewModel(model.Devices);
        }
        public AssistantDeviceViewModel AssistantViewModel { get; set; }      
    }
}
