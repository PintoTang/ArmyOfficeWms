using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CL.Framework.CmdDataModelPckg;

namespace WHSE.Monitor.Framework.UserControls.Convertor
{
    /// <summary>
    /// 值转换为颜色
    /// </summary>
   public class ValueToBackgroudConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush brush=Brushes.LightGray;

            if (value != null)
            {
                int order = ConvertHepler.ConvertToInt(value.ToString());
                if (order<0)
                {
                    brush = Brushes.Yellow;
                }
                else if(order>0)
                {
                    brush = Brushes.Green;
                }
                else
                {
                    brush = Brushes.Gray;
                }
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
