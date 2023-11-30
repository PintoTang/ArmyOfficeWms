using System.Windows;
using System.Windows.Controls;
using CLDC.CLWCS.Service.DataService;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.OperateLog.View;
using CLDC.CLWS.CLWCS.Service.OperateLog.ViewModel;

namespace CLDC.CLWS.CLWCS.Service.OperateLog
{
    public sealed class OperateLogManage : WcsDataServiceAbstract
    {

        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult InitilizeConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override Window GetAssistantView()
        {
            return new Window();
        }

        public override UserControl GetDetailView()
        {
            OperateLogDataView operateLogView = new OperateLogDataView();
            operateLogView.DataContext = new OperateLogDataViewModel(Name);
            return operateLogView;
        }
    }
}
