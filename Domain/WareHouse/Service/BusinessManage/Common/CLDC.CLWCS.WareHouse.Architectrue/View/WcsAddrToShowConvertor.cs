using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWCS.WareHouse.Architectrue.WcsAddress
{
    public class WcsAddrToShowConvertor : IValueConverter
    {
        readonly IWmsWcsArchitecture _architecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Addr fromAddr=value as Addr;
            if (fromAddr==null)
            {
                return value;
            }
            OperateResult<string> toShow = _architecture.WcsToShowName(fromAddr);
            if (!toShow.IsSuccess)
            {
                return fromAddr;
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
            OperateResult<Addr> toShow = _architecture.ShowNameToWcs(fromAddr);
            if (!toShow.IsSuccess)
            {
                return fromAddr;
            }
            return toShow.Content;
        }
    }
}
