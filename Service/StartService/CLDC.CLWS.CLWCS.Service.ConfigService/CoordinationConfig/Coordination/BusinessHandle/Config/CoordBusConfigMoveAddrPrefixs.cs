using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordBusConfigMoveAddrPrefixs : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/MoveAddrPrefixs";
            var nodeValueslst = context.xml.GetNodeInnerTextDataList(strNodeParms);

            string strAddress = "";
            foreach (var nvc in nodeValueslst)
            {
                strAddress = nvc["MoveAddrPrefixs"].Trim().ToString();
            }
            context.CurNextNodeInfo = strAddress;
        }
    }
}
