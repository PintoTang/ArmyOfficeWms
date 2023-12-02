using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DeviceMonitor;
using CLDC.Service.Project;
using CLWCS.WareHouse.ArchitectureData.HeFei;
using CLWCS.WareHouse.DeviceMonitor.HeFei;
using Microsoft.Practices.Unity;

namespace CLWCS.HeFei.Project
{
    public class ProjectMain : ProjectAbstract
    {
        public override OperateResult ParticularRestore()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularRegisterService()
        {
            DependencyResolver.RegisterType<IWmsWcsArchitecture, ArchitectureDataForHeFei>(new ContainerControlledLifetimeManager());
            DependencyResolver.RegisterType<DeviceMonitorAbstract, DeviceMonitorForHeFei>(new ContainerControlledLifetimeManager());
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitilizeConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        
    }
}
