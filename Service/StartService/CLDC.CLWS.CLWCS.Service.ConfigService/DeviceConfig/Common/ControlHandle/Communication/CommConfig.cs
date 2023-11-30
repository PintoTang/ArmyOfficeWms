using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CommConfig : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/Config");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
               
                string strType = nvc["Type"];

                context.SetState(new CommConfigConn());
                context.SetXmlNode(context.strNodePath + "[@Type='" + strType + "']");
                context.Request();
                context.ReMoveStartNodePath("/Connection");

                var deviceCommConfig = new DeviceCommConfig { ConfigType = strType };
               
                if (context.CurNextNodeInfo != null)
                {
                    deviceCommConfig.DeviceCommConfigConn = (DeviceCommConfigConn)context.CurNextNodeInfo;
                }

                context.SetState(new CommConfigDBItems());
                context.SetXmlNode(context.strNodePath + "[@Type='" + strType + "']");
                context.Request();

                if(context.CurNextNodeInfo!=null)
                {

                    deviceCommConfig.DeviceConfigDataBlockItems = (DeviceConfigDataBlockItems)context.CurNextNodeInfo;
                }



                context.CurNextNodeInfo = deviceCommConfig;
            }
        }
    }
}
