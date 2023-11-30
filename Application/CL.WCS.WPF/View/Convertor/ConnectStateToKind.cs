using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace CL.WCS.WPF.View.Convertor
{
    public class ConnectStateToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ConnectState)
            {
                ConnectState connectState = (ConnectState) value;
                if (connectState.Equals(ConnectState.Offline))
                {
                    return "LocalAreaNetworkDisconnect";
                }
                else
                {
                    return "LocalAreaNetworkConnect";
                }
            }
            else
            {
                return "LocalAreaNetworkDisconnect";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
