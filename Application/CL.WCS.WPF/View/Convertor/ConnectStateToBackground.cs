using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CL.WCS.WPF.View.Convertor
{
    public class ConnectStateToBackground : IValueConverter
    {

        SolidColorBrush offlineColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush onlineColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ConnectState)
            {
                ConnectState connectState = (ConnectState)value;
                if (connectState.Equals(ConnectState.Offline))
                {
                    return offlineColorBrush;
                }
                return onlineColorBrush;
            }
            return offlineColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
