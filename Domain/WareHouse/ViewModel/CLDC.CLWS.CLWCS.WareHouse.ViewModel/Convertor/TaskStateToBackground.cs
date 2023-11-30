using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class TaskStateToBackground : IValueConverter
    {
        SolidColorBrush freeWorkStateColoerBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        SolidColorBrush notFreeWorkStateColoerBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#228B22"));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is WorkStateMode)
            {
                WorkStateMode workState = (WorkStateMode)value;
                if (workState != WorkStateMode.Free)//空闲
                {
                    return notFreeWorkStateColoerBrush;

                }
                else
                {
                    return freeWorkStateColoerBrush;
                }
            }
            return freeWorkStateColoerBrush;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
