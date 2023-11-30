using System;
using System.Windows;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel.Convertor
{
   public class UserLevelToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            bool isUserLevel = value is RoleLevelEnum;
            if (!isUserLevel)
            {
                return Visibility.Collapsed;
            }
            bool isRightPara = parameter is RoleLevelEnum;
            if (!isRightPara)
            {
                return Visibility.Collapsed;
            }
            RoleLevelEnum curRole = (RoleLevelEnum) value;
            RoleLevelEnum defineRole = (RoleLevelEnum) parameter;
            if (curRole>=defineRole)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
