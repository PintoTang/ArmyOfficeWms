using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.ReadWriteForConfigPckg
{
    public class CommConfigDataBlockItems:State
    {
        public override void handle(Context context)
        {
         

            context.SetXmlNode(context.strNodePath + "/Connection");
            var nodeValueslst = context.xml.GetNodeInnerTextDataList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                context.CurNextNodeInfo = new DeviceCommConfigConn { Connection = nvc["Connection"] };
            }
        }
    }
}
