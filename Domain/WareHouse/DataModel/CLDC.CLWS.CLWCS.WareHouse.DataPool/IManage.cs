using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CL.Framework.CmdDataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.DataPool
{
    /// <summary>
    /// 可管理的接口
    /// </summary>
    public interface IManage<T> where T : IManageable
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        OperateResult Add(T data);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperateResult Delete(int id);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        OperateResult Update(T data);
        /// <summary>
        /// 通过ID查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindDeivceByDeviceId(int id);
        /// <summary>
        /// 通过名称查找
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        T FindDeviceByDeviceName(string name);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
       List<T> GetAllData();
    }
}
