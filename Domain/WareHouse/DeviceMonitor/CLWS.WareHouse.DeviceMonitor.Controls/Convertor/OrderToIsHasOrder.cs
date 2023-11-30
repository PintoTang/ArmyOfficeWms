using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CL.Framework.CmdDataModelPckg;

namespace WHSE.Monitor.Framework.UserControls.Convertor
{
    public class OrderToIsHasOrder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasOrder = false;
            if (value != null)
            {
                int order = ConvertHepler.ConvertToInt(value.ToString());
                hasOrder = order > 0;
            }
            return hasOrder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
