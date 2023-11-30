

using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class DeviceHelper
    {
        /// <summary>
        /// 加载数据对象 添加到 DeviceManage
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="strNodeName">节点名称</param>
        public static void Handle(Context context, string strNodeName)
        {
            string strNodeParms = context.strNodePath + strNodeName;
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);//只能有一条数据
            foreach (var nvc in nodeValueslst)
            {
                DeviceInfoConfig rolDeviceInfo = new DeviceInfoConfig();

                string deviceType = nvc["DeviceType"];
                string nameSpace = nvc["NameSpace"];
                string name = nvc["Name"];
                rolDeviceInfo.DeviceType = deviceType;
                rolDeviceInfo.DeviceNameSpace = nameSpace;
                rolDeviceInfo.Name = name;

                context.GlobalNameSpace = nameSpace;

                context.SetState(new Device());
                string curNodeName = strNodeParms + "[@DeviceType='" + deviceType + "'][@NameSpace='" + nameSpace + "']";
                context.SetXmlNode(curNodeName);
                context.Request();

                if (context.CurNextNodeInfo != null)
                {
                    List<Devices> deviceLst = (List<Devices>)context.CurNextNodeInfo;
                    rolDeviceInfo.DeviceLst = deviceLst;
                }
                DeviceInfoConfigManage.Instance.AddDevice(rolDeviceInfo);
            }
        }
    }
}
