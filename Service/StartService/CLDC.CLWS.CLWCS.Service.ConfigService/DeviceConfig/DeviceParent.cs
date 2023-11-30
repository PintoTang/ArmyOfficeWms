using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Service.ConfigService.DeviceConfig.PalletizerDevices;
using CLDC.CLWS.CLWCS.Service.ConfigService.DeviceConfig.RackPlaces;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class DeviceParent : StateAbstract
    {
       

        public override void handle(Context cc)
        {
            try
            {



                string strNodeParms = @"Configuration/Devices";

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new PalletizerDevices());
                cc.Request();

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new DisplayDevice());
                cc.Request();

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new SwitchDevice());
                cc.Request();

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new IdentityDevice());
                cc.Request();

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new RackPlaces());
                cc.Request();

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new LoadDevices());
                cc.Request();

                cc.SetXmlNode(strNodeParms);
                cc.SetState(new TransportDevices());
                cc.Request();


            }
            catch (Exception ex)
            {

            }
        }

    }
}
