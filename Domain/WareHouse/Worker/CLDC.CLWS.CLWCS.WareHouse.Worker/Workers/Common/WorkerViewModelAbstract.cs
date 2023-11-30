using System;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common
{
    public abstract class WorkerViewModelAbstract<TWorker> : WareHouseViewModelBase, IDoTask where TWorker : WorkerBaseAbstract
    {

        protected virtual void InitViewPanel()
        {
            IsShowCurrentStatusZone = Visibility.Visible;
            IsShowControlZone = Visibility.Visible;
            IsShowStatusZone = Visibility.Visible;
            IsShowTransportZone = Visibility.Visible;
            if (Worker.WorkerType == WorkerTypeEnum.IdentityWorker)
            {
                IsShowStatusZone = Visibility.Collapsed;
                IsShowTransportZone = Visibility.Collapsed;
                IsShowControlZone= Visibility.Collapsed;
            }
            if (Worker.WorkerType == WorkerTypeEnum.SwitchingWorker)
            {
                IsShowStatusZone = Visibility.Collapsed;
                IsShowTransportZone = Visibility.Collapsed;
                IsShowControlZone = Visibility.Collapsed;
            }
            if (Worker.WorkerType == WorkerTypeEnum.PickingWorker)
            {
                IsShowStatusZone = Visibility.Collapsed;
                IsShowTransportZone = Visibility.Collapsed;
                IsShowControlZone = Visibility.Collapsed;
            }
            if (Worker.WorkerType == WorkerTypeEnum.ServiceWorker)
            {
                IsShowStatusZone = Visibility.Collapsed;
                IsShowTransportZone = Visibility.Collapsed;
                IsShowControlZone = Visibility.Collapsed;
            }
            if (Worker.WorkerType == WorkerTypeEnum.PalletierWorker)
            {
                IsShowStatusZone = Visibility.Collapsed;
                IsShowTransportZone = Visibility.Collapsed;
                IsShowControlZone = Visibility.Collapsed;
            }
        }
        public Visibility IsShowCurrentStatusZone { get; set; }
        public Visibility IsShowTransportZone { get; set; }
        public Visibility IsShowControlZone { get; set; }

        public Visibility IsShowStatusZone { get; set; }
        public TWorker Worker { get; set; }
        public WorkerViewModelAbstract(TWorker worker)
        {
            this.Worker = worker;
            Id = worker.Id;
            Name = worker.Name;
            GroupName = worker.WorkerType.ToString();
            Worker.NotifyMsgToUiEvent += ShowLogMessage;
            Worker.RegisterAttributeListener(this);
            InitilizeViewPanel();
        }
        private void InitilizeViewPanel()
        {
            InitViewPanel();
        }

        private MyCommand _openConfigCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand OpenConfigCommand
        {
            get
            {
                if (_openConfigCommand == null)
                    _openConfigCommand = new MyCommand(OpenConfigView);
                return _openConfigCommand;
            }
        }

        private void OpenConfigView(object obj)
        {
            Window configView = Worker.GetConfigView();
            if (configView == null)
            {
                return;
            }
            configView.ShowDialog();
        }



        private MyCommand _showDetailCommad;
        /// <summary>
        /// 设备开始\暂停
        /// </summary>
        public MyCommand ShowDetailCommad
        {
            get
            {
                if (_showDetailCommad == null)
                    _showDetailCommad = new MyCommand(ShowWorkerDetail);
                return _showDetailCommad;
            }
        }

        private void ShowWorkerDetail(object arg)
        {
            if (Worker == null)
            {
                return;
            }
            WorkerDetailView detailView = new WorkerDetailView(Worker);
            detailView.ShowDialog();
        }

        public bool IsHasTask()
        {
            return Worker.IsHasTask;
        }

        public bool IsHasError()
        {
            return Worker.IsHasError;
        }
    }
}
