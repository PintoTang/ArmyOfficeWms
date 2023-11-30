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
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.UpperServiceForHeFei.Interface.ViewModel;

namespace CLWCS.UpperServiceForHeFei.Interface.View
{
    /// <summary>
    /// NotifyUnstackFinishCmdView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyUnstackFinishCmdView : UserControl
    {
        public NotifyUnstackFinishCmdView(NotifyUnstackFinishCmdMode cmd)
        {
            InitializeComponent();
            this.DataContext = new NotifyUnstackFinishCmdViewModel(cmd);
        }
    }
}
