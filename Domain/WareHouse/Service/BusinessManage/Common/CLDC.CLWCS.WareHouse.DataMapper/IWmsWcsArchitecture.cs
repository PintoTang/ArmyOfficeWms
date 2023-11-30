using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWCS.WareHouse.DataMapper
{
    /// <summary>
    /// Wms与Wcs的基础数据
    /// </summary>
    public interface IWmsWcsArchitecture
    {

        /// <summary>
        /// 获取所有的显示名称
        /// </summary>
        /// <returns></returns>
       List<string> GetAllShowName();

        /// <summary>
        /// 更新内存信息
        /// </summary>
        /// <returns></returns>
        OperateResult Refresh();

        /// <summary>
        /// Wcs地址转换为Wms地址
        /// </summary>
        /// <param name="destAddr"></param>
        /// <returns></returns>
        OperateResult<string> WcsToWmsAddr(string destAddr);
        /// <summary>
        /// Wms地址转换为Wcs地址
        /// </summary>
        /// <param name="destAddr"></param>
        /// <returns></returns>
        OperateResult<string> WmsToWcsAddr(string destAddr);
        /// <summary>
        /// Wcs地址转换为下层地址
        /// </summary>
        /// <param name="wcsAddr"></param>
        /// <returns></returns>
        OperateResult<string> WcsToLowerAddr(string  wcsAddr);
        /// <summary>
        /// wcs地址转换显示名字
        /// </summary>
        /// <param name="wcsAddr"></param>
        /// <returns></returns>
        OperateResult<string> WcsToShowName(string  wcsAddr);
        /// <summary>
        /// 上层地址转换为显示名字
        /// </summary>
        /// <param name="upperAddr"></param>
        /// <returns></returns>
        OperateResult<string> UpperAddrToShowName(string upperAddr);

        /// <summary>
        /// 显示名字转换Wcs的地址
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        OperateResult<string> ShowNameToWcs(string name);


    }
}
