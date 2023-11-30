using System;
using System.Globalization;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel.Convertor
{
    public class WcsAddrToShowConvertor : IValueConverter
    {
        readonly IWmsWcsArchitecture _architecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string addrValue;
            if(value is Addr)
            {
                addrValue = ((Addr) value).FullName;
            }
            else if (value is string)
            {
                addrValue = value.ToString();
            }
            else
            {
                return value;
            }
            OperateResult<string> toShow = _architecture.WcsToShowName(addrValue);
            if (!toShow.IsSuccess)
            {
                return addrValue;
            }
           return toShow.Content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fromAddr = value as string;
            if (fromAddr == null)
            {
                return value;
            }
            OperateResult<string> toShow = _architecture.ShowNameToWcs(fromAddr);
            if (!toShow.IsSuccess)
            {
                return fromAddr;
            }
            if (targetType == typeof (Addr))
            {
                return new Addr(toShow.Content);
            }
            return value;
        }
    }
}
