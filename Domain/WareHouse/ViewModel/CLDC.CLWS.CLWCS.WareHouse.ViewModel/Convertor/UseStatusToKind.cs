using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{

    public class UseStatusToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is UseStateMode)
            {
                UseStateMode dispatchState = (UseStateMode)value;
                if (dispatchState.Equals(UseStateMode.Disable))
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

                return "CheckboxBlankCircleOutline";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
