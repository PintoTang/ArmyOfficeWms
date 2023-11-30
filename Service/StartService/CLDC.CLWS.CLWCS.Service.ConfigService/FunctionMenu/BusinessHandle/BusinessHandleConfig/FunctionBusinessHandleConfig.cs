using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class FunctionBusinessHandleConfig : StateAbstract
    {
        public override void handle(Context context)
        {
            //todo 
            string strNodeParms = context.strNodePath + "/Config/Modules/Module";//强改路径,注意还原路径
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);
            FuncBusinessHandleConfig funcBusHandelConfig = new FuncBusinessHandleConfig();
            foreach (var nvc in nodeValueslst)
            {
                string strName = nvc["Name"];
                string strType = nvc["Type"];
                string strClass = nvc["Class"];
                string strNameSpace = nvc["NameSpace"];

                funcBusHandelConfig.Modules.Add(new FuncBusinessHandleConfigModule
                {
                     Name = nvc["Name"],
                     Type =  nvc["Type"],
                     Class = nvc["Class"],
                     NameSpace= nvc["NameSpace"]
                });
            }
            //if (nodeValueslst.Count > 0)
            //{
            //    context.ReMoveStartNodePath("/Module");
            //    context.ReMoveStartNodePath("/Modules");//还原路径
            //}
            context.CurNextNodeInfo = funcBusHandelConfig;
        }
    }
}
