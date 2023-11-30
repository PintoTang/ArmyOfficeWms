using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class ConnectStateToIsOnLine : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ConnectState)
            {
                ConnectState connectState = (ConnectState) value;
                if (connectState.Equals(ConnectState.Offline))
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
                return false;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return ConnectState.Online;
                }
            }
            return ConnectState.Offline;
           
        }

    }
}
