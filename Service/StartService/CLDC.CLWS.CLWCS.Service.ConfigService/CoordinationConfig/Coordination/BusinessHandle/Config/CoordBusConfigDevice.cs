using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class CoordBusConfigDevice : StateAbstract
    {
        public override void handle(Context context)
        {
            string strNodeParms = context.strNodePath + "/Devices/Device";
            var nodeValueslst = context.xml.GetNodeValuesList(strNodeParms);

            List<CoorDevice> tempCoorDeviceLst = new List<CoorDevice>();
            foreach (var nvc in nodeValueslst)
            {
                CoorDevice coorDevice = new CoorDevice
                {
                  
                    Type = nvc["Type"],
                };

                if (!string.IsNullOrEmpty(nvc["DeviceId"]))
                {
                    coorDevice.DeviceId = int.Parse(nvc["DeviceId"].Trim());
                }


                if(!string.IsNullOrEmpty(nvc["IsCheckBarcode"]))
                {
                    coorDevice.IsCheckReady = (nvc["IsCheckReady"].ToUpper() == "TRUE") ? true : false;
                }
                else
                {
                    coorDevice.IsCheckReady = false;
                }

                if (!string.IsNullOrEmpty(nvc["IsCheckBarcode"]))
                {
                    coorDevice.IsCheckBarcode = (nvc["IsCheckBarcode"].ToUpper() == "TRUE") ? true : false;
                }
                else
                {
                    coorDevice.IsCheckBarcode = false;
                }
               
              
                coorDevice.DestAddress = nvc["DestAddress"];
                tempCoorDeviceLst.Add(coorDevice);
            }
            context.CurNextNodeInfo = tempCoorDeviceLst;
        }
    }
}
