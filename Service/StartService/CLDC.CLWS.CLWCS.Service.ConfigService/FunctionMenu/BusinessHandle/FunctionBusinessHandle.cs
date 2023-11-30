using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class FunctionBusinessHandle : StateAbstract
    {
        public override void handle(Context context)
        {
           string strNodeName = @"/BusinessHandle";
         
            string strNodeParms = context.strNodePath + strNodeName;
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);//只有一条记录
            List<FuncBusinessHandle> tempFuncBusinessHandleLst = new List<FuncBusinessHandle>();
            foreach (var nvc in nodeValueslst)
            {
                int id = (string.IsNullOrEmpty(nvc["Id"]) == true) ? 0 : int.Parse(nvc["Id"]);
                string name = (string.IsNullOrEmpty(nvc["Name"]) == true) ? "" : nvc["Name"];
                string type = nvc["Type"];
                string cls = nvc["Class"];
                string nSpace = nvc["NameSpace"];

                context.SetXmlNode(strNodeParms + "[@Type='" + type + "']");

                //不用读取Config内容 后期不需要 则可删除
                context.SetState(new FunctionBusinessHandleConfig());
                context.Request();


               

                FuncBusinessHandle funcBusinessHandle = new FuncBusinessHandle
                {
                    Id = id,
                    Name = name,
                    Type = type,
                    Class = cls,
                    NameSpace = nSpace,
                };
                if (context.CurNextNodeInfo != null)
                {
                    funcBusinessHandle.FuncBusinessHandleConfig = (FuncBusinessHandleConfig)context.CurNextNodeInfo;
                }

                tempFuncBusinessHandleLst.Add(funcBusinessHandle);
            }
            context.CurNextNodeInfo = tempFuncBusinessHandleLst;
        }
    }
}
