using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class BusHandle : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/BusinessHandle");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            foreach (var nvc in nodeValueslst)
            {
                string strName = nvc["Name"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];
                string strType = nvc["Type"];

                var deviceBusHandle = new DeviceBusinessHandle
                {
                    Name = strName,
                    Class = strClass,
                    NameSpace = strNameSpace,
                    Type = strType
                };
                //如果当前节点没有命名空间，则默认取上一级的命名空间
                if (string.IsNullOrEmpty(strNameSpace))
                {
                    deviceBusHandle.NameSpace = context.CurNameSpace;
                }
                else
                {
                    context.CurNameSpace = strNameSpace;
                }

                //不用读取config 待删除 by zhangxing
                //context.SetState(new BusHandleConfig());
                //context.SetXmlNode(context.strNodePath + "[@Name='" + strName + "']");
                //context.Request();

                //if (context.CurNextNodeInfo != null)
                //{
                //    deviceBusHandle.DeviceConfig = (DeviceConfig)context.CurNextNodeInfo;
                //}


                context.CurNextNodeInfo = deviceBusHandle;
            }
        }
    }
}