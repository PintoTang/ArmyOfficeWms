using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class PTS : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/ProtocolTranslation");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                //string strName = nvc["Name"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];
                string strType = nvc["Type"];

                var deviceProtocolTranslation = new DeviceProtocolTranslation
                {
                    //Name = strName,
                    Class = strClass,
                    NameSpace = strNameSpace,
                    Type = strType
                };
                if (string.IsNullOrEmpty(strNameSpace))
                {
                    deviceProtocolTranslation.NameSpace = context.CurNameSpace;
                }
                else
                {
                    context.CurNameSpace = strNameSpace;
                }
                //不用读取Config 待删除 by zhangxing
                //context.SetState(new PTSConfig());
                //context.SetXmlNode(context.strNodePath + "[@Type='" + strType + "']");
                //context.Request();
                //if (context.CurNextNodeInfo != null)
                //{
                //    deviceProtocolTranslation.DeviceProtocolConfig = (DeviceProtocolConfig)context.CurNextNodeInfo;
                //}

                context.CurNextNodeInfo = deviceProtocolTranslation;
            }
        }
    }
}
