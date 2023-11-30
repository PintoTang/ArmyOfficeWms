using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class Comm : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/Communication");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                string strName = nvc["Name"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];
                string strType = nvc["Type"];

                var deviceCommunication = new DeviceCommunication
                {
                    Name = strName,
                    Class = strClass,
                    NameSpace = strNameSpace,
                    Type = strType
                };
                if (string.IsNullOrEmpty(strNameSpace))
                {
                    deviceCommunication.NameSpace = context.CurNameSpace;
                }
                else
                {
                    context.CurNameSpace = strNameSpace;
                }
                //不用读取Config 待删除 by zhangxing
                //context.SetState(new CommConfig());
                //context.SetXmlNode(context.strNodePath + "[@Name='" + strName + "']");
                //context.Request();

                //if (context.CurNextNodeInfo != null)
                //{
                //    deviceCommunication.DeviceCommConfig = (DeviceCommConfig)context.CurNextNodeInfo;
                //}
             
                context.CurNextNodeInfo = deviceCommunication;
            }
        }
    }
}
