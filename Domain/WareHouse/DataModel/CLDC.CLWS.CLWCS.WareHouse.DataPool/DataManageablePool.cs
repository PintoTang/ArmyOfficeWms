using System;
using System.Collections.Generic;
using System.Linq;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataPool
{
    public class DataManageablePool<T> where T : IManageable
    {
        private List<T> dataPool = new List<T>();

        /// <summary>
        /// 数据池容器
        /// </summary>
        public List<T> DataPool
        {
            get { return dataPool; }
            set
            {
                lock (dataPool)
                {
                    dataPool = value;
                }
            }
        }

        /// <summary>
        /// 容器数量
        /// </summary>
        public int Lenght
        {
            get { return dataPool.Count; }
        }

        public int Count(Predicate<T> predicate)
        {
            lock (dataPool)
            {
                return dataPool.Count(t=>predicate(t));
            }
        }

        /// <summary>
        /// 添加指定的数据到数据池
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult AddPool(T data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult("添加参数为空", 1);
            }
            lock (dataPool)
            {
                if (DataPool.Exists(p => p.Id.Equals(data.Id) || p.Name.Equals(data.Name)))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    failResult.Message = string.Format("ID： {0} 或者名字：{1} 已存在", data.Id, data.Name);
                    return failResult;
                }
                DataPool.Add(data);
                return OperateResult.CreateSuccessResult();
            }
        }

        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult RemovePool(int id)
        {
            lock (dataPool)
            {
                OperateResult<T> removeData = FindData(id);
                if (!removeData.IsSuccess)
                {
                    string msg = string.Format("不存在编号为：{0} 的数据", id);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                bool result = DataPool.Remove(removeData.Content);
                if (result)
                {
                    return OperateResult.CreateSuccessResult(string.Format("编号为：{0} 的数据，移除成功", id));
                }
                return OperateResult.CreateFailedResult(string.Format("编号为：{0} 的数据，移除失败", id), 1);
            }
        }

        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult RemovePool(T data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult("删除参数为空", 1);
            }
            lock (dataPool)
            {
                OperateResult<T> removeData = FindData(data.Id);
                if (!removeData.IsSuccess)
                {
                    string msg = string.Format("不存在编号为：{0} 的数据", data.Id);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                bool result = DataPool.Remove(removeData.Content);
                if (result)
                {
                    return OperateResult.CreateSuccessResult(string.Format("编号为：{0} 的数据，移除成功", data.Id));
                }
                return OperateResult.CreateFailedResult(string.Format("编号为：{0} 的数据，移除失败", data.Id), 1);
            }
        }

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public OperateResult UpdatePool(int id, T data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult("参数对象为空", 1);
            }
            lock (dataPool)
            {
                OperateResult<T> getResult = FindData(id);
                if (!getResult.IsSuccess)
                {
                    return AddPool(data);
                }
                OperateResult removeResult = RemovePool(getResult.Content);
                if (!removeResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("删除原来数据失败，信息源：{0}", data), 1);
                }
                OperateResult addResult = AddPool(data);
                return addResult;
            }
        }

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public OperateResult UpdatePool(T data, Predicate<T> predicate)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult("参数对象为空", 1);
            }
            lock (dataPool)
            {
                OperateResult<T> getResult = FindData(predicate);
                if (!getResult.IsSuccess)
                {
                    return AddPool(data);
                }
                OperateResult removeResult = RemovePool(getResult.Content);
                if (!removeResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("删除原来数据失败，信息源：{0}", data), 1);
                }
                OperateResult addResult = AddPool(data);
                return addResult;
            }
        }

        /// <summary>
        /// 更新数据池
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult UpdatePool(T data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult("参数对象为空", 1);
            }
            lock (dataPool)
            {
                OperateResult<T> getResult = FindData(data);
                if (!getResult.IsSuccess)
                {
                    return AddPool(data);
                }
                OperateResult removeResult = RemovePool(getResult.Content);
                if (!removeResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("删除原来数据失败，信息源：{0}", data), 1);
                }
                OperateResult addResult = AddPool(data);
                return addResult;
            }
        }

        /// <summary>
        /// 获取指定条件的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult<T> FindData(int id)
        {
            lock (dataPool)
            {
                if (DataPool.Exists(d => d.Id.Equals(id)))
                {
                    T tempData = DataPool.FirstOrDefault(d => d.Id.Equals(id));
                    return OperateResult.CreateSuccessResult(tempData);
                }
                return OperateResult.CreateFailedResult(default(T), "获取失败");
            }
        }

        /// <summary>
        /// 通过名称获取数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public OperateResult<T> FindData(string name)
        {

            lock (dataPool)
            {
                if (DataPool.Exists(d => d.Name.Equals(name)))
                {
                    T tempData = DataPool.FirstOrDefault(d => d.Name.Equals(name));
                    return OperateResult.CreateSuccessResult(tempData);
                }
                return OperateResult.CreateFailedResult(default(T), "获取失败");
            }

        }

        /// <summary>
        /// 获取指定条件的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult<T> FindData(T data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult(data, "参数为空");
            }
            lock (dataPool)
            {
                if (DataPool.Exists(d => d.Equals(data)))
                {
                    T tempData = DataPool.FirstOrDefault(d => d.Id.Equals(data));
                    return OperateResult.CreateSuccessResult(tempData);
                }
                return OperateResult.CreateFailedResult(default(T), "获取失败");
            }
        }

        /// <summary>
        /// 根据条件获取指定的值
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public OperateResult<T> FindData(Predicate<T> predicate)
        {
            lock (dataPool)
            {
                if (dataPool.Exists(predicate))
                {
                    T TempData = dataPool.FirstOrDefault(o => predicate(o));
                    return OperateResult.CreateSuccessResult(TempData);
                }
                return OperateResult.CreateFailedResult(default(T), "获取失败");
            }
        }

        /// <summary>
        /// 根据条件获取指定的值
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public OperateResult<List<T>> FindAllData(Predicate<T> predicate)
        {
            lock (dataPool)
            {
                if (dataPool.Exists(predicate))
                {
                    List<T> TempData = dataPool.FindAll(predicate);
                    return OperateResult.CreateSuccessResult(TempData);
                }
                return OperateResult.CreateFailedResult(default(List<T>), "获取失败");
            }
        }

    }
}
