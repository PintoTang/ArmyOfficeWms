using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DbHelper
{
    public abstract class DatabaseBusinessAbstract<T>
    {
        protected readonly IDbHelper DbHelper;
        public DatabaseBusinessAbstract(IDbHelper dbHelper)
        {
            this.DbHelper = dbHelper;
        }

        private object _lockObj = new object();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult Save(T data)
        {
            OperateResult savaResult;
            lock (_lockObj)
            {
                if (IsExist(data).IsSuccess)
                {
                    savaResult = Update(data);
                }
                else
                {
                    savaResult = Insert(data);
                }
            }
            return savaResult;
        }
        /// <summary>
        /// 异步保存信息
        /// </summary>
        /// <param name="data"></param>
        public void SaveAsync(T data)
        {
            Task.Run(new Action(()=>
            {
                lock (_lockObj)
                {
                    if (IsExist(data).IsSuccess)
                    {
                        Update(data);
                    }
                    else
                    {
                        Insert(data);
                    }
                }
            }));
        }

        /// <summary>
        /// 是否存在指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract OperateResult IsExist(T data);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract OperateResult Update(T data);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract OperateResult Insert(T data);
    }
}
