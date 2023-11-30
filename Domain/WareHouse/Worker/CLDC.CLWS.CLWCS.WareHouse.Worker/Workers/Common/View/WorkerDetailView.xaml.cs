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
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View
{
    /// <summary>
    /// WorkerDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class WorkerDetailView
    {
        private WorkerDetailViewModel viewModel;
        public WorkerDetailView(WorkerBaseAbstract worker)
        {
            InitializeComponent();
            viewModel = new WorkerDetailViewModel(worker);
            this.DataContext = viewModel;
            this.MouseLeftButtonDown += delegate { DragMove(); }; //拖拽
            CtrlTitle.SetOnCloseRoutedEventHandler(OnClose);
            InitViewSize();
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InitViewSize()
        {
            this.Width = SystemParameters.WorkArea.Width*0.9;
            this.Height = SystemParameters.WorkArea.Height*0.9;
        }
    }
}
