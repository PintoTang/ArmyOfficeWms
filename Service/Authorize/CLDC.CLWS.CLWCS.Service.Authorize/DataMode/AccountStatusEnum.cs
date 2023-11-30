using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.Service.Authorize.DataMode
{
    public enum AccountStatusEnum
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        启用 = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        禁用 = 0
    }
}
