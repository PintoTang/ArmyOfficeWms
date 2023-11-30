using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View
{
    /// <summary>
    /// ShowCard.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceShowCard:IShowCard
    {
        public DeviceShowCard()
        {
            InitializeComponent();
        }
        public void ChangeViewState(ShowCardShowEnum state)
        {
            switch (state)
            {
                case ShowCardShowEnum.Task:
                    ShowTask();
                    break;
                case ShowCardShowEnum.State:
                    ShowState();
                    break;
                case ShowCardShowEnum.Log:
                    ShowLog();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state", state, null);
            }
        }

        private void ShowTask()
        {
            if (this.TabItemTask.Visibility == Visibility.Visible)
            {
                this.TabItemWorkerState.IsSelected = false;
                this.TabItemLog.IsSelected = false;
                this.TabItemTask.IsSelected = true;
            }
        }

        private void ShowLog()
        {
            if (this.TabItemLog.Visibility == Visibility.Visible)
            {
                this.TabItemWorkerState.IsSelected = false;
                this.TabItemLog.IsSelected = true;
                this.TabItemTask.IsSelected = false;
            }
        }

        private void ShowState()
        {
            if (this.TabItemWorkerState.Visibility == Visibility.Visible)
            {
                this.TabItemWorkerState.IsSelected = true;
                this.TabItemLog.IsSelected = false;
                this.TabItemTask.IsSelected = false;
            }
        }
    }
}
