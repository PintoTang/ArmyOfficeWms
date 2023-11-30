using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordinationConfigEquipment : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Device";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);
            List<DeviceData> equipmentLst = new List<DeviceData>();
            foreach (var nvc in nodeValueslst)
            {
                DeviceData equipment = new DeviceData();
                equipment.DeviceId =int.Parse( nvc["DeviceId"]);
                equipmentLst.Add(equipment);
            }
            context.CurNextNodeInfo = equipmentLst;
        }
    }
}