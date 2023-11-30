using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 装备的类型
    /// </summary>
    public enum EquipmentTypeEnum
    {
        /// <summary>
        /// 入口
        /// </summary>
        [Description("入口")]
        In=0,
        /// <summary>
        /// 出口
        /// </summary>
        [Description("出口")]
        Out =1,
        /// <summary>
        /// 出入口
        /// </summary>
        [Description("出入口")]
        InAndOut =2,
        /// <summary>
        /// 异常口
        /// </summary>
        [Description("异常口")]
        Exp =3,
        /// <summary>
        /// 拣选口
        /// </summary>
        [Description("拣选口")]
        Pick =4,
        /// <summary>
        /// 盘点口
        /// </summary>
        [Description("盘点口")]
        Inventory =5,
        /// <summary>
        /// 搬运设备
        /// </summary>
        [Description("搬运")]
        Transport=6
    }
}
