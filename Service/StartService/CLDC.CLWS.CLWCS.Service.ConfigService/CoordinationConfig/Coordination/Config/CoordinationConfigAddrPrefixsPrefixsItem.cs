using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordinationConfigAddrPrefixsPrefixsItem : StateAbstract
    {

        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath;
            var nodeValueslst = context.xml.GetNodeInnerTextDataList(strNodeParms);//只有一条数据
            foreach (var nvc in nodeValueslst)
            {
                string strAddress = nvc[0].ToString().Trim();

                context.CurNextNodeInfo = strAddress;
            }
        }
    }
}
