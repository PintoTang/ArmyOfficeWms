using System;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel.Convertor
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
