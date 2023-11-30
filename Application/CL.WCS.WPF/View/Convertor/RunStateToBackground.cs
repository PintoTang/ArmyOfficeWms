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
    public class RunStateToBackground : IValueConverter
    {
        SolidColorBrush defaultColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush RunStateInRun_Restore_ResetColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        SolidColorBrush RunStatePauseAndStopColorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
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
                        return RunStateInRun_Restore_ResetColorBrush;
                    case RunStateMode.Pause:
                    case RunStateMode.Stop:
                        return RunStatePauseAndStopColorBrush;
                    default:
                        return defaultColorBrush;
                }
            }
            else
            {
                return RunStatePauseAndStopColorBrush;
            }
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
