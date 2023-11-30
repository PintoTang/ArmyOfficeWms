using System;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View
{
    /// <summary>
    /// ShowCard.xaml 的交互逻辑
    /// </summary>
    public partial class WorkerShowCard : IShowCard
    {
        public WorkerShowCard()
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
