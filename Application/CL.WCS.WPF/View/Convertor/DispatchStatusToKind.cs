using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.WCS.WPF.View.Convertor
{
    public class DispatchStatusToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DispatchState)
            {
                DispatchState dispatchState = (DispatchState)value;
                if (dispatchState.Equals(DispatchState.Off))
                {
                    return "CheckboxBlankCircleOutline";
                }
                else
                {
                    return "CheckboxMarkedCircle";
                }
            }
            else
            {

                return "LaptopOff";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
