using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.WCS.WPF.View.Convertor
{
     public class RunStatusToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string deviceRunStatuKindName = "PauseCircleOutline";
            if (value is RunStateMode)
            {
              
                RunStateMode pauseOrStartState = (RunStateMode)value;
                switch (pauseOrStartState)
                {
                  case RunStateMode.Run:
                        deviceRunStatuKindName = "PlayCircle";

                    break;
                case RunStateMode.Pause:
                    deviceRunStatuKindName = "PauseCircle";

                    break;
                case RunStateMode.Stop:
                    deviceRunStatuKindName = "StopCircle";

                    break;
                case RunStateMode.Reset:
                    deviceRunStatuKindName = "BackupRestore";

                    break;
                case RunStateMode.Restore:
                    deviceRunStatuKindName = "BackupRestore";

                    break;
                default:

                    deviceRunStatuKindName = "CheckboxBlankCircleOutline";
                    break;
                }
                return deviceRunStatuKindName;
            }
            else
            {
                return "PauseCircleOutline";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
