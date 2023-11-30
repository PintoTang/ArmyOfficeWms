using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    public abstract class BusinessHandleBasicPropertyAbstract : InstanceProperty, IProperty
    {
        public abstract UserControl CreateView();

    }
}
