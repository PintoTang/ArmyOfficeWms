using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class PTSConfig : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/Config");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                string strType = nvc["Type"];

                //context.SetState(new PTSConfigDBItems());
                //context.SetXmlNode(context.strNodePath + "[@Type='" + strType + "']");
                //context.Request();

                var deviceProtocolConfig = new DeviceProtocolConfig
                {
                   ConfigType= strType,
                };

                context.CurNextNodeInfo = deviceProtocolConfig;
            }
        }
    }
}
