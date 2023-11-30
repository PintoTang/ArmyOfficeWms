using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CommConfigDBItems : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/DataBlockItems");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                string strTemplate = nvc["Template"].Trim();

                var deviceDataBlockItems = new DeviceConfigDataBlockItems
                {
                    Template = strTemplate
                };
                
                context.SetState(new CommConfigDBItemsItem());
                context.SetXmlNode(context.strNodePath + "[@Template='" + strTemplate + "']");
                context.Request();

               
                if (context.CurNextNodeInfo != null)
                {
                    deviceDataBlockItems.DeviceDataBlockItemsLst = (DeviceDataBlockItems)context.CurNextNodeInfo;
                }

                context.CurNextNodeInfo = deviceDataBlockItems;
            }
        }
    }
}
