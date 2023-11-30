using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;

namespace WHSE.Monitor.Framework.UserControls.Convertor
{
   public class DireValueToIsMove : IValueConverter
    {
       public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
           int direValue = ConvertHepler.ConvertToInt(value.ToString());
           if (direValue>0)
           {
               return true;
           }
           else
           {
               return false;
           }
       }

       public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
       {
           throw new NotImplementedException();
       }
    }
}
