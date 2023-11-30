using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class ControlHandle : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/ControlHandle");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            DeviceControlHandle deviceControlHandle = new DeviceControlHandle();
            foreach (var nvc in nodeValueslst)
            {
                string strName = nvc["Name"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];
                string strType = nvc["Type"];


                deviceControlHandle.Name = strName;
                deviceControlHandle.Class = strClass;
                deviceControlHandle.NameSpace = strNameSpace;
                deviceControlHandle.Type = strType;

                if (string.IsNullOrEmpty(strNameSpace))
                {
                    deviceControlHandle.NameSpace = context.CurNameSpace;
                }
                else
                {
                    context.CurNameSpace = strNameSpace;
                }


                context.SetState(new PTS());
                context.Request();
                context.ReMoveStartNodePath("/ProtocolTranslation");
                if (context.CurNextNodeInfo != null)
                {
                    deviceControlHandle.ProtocolTranslation = (DeviceProtocolTranslation)context.CurNextNodeInfo;
                }

                context.SetState(new Comm());
                context.Request();
                if (context.CurNextNodeInfo != null)
                {
                    deviceControlHandle.Communication = (DeviceCommunication)context.CurNextNodeInfo;
                }
            }
            context.CurNextNodeInfo = deviceControlHandle;

           
        }
    }
}
