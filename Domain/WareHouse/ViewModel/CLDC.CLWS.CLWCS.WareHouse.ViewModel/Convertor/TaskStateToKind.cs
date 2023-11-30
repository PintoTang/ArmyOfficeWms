using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class TaskStateToKind : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is WorkStateMode)
            {

                WorkStateMode workState = (WorkStateMode)value;
                if (workState != WorkStateMode.Free)//空闲
                {
                    return  "CheckboxMarkedCircle";//有
                  
                }
                else
                {
                    return  "CheckboxBlankCircleOutline";
                }
            }
            else
            {

                return "CheckboxBlankCircleOutline";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
