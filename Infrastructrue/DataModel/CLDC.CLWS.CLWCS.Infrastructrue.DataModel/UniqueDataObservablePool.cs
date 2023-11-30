using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    public sealed class UniqueDataObservablePool<T> where T : IUnique
    {
        private ObservableCollection<T> _dataPool = new ObservableCollection<T>();


        /// <summary>
        /// 数据池容器
        /// </summary>
        public ObservableCollection<T> DataPool
        {
            get { return _dataPool; }
            set
            {
                lock (_dataPool)
                {
                    _dataPool = value;
                }
            }
        }

        /// <summary>
        /// 容器数量
        /// </summary>
        public int Lenght
        {
            get { return _dataPool.Count; }
        }

        public int Count(Func<T, bool> predicate)
        {
            lock (_dataPool)
            {
                return _dataPool.Count(predicate);
            }
        }

        public void ClearPool()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _dataPool.Clear();
            });
        }

        public List<T> ToList()
        {
            List<T> tempList=new List<T>();
            foreach (T data in DataPool)
            {
                tempList.Add(data);
            }
            return tempList;
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
            lock (_dataPool)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!_dataPool.Any(d => d.UniqueCode.Equals(data.UniqueCode)))
                    {
                        _dataPool.Add(data);
                    }
                });
            }
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
            lock (_dataPool)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dataPool.Remove(data);
                });
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult RemovePool(Predicate<T> predicate)
        {
            lock (_dataPool)
            {
                T data = _dataPool.FirstOrDefault(t => predicate(t));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dataPool.Remove(data);
                });
            }
            return OperateResult.CreateSuccessResult();
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
            lock (_dataPool)
            {

                T tempData = _dataPool.FirstOrDefault(d => d.Equals(data));
                if (tempData != null)
                {
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
            lock (_dataPool)
            {
                T tempData = _dataPool.FirstOrDefault(o => predicate(o));
                if (tempData != null)
                {
                    return OperateResult.CreateSuccessResult(tempData);
                }
            }
            return OperateResult.CreateFailedResult(default(T), "获取失败");

        }

        public IEnumerable<T> FindAllData(Predicate<T> predicate)
        {
            lock (_dataPool)
            {
                return _dataPool.Where(t => predicate(t));
            }
        }
    }
}
