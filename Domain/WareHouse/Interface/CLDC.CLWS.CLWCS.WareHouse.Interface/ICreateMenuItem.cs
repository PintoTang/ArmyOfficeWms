using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
   public interface ICreateMenuItem
   {
       /// <summary>
       /// 创建菜单项
       /// </summary>
       /// <returns></returns>
       ObservableCollection<MenuItem> CreateMenuItem();
   }
}
