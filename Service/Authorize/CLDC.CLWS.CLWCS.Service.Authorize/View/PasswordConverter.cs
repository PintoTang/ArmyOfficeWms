using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.Infrastructrue.Security;

namespace CLDC.CLWS.CLWCS.Service.Authorize.View
{
    public class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string decryptPd = SecurityHelper.Decrypt(value.ToString());
                return decryptPd;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string encryptPd = SecurityHelper.Encrypt(value.ToString());
                return encryptPd;
            }
            return string.Empty;
        }
    }
}
