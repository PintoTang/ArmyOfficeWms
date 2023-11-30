using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class BusHandleConfigParms : StateAbstract
    {
        public override void handle(Context context)
        {
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath + "/Config");
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                DeviceBugConfig deviceConfig= new DeviceBugConfig { ConfigType = nvc["Type"] };//目前结构 只考虑一条


               

                //var GetNodeIsCheckReadyObj = context.xml.GetNodeValuesList(context.strNodePath + "/Config/IsCheckReady");
                //foreach (var nvcIsCheckReady in GetNodeIsCheckReadyObj)
                //{
                //    deviceConfig.IsCheckReady = (nvcIsCheckReady["IsCheckReady"].ToUpper()=="TRUE");
                //}

                //var GetNodeIsNeedTransportToNextAddrObj = context.xml.GetNodeValuesList(context.strNodePath + "/Config/IsNeedTransportToNextAddr");
                //foreach (var nvcIsCheckReady in GetNodeIsCheckReadyObj)
                //{
                //    deviceConfig.IsCheckReady = (nvcIsCheckReady["IsCheckReady"].ToUpper() == "TRUE");
                //}

                context.CurNextNodeInfo = deviceConfig;
            }
        }
    }
}
