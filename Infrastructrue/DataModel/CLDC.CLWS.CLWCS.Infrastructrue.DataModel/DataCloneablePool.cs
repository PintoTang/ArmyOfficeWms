using System;
using System.Collections.Generic;
using System.Linq;
using CLDC.CLWS.CLWCS.Framework;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    public class DataCloneablePool<T> : ViewModelBase where T : ICloneable
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
                    RaisePropertyChanged("DataPool");
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

        public int Count(Func<T, bool> predicate)
        {
            lock (dataPool)
            {
                return dataPool.Count(predicate);
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
                if (dataPool.Exists(p => p.Equals(data)))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    failResult.Message = string.Format("已存在： {0}", data.ToString());
                    return failResult;
                }

                dataPool.Add(data);
            }
            RaisePropertyChanged("DataPool");
            RaisePropertyChanged("Lenght");
            return OperateResult.CreateSuccessResult();
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
                if (!dataPool.Exists(d => d.Equals(data)))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    failResult.Message = string.Format("不存在：{0}", data.ToString());
                    return failResult;
                }

                bool result = dataPool.Remove(data);
                if (result)
                {
                    RaisePropertyChanged("DataPool");
                    RaisePropertyChanged("Lenght");
                    return OperateResult.CreateSuccessResult(string.Format("移除成功：{0}", data.ToString()));
                }
                return OperateResult.CreateFailedResult(string.Format("移除失败：{0}", data.ToString()), 1);
            }
        }

        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult RemovePool(Predicate<T> predicate)
        {
            lock (dataPool)
            {

                if (!dataPool.Exists(predicate))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    return failResult;
                }
                T data = dataPool.Find(predicate);

                bool result = dataPool.Remove(data);
                if (result)
                {
                    RaisePropertyChanged("DataPool");
                    RaisePropertyChanged("Lenght");
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
            OperateResult<T> getResult = FindData(predicate);
            if (!getResult.IsSuccess)
            {
                return AddPool(data);
            }
            OperateResult result = new OperateResult();
            try
            {
                PropertyHepler.AutoCopy(data, getResult.Content);
                RaisePropertyChanged("DataPool");
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
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

            OperateResult<T> getResult = FindData(data);
            if (!getResult.IsSuccess)
            {
                return AddPool(data);
            }
            OperateResult result = new OperateResult();
            try
            {
                PropertyHepler.AutoCopy(data, getResult.Content);
                RaisePropertyChanged("DataPool");
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
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
                if (dataPool.Exists(d => d.Equals(data)))
                {
                    T tempData = dataPool.FirstOrDefault(d => d.Equals(data));
                    return OperateResult.CreateSuccessResult(tempData);
                }
            }

            return OperateResult.CreateFailedResult(default(T), "获取失败");

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
                    T tempData = dataPool.FirstOrDefault(o => predicate(o));
                    return OperateResult.CreateSuccessResult(tempData);
                }
            }
            return OperateResult.CreateFailedResult(default(T), "获取失败");

        }

        public List<T> FindAllData(Predicate<T> predicate)
        {
            lock (dataPool)
            {

                if (dataPool.Exists(predicate))
                {
                    return dataPool.FindAll(predicate);
                }
                else
                {
                    return new List<T>();
                }
            }

        }

        public List<T> Clone()
        {
            List<T> cloneList = new List<T>();
            lock (dataPool)
            {
                foreach (T item in dataPool)
                {
                    cloneList.Add((T)item.Clone());
                }
            }
            return cloneList;
        }

    }
}
