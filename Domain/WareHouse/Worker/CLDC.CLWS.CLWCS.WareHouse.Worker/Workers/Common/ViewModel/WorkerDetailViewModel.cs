using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel
{
    public class WorkerDetailViewModel
    {
        public WorkerDetailViewModel(WorkerBaseAbstract worker)
        {
            Worker = worker;
            DetailView = worker.GetDetailViewForWpf();
            MonitorView = worker.GetMonitorViewForWpf();
        }
        public WorkerBaseAbstract Worker { get; set; }
        public UserControl DetailView { get; set; }
        public UserControl MonitorView { get; set; }

        private MyCommand _openLogCommand;
        /// <summary>
        /// 打开当前日志
        /// </summary>
        public MyCommand OpenLogCommand
        {
            get
            {
                if (_openLogCommand == null)
                    _openLogCommand = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              LogManageHelper.Instance.OpenLogFile(Worker.Name);
                          }
                        ));
                return _openLogCommand;
            }
        }
    }
}
