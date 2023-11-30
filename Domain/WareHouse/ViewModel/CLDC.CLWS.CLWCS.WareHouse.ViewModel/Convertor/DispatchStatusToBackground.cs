using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class DispatchStatusToBackground : IValueConverter
    {
        SolidColorBrush OffDispatchStateColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush OnDispatchStateColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DispatchState)
            {
                DispatchState dispatchState = (DispatchState)value;
                if (dispatchState.Equals(DispatchState.Off))
                {
                    return OffDispatchStateColorBrush;
                }
                else
                {
                    return OnDispatchStateColorBrush;
                }
            }
            return OffDispatchStateColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
