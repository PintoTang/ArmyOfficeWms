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
    public class TaskStateToBackground : IValueConverter
    {
        SolidColorBrush freeColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush notfreeColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is WorkStateMode)
            {
                WorkStateMode workState = (WorkStateMode)value;
                if (workState != WorkStateMode.Free)//空闲
                {
                    return notfreeColorBrush;
                }
                else
                {
                    return freeColorBrush;
                }
            }
            return freeColorBrush;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
