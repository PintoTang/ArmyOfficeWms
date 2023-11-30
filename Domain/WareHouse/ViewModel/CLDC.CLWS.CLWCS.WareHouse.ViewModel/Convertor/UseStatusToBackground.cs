using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class UseStatusToBackground : IValueConverter
    {
        SolidColorBrush disableUseStateColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush enableUseStateColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is UseStateMode)
            {
               UseStateMode useStateMode = (UseStateMode)value;
               if (useStateMode.Equals(UseStateMode.Disable))
               {
                   return disableUseStateColorBrush;
               }
                else
               {
                   return enableUseStateColorBrush;
               }
            }
            return disableUseStateColorBrush;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
