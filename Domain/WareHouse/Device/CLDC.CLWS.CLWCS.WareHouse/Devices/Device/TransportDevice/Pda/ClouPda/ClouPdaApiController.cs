using System.Web.Http;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Pda.ClouPda
{
    public class ClouPdaApiController : ApiController,IClouPdaApi
    {
       [HttpPost]
        public SyncResReErr NotifyInstructFinish(string cmd)
       {
           IClouPdaApi clouPdaApi = DependencyHelper.GetService<IClouPdaApi>();
           return clouPdaApi.NotifyInstructFinish(cmd);
       }
    }
}
