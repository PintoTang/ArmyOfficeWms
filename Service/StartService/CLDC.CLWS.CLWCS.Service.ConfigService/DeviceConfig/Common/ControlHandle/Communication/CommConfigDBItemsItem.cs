using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CommConfigDBItemsItem : StateAbstract
    {
        public override void handle(Context context)
        {
            context.SetXmlNode(context.strNodePath + "/Item");
            var nodeValueslst = context.xml.GetNodeValuesList(context.strNodePath);
            context.CurNextNodeInfo = null;
            var curContent = new DeviceDataBlockItems();
            List<DataBlock> dataBlockLst = new List<DataBlock>();
            foreach (var nvc in nodeValueslst)
            {
                DataBlock dataBlock = new DataBlock
                {
                    Name = nvc["Name"],
                    DataBlockName = nvc["DataBlockName"],
                    realDataBlockAddr = nvc["realDataBlockAddr"]
                };
                dataBlockLst.Add(dataBlock);

            }
            curContent.DataBlockLst = dataBlockLst;

            context.CurNextNodeInfo = curContent;
        }
    }
}
