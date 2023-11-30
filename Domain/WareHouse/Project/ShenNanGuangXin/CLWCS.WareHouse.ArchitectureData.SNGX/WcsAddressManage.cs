using System.Windows;
using System.Windows.Controls;
using CLDC.CLWCS.WareHouse.Architectrue.ViewModel;
using CLDC.CLWCS.WareHouse.Architectrue.View;
using CLDC.CLWCS.WareHouse.Architecture.Manage;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLWCS.WareHouse.ArchitectureData.HeFei
{
    public sealed class WcsAddressManage : ArchitectureDataAbstract
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
            WcsAddressView apiServiceView = new WcsAddressView();
            apiServiceView.DataContext = new WcsAddressViewModel(Name);
            return apiServiceView;
        }
    }
}
