using System;
using System.Reflection;

namespace CLDC.CLWS.CLWCS.Framework
{
    public class PropertyHepler
    {
        /// <summary>
        /// 自动拷贝属性名字相同的属性
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <typeparam name="TOld"></typeparam>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public static TOld AutoCopy<TNew, TOld>(TNew newValue, TOld oldValue)
        {
            try
            {
                var newValueType = newValue.GetType();//获得类型  
                var oldValueType = typeof(TOld);
                foreach (PropertyInfo newValueProperty in newValueType.GetProperties())//获得类型的属性字段  
                {
                    if (!newValueProperty.CanRead)
                    {
                        continue;
                    }
                    foreach (PropertyInfo oldeValueProperty in oldValueType.GetProperties())
                    {
                        if (oldeValueProperty.CanRead && oldeValueProperty.CanWrite)
                        {
                            if (oldeValueProperty.Name == newValueProperty.Name && oldeValueProperty.PropertyType == newValueProperty.PropertyType)//判断属性名是否相同  
                            {
                                var newvalue = newValueProperty.GetValue(newValue, null);
                                oldeValueProperty.SetValue(oldValue, newvalue, null);//获得s对象属性的值复制给d对象的属性  
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oldValue;
        }

    }
}
