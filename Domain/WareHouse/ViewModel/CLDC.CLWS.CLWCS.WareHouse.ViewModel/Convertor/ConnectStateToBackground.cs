using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class ConnectStateToBackground : IValueConverter
    {
        SolidColorBrush OfflineStateColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush OnlineStateColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ConnectState)
            {
                ConnectState connectState = (ConnectState)value;
                if (connectState.Equals(ConnectState.Offline))
                {
                    return OfflineStateColorBrush;
                }
                else
                {
                    return OnlineStateColorBrush;
                }
            }
            else
            {
                return OfflineStateColorBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
