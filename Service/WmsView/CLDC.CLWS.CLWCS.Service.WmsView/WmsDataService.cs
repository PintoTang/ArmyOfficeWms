using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    public class WmsDataService
    {
        private readonly WmsDataAbstract _wmsDataAccess;
        public WmsDataService(WmsDataAbstract wmsDataAccess)
        {
            _wmsDataAccess = wmsDataAccess;
        }

        public List<Material> GetMaterialList(string name)
        {
            return _wmsDataAccess.GetMaterialList(name);
        }

    }
}
