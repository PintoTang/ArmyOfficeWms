using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CL.Framework.CmdDataModelPckg;


namespace CL.WCS.WPF.View.Convertor
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
