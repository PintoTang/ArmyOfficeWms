using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordinationConfigEquipments : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Devices";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            DeviceDatas equipments = new DeviceDatas();
            foreach (var nvc in nodeValueslst)
            {

                context.SetXmlNode(strNodeParms);
                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new CoordinationConfigEquipment());
                context.Request();

                if (context.CurNextNodeInfo != null)
                {

                    equipments.DeviceDataLst = (List<DeviceData>)context.CurNextNodeInfo;
                }

            }
            context.CurNextNodeInfo = equipments;
        }
    }
}
