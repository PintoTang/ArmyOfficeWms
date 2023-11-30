using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CL.Framework.CmdDataModelPckg;


namespace CL.WCS.WPF.View.Convertor
{
    public class CurAddrToValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Addr)
            {
                Addr address = (Addr)value;//CurrAddress
                return address.ToString();
            }
            else
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
