using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel
{
    public class HotTransportManageViewModel
    {
        private readonly ITransportManage _transportManage;
        public HotTransportManageViewModel()
        {
            _transportManage = DependencyHelper.GetService<ITransportManage>();
            InitTransportMessageViewModel();
        }
        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }
        private void InitTransportMessageViewModel()
        {
            TransportMessageViewModel = new TransportMessageViewModel();
            TransportMessageViewModel.UnfinishedTransportList = _transportManage.ManagedDataPool.DataPool;
        }

        public TransportMessageViewModel TransportMessageViewModel { get; private set; }

    }
}
