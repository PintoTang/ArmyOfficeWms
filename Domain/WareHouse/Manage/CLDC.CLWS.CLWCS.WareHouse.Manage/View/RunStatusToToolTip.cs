using System;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.View
{
    public class RunStatusToToolTip : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is RunStateMode)
            {
                RunStateMode runStateMode = (RunStateMode)value;
                if (runStateMode.Equals(RunStateMode.Run))
                {
                    return "暂停";
                }
                else
                {
                    return "开始";
                }
            }
            else
            {
                return "暂停";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
