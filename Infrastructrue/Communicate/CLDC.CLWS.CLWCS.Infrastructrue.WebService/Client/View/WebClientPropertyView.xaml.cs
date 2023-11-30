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
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.ViewModel;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.View
{
    /// <summary>
    /// WebClientPropertyView.xaml 的交互逻辑
    /// </summary>
    public partial class WebClientPropertyView : UserControl
    {
        public WebClientPropertyView(WebClientCommunicationProperty dataModel)
        {
            InitializeComponent();
            this.DataContext =new WebClientPropertyViewModel(dataModel);
        }
    }
}
