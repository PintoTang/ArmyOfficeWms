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

    public class UseStateToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is UseStateMode)
            {
                UseStateMode dispatchState = (UseStateMode)value;
                if (dispatchState.Equals(UseStateMode.Disable))
                {
                    return "ShieldOutline";
                }
                else
                {
                    return "ShieldLock";
                }
            }
            else
            {

                return "ShieldOutline";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
