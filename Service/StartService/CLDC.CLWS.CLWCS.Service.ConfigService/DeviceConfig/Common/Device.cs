using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class Device : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/Device");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            List<Devices> tempDeviceLst = new List<Devices>();
            context.CurNextNodeInfo = null;

            foreach (var nvc in nodeValueslst)
            {
                string strDeviceID = nvc["DeviceId"].Trim();
                string strName = nvc["Name"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];
                string strDeviceName = nvc["DeviceName"];
                string strCurAddress = nvc["CurAddress"];
                bool strIsShowUi = true;
                if (nvc["IsShowUi"] != null)
                {
                    Boolean.TryParse(nvc["IsShowUi"], out strIsShowUi);
                }
                int workSize = 0;
                if (nvc["WorkSize"] != null)
                {
                    workSize = int.Parse(nvc["WorkSize"].ToString().Trim());
                }

                Devices device = new Devices
                {
                    DeviceId = strDeviceID,
                    Name = strName,
                    Class = strClass,
                    NameSpace = strNameSpace,
                    DeviceName = strDeviceName,
                    CurAddress = strCurAddress,
                    WorkSize = workSize,
                    IsShowUi = strIsShowUi,
                };
                if (string.IsNullOrEmpty(strNameSpace))
                {
                    //当前节点命名空间为空，赋值给下一层
                    device.NameSpace = context.GlobalNameSpace;
                    context.CurNameSpace = device.NameSpace;
                }
                else
                {
                    context.CurNameSpace = strNameSpace;
                }

                context.SetXmlNode(context.strNodePath + "[@DeviceId='" + strDeviceID + "']" + "[@Name='" + strName + "']");

                context.SetState(new BusHandle());
                context.Request();
                context.ReMoveStartNodePath("/BusinessHandle");
                if (context.CurNextNodeInfo != null)
                {
                    device.BusinessHandle = (DeviceBusinessHandle)context.CurNextNodeInfo;
                }

                context.SetState(new ControlHandle());
                context.Request();
                context.ReMoveStartNodePath("/ControlHandle");
                if (context.CurNextNodeInfo != null)
                {
                    device.DeviceControlHandle = (DeviceControlHandle)context.CurNextNodeInfo;
                }

                tempDeviceLst.Add(device);

                //数据查询完成后 设置新路径(路径还原，回到当前设备的根节点)
                context.ReMoveStartNodePath("[@DeviceId");
            }
            context.CurNextNodeInfo = tempDeviceLst;
            context.ReMoveStartNodePath("/Device");
        }
    }
}
