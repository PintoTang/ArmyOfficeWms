using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class UseStateToToolTip : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is UseStateMode)
            {
                UseStateMode useStateMode = (UseStateMode)value;
                if (useStateMode.Equals(UseStateMode.Disable))
                {
                    return "启用";
                }
                else
                {
                    return "禁用";
                }
            }
            else
            {
                return "禁用";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
