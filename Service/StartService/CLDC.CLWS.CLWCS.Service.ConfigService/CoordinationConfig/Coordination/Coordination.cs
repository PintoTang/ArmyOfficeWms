using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class Coordination : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Coordination"; 
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            List<Coords> coordsLst = new List<Coords>();
            foreach (var nvc in nodeValueslst)
            {
                string strType = nvc["Type"];
                string strId = nvc["Id"];
                string strName = nvc["Name"];
                string strDeviceName = nvc["WorkerName"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];
                string workerSize = nvc["WorkSize"];
                Coords coords = new Coords
                {
                    Id = strId,
                    Name = strName,
                    Type = strType,
                    DeviceName = strDeviceName,
                    Class = strClass,
                    NameSpace = strNameSpace,
                    WorkSize=int.Parse(workerSize)
                };
                context.GlobalNameSpace = strNameSpace;
                context.SetXmlNode(strNodeParms + "[@Id='" + strId + "']");


                context.SetState(new CoordinationConfig());
                context.Request();
                if (context.CurNextNodeInfo != null)
                {
                    coords.CoordConfigInfo = (CoordConfigInfo)context.CurNextNodeInfo;
                }


                context.SetXmlNode(strNodeParms + "[@Id='" + strId + "']");
                context.SetState(new BusinessHandle());
                context.Request();
                if (context.CurNextNodeInfo != null)
                {
                    coords.CoordBusinessHandle = (CoordBusinessHandle)context.CurNextNodeInfo;
                }
                coordsLst.Add(coords);
            }
            context.CurNextNodeInfo = coordsLst;

           
        }
    }
}
