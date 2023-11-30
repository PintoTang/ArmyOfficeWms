using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordinationConfigAddrPrefixsPrefixs : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Prefixs";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            List<Prefixs> prefixsLst = new List<Prefixs>();
            foreach (var nvc in nodeValueslst)
            {
                Prefixs prefixs = new Prefixs();
                string strType = nvc["Type"];
                prefixs.Type = strType;

                context.SetXmlNode(strNodeParms + "[@Type='" + strType + "']");

                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new CoordinationConfigAddrPrefixsPrefixsItem());
                context.Request();

                if (context.CurNextNodeInfo != null)
                {
                    prefixs.Address = context.CurNextNodeInfo.ToString();
                }
                prefixsLst.Add(prefixs);
            }
            context.CurNextNodeInfo = prefixsLst;
          
        }
    }
}
