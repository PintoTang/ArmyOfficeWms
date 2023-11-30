using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CLDC.CLWCS.Service.MenuService.View;
using CLDC.CLWCS.WareHouse.Architecture.Manage;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;

namespace CLDC.CLWCS.Service.MenuService
{
    public class AuthServiceMenu : WcsMenuAbstract
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
            return new AuthServiceView();
        }
    }
}
