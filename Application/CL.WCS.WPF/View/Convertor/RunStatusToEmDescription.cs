using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.WCS.WPF.View.Convertor
{
    public class RunStatusToEmDescription : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is RunStateMode)
            {
                RunStateMode runStateMode = (RunStateMode)value;
                return GetDescription(runStateMode);
            }
            else
            {
                return RunStateMode.Run;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        /// <summary>  
        /// 获取枚举描述
        /// </summary>  
        /// <param name="en">枚举</param>  
        /// <returns>返回枚举的描述 </returns>  
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();   //获取类型  
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                as DescriptionAttribute[];   //获取描述特性  
                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述
                }
            }
            return en.ToString();

        }

    }
}
