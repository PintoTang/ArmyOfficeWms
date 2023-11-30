using System;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Convertor
{
    public class DeviceNameToValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DeviceName)
            {
                DeviceName deviceName = (DeviceName)value;
                return deviceName.ToString();
            }
            else
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
