using System;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.View
{
    public class RunStateToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            RunStateMode pauseOrStartState = (RunStateMode)value;
            SolidColorBrush colorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            if (value is RunStateMode)
            {
                RunStateMode runState = (RunStateMode)value;
                switch (runState)
                {
                    case RunStateMode.Run:
                    case RunStateMode.Restore:
                    case RunStateMode.Reset:
                        colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
                        break;
                    case RunStateMode.Pause:
                    case RunStateMode.Stop:
                        colorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        break;
                    default:
                        colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                        //colorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        // colorBrush = new SolidColorBrush(Color.FromRgb(58, 200, 200));
                        break;
                }
                return colorBrush;
            }
            else
            {
                return colorBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
