using System.Windows.Controls;
using CL.WCS.SystemConfigPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWCS.Service.MenuService
{
    public class SystemConfigMenu : WcsMenuAbstract
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
            return new SystemConfigView();
        }
    }
}
