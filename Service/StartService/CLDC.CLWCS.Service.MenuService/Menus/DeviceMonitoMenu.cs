using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DeviceMonitor;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWCS.Service.MenuService
{
    public class DeviceMonitoMenu : WcsMenuAbstract
    {
        private DeviceMonitorAbstract _deviceMonitorAbstract;
        protected override OperateResult ParticularInitlize()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                _deviceMonitorAbstract = DependencyHelper.GetService<DeviceMonitorAbstract>();
               OperateResult initializeResult= _deviceMonitorAbstract.Initialize();
               if (!initializeResult.IsSuccess)
               {
                   return initializeResult;
               }
                result.IsSuccess = true;
                result.Message = "初始化成功";
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;

        }

        protected override OperateResult InitilizeConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetDetailView()
        {
            if (_deviceMonitorAbstract!=null)
            {
                return _deviceMonitorAbstract.MonitorProjectView;
            }
            return new UserControl();
        }
    }
}
