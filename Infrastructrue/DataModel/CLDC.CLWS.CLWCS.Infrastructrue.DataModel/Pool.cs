using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    /// <summary>
    /// 池 主要对数据的操作及存储
    /// </summary>
    public class Pool<T>
    {
        private List<T> container = new List<T>();


        /// <summary>
        /// 数据池容器
        /// </summary>
        public List<T> Container
        {
            get { return container; }
            set { container = value; }
        }

        /// <summary>
        /// 容器数量
        /// </summary>
        public int Count
        {

            get
            {
                lock (container)
                {
                    return container.Count;
                }
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
            lock (container)
            {
                if (container.Exists(p => p.Equals(data)))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    failResult.Message = string.Format("已存在： {0}", data.ToString());
                    return failResult;
                }
                container.Add(data);
                return OperateResult.CreateSuccessResult();
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
            lock (container)
            {
                if (!container.Exists(d => d.Equals(data)))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    failResult.Message = string.Format("不存在：{0}", data.ToString());
                    return failResult;
                }
                bool result = container.Remove(data);
                if (result)
                {
                    return OperateResult.CreateSuccessResult(string.Format("移除成功：{0}", data.ToString()));
                }
                return OperateResult.CreateFailedResult(string.Format("移除失败：{0}", data.ToString()), 1);
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
            lock (container)
            {
                OperateResult<T> getResult = ObtainData(predicate);
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
            lock (container)
            {
                OperateResult<T> getResult = ObtainData(data);
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
        public OperateResult<T> ObtainData(T data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult(data, "参数为空");
            }
            lock (container)
            {
                if (container.Exists(d => d.Equals(data)))
                {
                    T tempData = container.FirstOrDefault(d => d.Equals(data));
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
        public OperateResult<T> ObtainData(Predicate<T> predicate)
        {
            lock (container)
            {
                if (container.Exists(predicate))
                {
                    T TempData = container.FirstOrDefault(o => predicate(o));
                    return OperateResult.CreateSuccessResult(TempData);
                }
                return OperateResult.CreateFailedResult(default(T), "获取失败");
            }
        }

        //public List<T> Clone()
        //{
        //	List<T> cloneList = new List<T>();
        //	lock (container)
        //	{
        //		foreach (T item in container)
        //		{
        //			cloneList.Add((T)item.Clone());
        //		}
        //	}
        //	return cloneList;
        //}

    }
}
