using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Service.ConfigService;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class BusinessHandleConfig : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Config";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            CoordBusinessHandleConfig coorConfig = new CoordBusinessHandleConfig();
            foreach (var nvc in nodeValueslst)
            {
                string strCurrAddress = nvc["CurrAddress"];
                string strDestAddress = nvc["DestAddress"];
                string strType = nvc["Type"];

                coorConfig.CurrAddress = strCurrAddress;
                coorConfig.DestAddress = strDestAddress;
                coorConfig.Type = strType;



               
            }
            context.CurNextNodeInfo = coorConfig;
        }
    }
}
