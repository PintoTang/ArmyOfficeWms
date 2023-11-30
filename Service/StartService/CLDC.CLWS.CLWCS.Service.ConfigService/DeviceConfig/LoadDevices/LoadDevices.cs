namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class LoadDevices : StateAbstract
    {
        public override void handle(Context context)
        {
            DeviceHelper.Handle(context, "/LoadDevices");
        }
    }

}