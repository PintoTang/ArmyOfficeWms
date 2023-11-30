using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
    public class DirectionConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TransportDirectionEnum))
            {
                return Visibility.Hidden;
            }
            if (parameter==null)
            {
                return Visibility.Hidden;
            }
            TransportDirectionEnum curDirection = (TransportDirectionEnum)Enum.Parse(typeof (TransportDirectionEnum), parameter.ToString());
            TransportDirectionEnum needDirection = (TransportDirectionEnum)Enum.Parse(typeof(TransportDirectionEnum), value.ToString());
            if (needDirection.Equals(TransportDirectionEnum.Both))
            {
                return Visibility.Visible;
            }
            if (curDirection.Equals(needDirection))
            {
                return Visibility.Visible;
            }           
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
