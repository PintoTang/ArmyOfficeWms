using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.Infrastructrue.UserCtrl.Convertor
{
    public class EnumDescriptionConverter : IValueConverter
    {
       

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum @enum = value as Enum;
            if (@enum != null)
            {
                Enum myEnum = @enum;
                string description = myEnum.GetDescription();
                return description;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
