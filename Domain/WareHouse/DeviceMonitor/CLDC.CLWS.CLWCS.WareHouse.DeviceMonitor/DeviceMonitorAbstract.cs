using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.WareHouse.DeviceMonitor
{
    /// <summary>
    /// 设备监控虚拟基类
    /// </summary>
    public abstract class DeviceMonitorAbstract
    {
        public abstract OperateResult ParticularInitlize();

        public abstract OperateResult ParticularConfig();


        public DeviceMonitorView MonitorProjectView
        {
            get
            {
                return monitorProjectView;
            }
        }

        protected DeviceMonitorView monitorProjectView;

        private OperateResult InitConfig()
        {
            OperateResult particularConfig = ParticularConfig();
            if (!particularConfig.IsSuccess)
            {
                return particularConfig;
            }
            return OperateResult.CreateSuccessResult();
        }
        public OperateResult Initialize()
        {

            OperateResult particularResult = ParticularInitlize();
            if (!particularResult.IsSuccess)
            {
                return particularResult;
            }
            OperateResult initConfig = InitConfig();
            if (!initConfig.IsSuccess)
            {
                return initConfig;
            }
            return OperateResult.CreateSuccessResult();
        }

    }
}
