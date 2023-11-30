using System;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    /// <summary>
    /// 任务类型的公共拥有属性
    /// </summary>
    public interface IUnique : ICloneable
    {
        /// <summary>
        /// 任务ID号
        /// </summary>
         string UniqueCode { get; set; }
    }
}
