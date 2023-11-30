using CLDC.CLWS.CLWCS.Service.ConfigService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class BusinessHandle : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/BusinessHandle";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            CoordBusinessHandle coorConfig = new CoordBusinessHandle();
            foreach (var nvc in nodeValueslst)
            {
                string strType = nvc["Type"];
               
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];

                coorConfig.Type = strType;
                coorConfig.Class = strClass;
                coorConfig.NameSpace = strNameSpace;
                if (string.IsNullOrEmpty(strNameSpace.Trim()))
                {
                    coorConfig.NameSpace = context.GlobalNameSpace;
                }
               

                context.SetXmlNode(strNodeParms + "[@Type='" + strType + "']");

                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new BusinessHandleConfig());
                context.Request();
              
                if (context.CurNextNodeInfo != null)
                {
                    coorConfig.CoordBusinessHandleConfig = (CoordBusinessHandleConfig)context.CurNextNodeInfo;
                }
             
            }
            context.CurNextNodeInfo = coorConfig;
        }
    }
}
