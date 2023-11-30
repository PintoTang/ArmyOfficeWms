using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.ViewModel;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common
{
    public class ServiceApiViewModel : ViewModelBase
    {
        public ServiceApiViewModel(string serviceName)
        {
            DataViewModel = new ReceiveDataViewModel(serviceName);
        }
        public ReceiveDataViewModel DataViewModel { get; set; }

    }
}
