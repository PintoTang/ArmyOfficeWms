using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
    public interface IShowCard
    {
        /// <summary>
        /// 切换卡片显示状态
        /// </summary>
        /// <param name="state"></param>
        void ChangeViewState(ShowCardShowEnum state);
    }
}
