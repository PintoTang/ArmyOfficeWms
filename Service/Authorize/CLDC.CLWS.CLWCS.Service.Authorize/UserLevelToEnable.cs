using System;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    public class UserLevelToEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            bool isUserLevel = value is RoleLevelEnum;
            if (!isUserLevel)
            {
                return false;
            }
            bool isRightPara = parameter is RoleLevelEnum;
            if (!isRightPara)
            {
                return false;
            }
            RoleLevelEnum curRole = (RoleLevelEnum) value;
            RoleLevelEnum defineRole = (RoleLevelEnum) parameter;
            if (curRole>=defineRole)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
