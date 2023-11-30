using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordinationConfigAddrPrefixs : StateAbstract
    {

        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/AddrPrefixs";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            AddrPrefixs addrPrefixs = new AddrPrefixs();
            foreach (var nvc in nodeValueslst)
            {

                context.SetXmlNode(strNodeParms);
                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new CoordinationConfigAddrPrefixsPrefixs());
                context.Request();

                if (context.CurNextNodeInfo != null)
                {
                    addrPrefixs.PrefixsLst = (List<Prefixs>)context.CurNextNodeInfo;
                }
                
            }
            context.CurNextNodeInfo = addrPrefixs;
        }
    }
}