﻿using System;

namespace CLDC.CLWS.CLWCS.Framework
{
	public class SingletonProvider<T> where T : class
	{
		/*
         * 单线程测试通过！
         * 多线程测试通过！
         * 根据需要在调用的时候才实例化单例类！
          */
		private static T instance;
		private static readonly object SyncObject=new object();

		public static T Instance
		{
			get
			{
				if (instance == null)//没有第一重 singleton == null 的话，每一次有线程进入 GetInstance()时，均会执行锁定操作来实现线程同步，
				//非常耗费性能 增加第一重singleton ==null 成立时的情况下执行一次锁定以实现线程同步
				{
					lock (SyncObject)
					{
						if (instance == null)//Double-Check Locking 双重检查锁定
						{
							//instance = new T();
							//需要非公共的无参构造函数，不能使用new T() ,new不支持非公共的无参构造函数 
							instance = (T)Activator.CreateInstance(typeof(T), true); //第二个参数防止异常：“没有为该对象定义无参数的构造函数。”
						}
					}
				}
				return instance;
			}
		}
	}
}
