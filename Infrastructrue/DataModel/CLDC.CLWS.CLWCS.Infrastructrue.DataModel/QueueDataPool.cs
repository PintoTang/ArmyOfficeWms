using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
	public sealed class QueueDataPool<T>
	{
		/// <summary>
		/// 互斥锁
		/// 
		/// </summary>
		private static Mutex mutexORACLEPool = new Mutex();
		public Queue<T> pool;

		/// <summary>
		/// 数量
		/// 
		/// </summary>
		public int Count
		{
			get
			{
				return this.pool.Count;
			}
		}

		/// <summary>
		/// 队列数据池
		/// 
		/// </summary>
		public QueueDataPool()
		{
			this.pool = new Queue<T>();
		}

		/// <summary>
		/// 返回位于 Queue 开始处的对象但不将其移除。
		/// 
		/// </summary>
		/// 
		/// <returns/>
		public T Peek()
		{
			lock (this.pool)
				return this.pool.Peek();
		}

		/// <summary>
		/// 从池中取出数据
		/// 
		/// </summary>
		/// 
		/// <returns/>
		public T Dequeue()
		{
			lock (this.pool)
			{
				if (this.Count <= 0)
					return default(T);
				return this.pool.Dequeue();
			}
		}

		/// <summary>
		/// 增加一个数据到池中
		/// 
		/// </summary>
		/// <param name="item"/>
		public void Enqueue(T item)
		{
			if ((object)item == null)
				throw new ArgumentNullException("Items added to a QueueDataPool cannot be null");
			QueueDataPool<T>.mutexORACLEPool.WaitOne();
			try
			{
				lock (this.pool)
				{
					if (!this.pool.Contains(item))
						this.pool.Enqueue(item);
				}
			}
			catch
			{
			}
			QueueDataPool<T>.mutexORACLEPool.ReleaseMutex();
		}

		/// <summary>
		/// 清空
		/// 
		/// </summary>
		/// 
		/// <returns/>
		public T[] Clear()
		{
			T[] objArray = new T[0];
			QueueDataPool<T>.mutexORACLEPool.WaitOne();
			try
			{
				lock (this.pool)
				{
					objArray = this.pool.ToArray();
					this.pool.Clear();
				}
			}
			catch
			{
			}
			QueueDataPool<T>.mutexORACLEPool.ReleaseMutex();
			return objArray;
		}
	}
}
