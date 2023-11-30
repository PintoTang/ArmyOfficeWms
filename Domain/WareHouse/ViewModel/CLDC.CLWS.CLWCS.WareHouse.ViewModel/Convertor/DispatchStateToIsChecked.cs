using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class DispatchStateToIsChecked : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DispatchState)
            {
                DispatchState dispatchState = (DispatchState)value;
                if (dispatchState.Equals(DispatchState.Off))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           if(value is bool)
           {
               if((bool) value)
               {
                   return DispatchState.On;
               }
           }
           return DispatchState.Off;
        }
    }
}
