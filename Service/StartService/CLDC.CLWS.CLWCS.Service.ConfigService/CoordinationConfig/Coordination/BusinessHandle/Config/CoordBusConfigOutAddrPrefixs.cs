using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordBusConfigOutAddrPrefixs : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/OutAddrPrefixs";
            var nodeValueslst = context.xml.GetNodeInnerTextDataList(strNodeParms);

            string strAddress = "";
            foreach (var nvc in nodeValueslst)
            {
                strAddress = nvc["OutAddrPrefixs"].Trim().ToString();
            }
            context.CurNextNodeInfo = strAddress;
        }
    }
}
