using System;
using System.ComponentModel;
using System.Reflection;

namespace CLDC.CLWS.CLWCS.Framework
{
   public static class EnumEx
    {
        public static string GetDescription(this Enum enEnum)
        {

            Type type = enEnum.GetType();   //获取类型  
            MemberInfo[] memberInfos = type.GetMember(enEnum.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                as DescriptionAttribute[];   //获取描述特性  
                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述
                }
            }
            return enEnum.ToString();

        }
    }
}
