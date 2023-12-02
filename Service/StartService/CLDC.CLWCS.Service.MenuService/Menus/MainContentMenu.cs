using CLDC.CLWCS.Service.MenuService.View;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System.Windows.Controls;

namespace CLDC.CLWCS.Service.MenuService
{
    public class MainContentMenu : WcsMenuAbstract
    {
        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult InitilizeConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetDetailView()
        {
            return new MainContentView();
        }

    }
}
