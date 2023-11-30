using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DeviceMonitor;
using CLWCS.WareHouse.DeviceMonitor.HeFei.MonitorView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CLWCS.WareHouse.DeviceMonitor.HeFei
{
   public class DeviceMonitorForHeFei : DeviceMonitorAbstract
    {
        public override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitlize()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    monitorProjectView = new DeviceMonitorView();

                    DeviceMonitorViewDetailForHeFei userCtrl = new DeviceMonitorViewDetailForHeFei();
                    monitorProjectView.AddMonitorDetail(userCtrl.UseCtrlId, userCtrl);

                    DeviceMonitorViewDetail2ForHeFei userCtrl2 = new DeviceMonitorViewDetail2ForHeFei();
                    monitorProjectView.AddMonitorDetail(userCtrl2.UseCtrlId, userCtrl2);

                    monitorProjectView.InitializeMonitorDetail(userCtrl.UseCtrlId);
                    monitorProjectView.SetMonitorViewBoxSize(userCtrl.Height, userCtrl.Width);
                });
            return OperateResult.CreateSuccessResult();
          
        }




    }
}
