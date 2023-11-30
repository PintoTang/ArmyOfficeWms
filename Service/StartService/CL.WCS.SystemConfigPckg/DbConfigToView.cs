using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;

namespace CL.WCS.SystemConfigPckg
{
    public class DbConfigToView : IMultiValueConverter
    {
        //0:连接串
        //1:连接类型
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                if (values[1] is DatabaseTypeEnum)
                {
                    DatabaseTypeEnum databaseType = (DatabaseTypeEnum)values[1];
                    UserControl configView = databaseType.GetConfigView(values[0]);
                    return configView;
                }
            }
            return new TextBox() { Width = 200, Text = values[0].ToString() };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is ContentControl)
            {
                IDbConfiguration configView = (IDbConfiguration)(value as ContentControl).Content;
                if (configView != null)
                {
                    string connectString = configView.GetConnectionString();
                    return new object[] { connectString,null };
                }
            }

            return new object[] { string.Empty,null };

        }
    }
}
