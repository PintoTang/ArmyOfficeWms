using System;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Convertor
{
    public class AddrConvertToValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Addr)
            {
                Addr address = (Addr)value;
                return address.ToString();
            }
            else
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is string)
                {
                    return new Addr(value.ToString());
                }
                return new Addr("");
            }
            catch (Exception ex)
            {
                return new Addr("");
            }
        }

    }
}
