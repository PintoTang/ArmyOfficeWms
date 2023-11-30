using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class RunStateToBackground : IValueConverter
    {
        SolidColorBrush runStateModeIn_Pause_ResetColorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        SolidColorBrush runStateModeIn_Run_Restore_ResetColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        SolidColorBrush defaultColorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is RunStateMode)
            {
                RunStateMode runState = (RunStateMode)value;
                switch (runState)
                {
                    case RunStateMode.Run:
                    case RunStateMode.Restore:
                    case RunStateMode.Reset:
                        return runStateModeIn_Run_Restore_ResetColorBrush;
                    case RunStateMode.Pause:
                    case RunStateMode.Stop:
                        return runStateModeIn_Pause_ResetColorBrush;
                    default:
                        return defaultColorBrush;
                }
            }
            else
            {
                return runStateModeIn_Pause_ResetColorBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
