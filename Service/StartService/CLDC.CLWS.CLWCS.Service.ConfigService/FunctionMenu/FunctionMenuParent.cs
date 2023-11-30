using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    /// <summary>
    /// 菜单读取类
    /// </summary>
    public class FunctionMenuParent:StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeName = @"/Function";
            string strNodeParms = context.strNodePath + strNodeName;
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            foreach (var nvc in nodeValueslst)
            {
                FunctionMenuInfo funcMenuInfo = new FunctionMenuInfo();
                string id = nvc["Id"];
                string name = nvc["Name"];
                string functionName = nvc["FunctionName"];

                funcMenuInfo.Id = int.Parse(id);
                funcMenuInfo.Name = name;
                funcMenuInfo.FunctionName = functionName;

                context.SetState(new FunctionBusinessHandle());
                string curNodeName = strNodeParms + "[@Id='" + id + "'][@Name='" + name + "']";
                context.SetXmlNode(curNodeName);
                context.Request();

                if (context.CurNextNodeInfo != null)
                {
                    funcMenuInfo.FuncBusinessHandleLst = (List<FuncBusinessHandle>)context.CurNextNodeInfo;
                }
                FunctionManage.InStance.Add(funcMenuInfo);
            }
        }
    }
}
