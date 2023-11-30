using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.WCS.WPF.View.Convertor
{
    public class DispatchStatusToBackground : IValueConverter
    {
        SolidColorBrush offDispatchColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush onfDispatchColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DispatchState)
            {
                DispatchState dispatchState = (DispatchState) value;
                if (dispatchState.Equals(DispatchState.Off))
                {
                    return offDispatchColorBrush;
                }
                else
                {
                    return onfDispatchColorBrush;
                }
            }
            return offDispatchColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
