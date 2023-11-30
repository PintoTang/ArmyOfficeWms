using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
    public  interface IProperty
    {
        /// <summary>
        /// 创建属性对应的界面
        /// </summary>
        /// <returns></returns>
        UserControl CreateView();
    }
}
