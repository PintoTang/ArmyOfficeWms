using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class StartOrPauseToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is RunStateMode)
            {
                RunStateMode pauseOrStartState = (RunStateMode)value;
                if (pauseOrStartState.Equals(RunStateMode.Run))
                {
                    return "PauseCircleOutline";
                }
                else
                {
                    return "PlayCircleOutline";
                }
            }
            else
            {

                return "PauseCircleOutline";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
