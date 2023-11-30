using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CLDC.Infrastructrue.UserCtrl.Convertor
{
    public class DateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DateTime.Now;
            }
            string stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                DateTime outDt;
                bool isOk = DateTime.TryParse(value.ToString(), out outDt);
                if (isOk)
                {
                    return outDt;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            bool checkType = value is DateTime;
            if (checkType)
            {
                return value.ToString();
            }
            else
            {
                return value;
            }
        }
    }
}
