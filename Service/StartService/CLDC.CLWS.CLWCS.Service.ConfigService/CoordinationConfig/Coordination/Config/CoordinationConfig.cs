using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordinationConfig : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Config";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            CoordConfigInfo coordConfigInfo = new CoordConfigInfo();
            foreach (var nvc in nodeValueslst)
            {


                context.SetXmlNode(strNodeParms);

                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new CoordinationConfigAddrPrefixs());
                context.Request();

                if (context.CurNextNodeInfo != null)
                {
                    coordConfigInfo.AddrPrefixs = (AddrPrefixs)context.CurNextNodeInfo;
                }



                ///******************todo
                context.SetXmlNode(strNodeParms);

                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new CoordinationConfigEquipments());
                context.Request();

                if (context.CurNextNodeInfo != null)
                {
                    coordConfigInfo.Devices = (DeviceDatas)context.CurNextNodeInfo;
                }




            }
            context.CurNextNodeInfo = coordConfigInfo;
        }
    }
}
